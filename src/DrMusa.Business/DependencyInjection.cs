using DrMusa.Business.Interfaces;
using DrMusa.Business.Services;
using DrMusa.Data.Context;
using DrMusa.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DrMusa.Business;

/// <summary>
/// Extension method to register all application services with the DI container.
/// Call this from Program.cs: services.AddDrMusaServices(connectionString);
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddDrMusaServices(
        this IServiceCollection services, string connectionString)
    {
        // Database
        services.AddDbContext<DrMusaDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();

        // Business Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<ISettingService, SettingService>();

        return services;
    }
}
