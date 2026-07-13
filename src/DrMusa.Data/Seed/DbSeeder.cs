using DrMusa.Common.Enums;
using DrMusa.Common.Utilities;
using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Seed;

/// <summary>Seeds the database with default admin user, sample categories and settings.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(DrMusaDbContext context)
    {
        await context.Database.MigrateAsync();

        // Seed default admin user
        if (!await context.Users.AnyAsync())
        {
            context.Users.Add(new User
            {
                Username = "admin",
                PasswordHash = PasswordHelper.HashPassword("admin123"),
                FullName = "System Administrator",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.Now
            });
        }

        // Seed default categories
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "General", Description = "General products" },
                new Category { Name = "Medicine", Description = "Pharmaceutical products" },
                new Category { Name = "Electronics", Description = "Electronic devices" }
            );
        }

        // Seed default settings
        if (!await context.Settings.AnyAsync())
        {
            context.Settings.AddRange(
                new Setting { Key = "BusinessName", Value = "DrMusa Store", Description = "Business name" },
                new Setting { Key = "BusinessPhone", Value = "", Description = "Contact phone" },
                new Setting { Key = "BusinessAddress", Value = "", Description = "Business address" },
                new Setting { Key = "Currency", Value = "PKR", Description = "Currency symbol" },
                new Setting { Key = "TaxPercent", Value = "0", Description = "Default tax %" },
                new Setting { Key = "ReceiptHeader", Value = "Thank you for shopping!", Description = "Receipt header" },
                new Setting { Key = "ReceiptFooter", Value = "Please come again", Description = "Receipt footer" }
            );
        }

        await context.SaveChangesAsync();
    }
}
