using DrMusa.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DrMusa.Data;

/// <summary>
/// Design-time factory used exclusively by EF Core CLI tools (dotnet ef migrations).
/// Not used at runtime — the DI container handles the real DbContext.
/// </summary>
public class DrMusaDbContextFactory : IDesignTimeDbContextFactory<DrMusaDbContext>
{
    public DrMusaDbContext CreateDbContext(string[] args)
    {
        var dbPath = Path.GetFullPath(Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "..", "..", "..", "..", "..", "database", "DrMusa.db"));

        var optionsBuilder = new DbContextOptionsBuilder<DrMusaDbContext>();
        optionsBuilder.UseSqlite($"Data Source={dbPath}");

        return new DrMusaDbContext(optionsBuilder.Options);
    }
}
