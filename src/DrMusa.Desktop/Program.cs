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
            var connectionString = $"Data Source={Path.GetFullPath(dbPath)}";

            services.AddDrMusaServices(connectionString);

            ServiceProvider = services.BuildServiceProvider();

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