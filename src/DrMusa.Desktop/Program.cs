using DrMusa.Business;
using DrMusa.Data.Seed;
using DrMusa.Data.Context;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DrMusa.Desktop;

internal static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File("logs/drmusa-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        ApplicationConfiguration.Initialize();
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // Load persisted session preferences (Remember Me)
        DrMusa.Desktop.Helpers.SessionManager.LoadSavedSettings();

        try
        {
            // Build DI container
            var services = new ServiceCollection();
            var dbPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "..", "..", "..", "..", "..", "database", "DrMusa.db");
            var dbFullPath = Path.GetFullPath(dbPath);
            var connectionString = $"Data Source={dbFullPath}";

            services.AddDrMusaServices(connectionString);
            ServiceProvider = services.BuildServiceProvider();

            // Automatic Backup Logic
            try
            {
                using var scope = ServiceProvider.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DrMusaDbContext>();
                
                string? customBackupDir = null;
                try
                {
                    customBackupDir = db.Settings.FirstOrDefault(s => s.Key == "AutoBackupDirectory")?.Value;
                }
                catch { /* Ignore if settings table does not exist yet (first run) */ }

                var backupDir = !string.IsNullOrWhiteSpace(customBackupDir) 
                    ? customBackupDir 
                    : Path.Combine(Path.GetDirectoryName(dbFullPath)!, "backups");

                if (!Directory.Exists(backupDir)) Directory.CreateDirectory(backupDir);

                string todayBackupPath = Path.Combine(backupDir, $"DrMusa_Backup_{DateTime.Now:yyyyMMdd}.db");
                if (File.Exists(dbFullPath) && !File.Exists(todayBackupPath))
                {
                    File.Copy(dbFullPath, todayBackupPath, true);
                    
                    // Cleanup backups older than 7 days
                    var oldBackups = Directory.GetFiles(backupDir, "DrMusa_Backup_*.db")
                        .Select(f => new FileInfo(f))
                        .Where(f => f.CreationTime < DateTime.Now.AddDays(-7));
                    foreach (var old in oldBackups) { try { old.Delete(); } catch { } }
                }
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Failed to perform automatic database backup.");
            }

            // Run migrations & seed
            using (var scope = ServiceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DrMusaDbContext>();
                DbSeeder.SeedAsync(db).GetAwaiter().GetResult();
            }

            // Launch Login form
            Application.Run(new Forms.Login.LoginForm(ServiceProvider));
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application failed to start.");
            MessageBox.Show($"Startup error: {ex.Message}", "DrMusa Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}