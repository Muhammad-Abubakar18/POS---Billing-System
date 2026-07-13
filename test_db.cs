using System;
using Microsoft.EntityFrameworkCore;
using DrMusa.Data.Context;
using DrMusa.Data.Models;

class Program {
    static void Main() {
        var options = new DbContextOptionsBuilder<DrMusaDbContext>()
            .UseSqlite("Data Source=../database/DrMusa.db")
            .Options;
        using var db = new DrMusaDbContext(options);
        var product = new Product {
            Name = "injected burger",
            Description = "white suace injected burger",
            CategoryId = 1,
            PurchasePrice = 200,
            SellingPrice = 500,
            CurrentStock = 380,
            MinimumStock = 50,
            Barcode = null
        };
        try {
            db.Products.Add(product);
            db.SaveChanges();
            Console.WriteLine("Success!");
        } catch (Exception ex) {
            Console.WriteLine("Error: " + ex.Message);
            if (ex.InnerException != null) Console.WriteLine("Inner: " + ex.InnerException.Message);
        }
    }
}
