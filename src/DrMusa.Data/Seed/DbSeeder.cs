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

        // Seed default fast food categories
        if (!await context.Categories.AnyAsync())
        {
            context.Categories.AddRange(
                new Category { Name = "Burgers", Description = "Beef, chicken, and specialty burgers" },
                new Category { Name = "Pizzas", Description = "Classic and specialty pizzas" },
                new Category { Name = "Chinese", Description = "Chinese cuisine and rice dishes" },
                new Category { Name = "Shawarma", Description = "Shawarma wraps and platters" },
                new Category { Name = "Drinks", Description = "Beverages, juices, and soft drinks" },
                new Category { Name = "Desserts", Description = "Sweet treats and ice cream" },
                new Category { Name = "Sides", Description = "Fries, nuggets, and appetizers" }
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
