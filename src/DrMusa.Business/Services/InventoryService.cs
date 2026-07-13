using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Enums;
using DrMusa.Data.Context;
using DrMusa.Data.Models;
using DrMusa.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Business.Services;

public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepo;
    private readonly DrMusaDbContext _context;

    public InventoryService(IProductRepository productRepo, DrMusaDbContext context)
    {
        _productRepo = productRepo;
        _context = context;
    }

    public async Task StockInAsync(int productId, int quantity, string? notes, int userId)
    {
        var product = await _productRepo.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        var before = product.CurrentStock;
        product.CurrentStock += quantity;
        await _productRepo.UpdateAsync(product);

        _context.Inventories.Add(new Inventory
        {
            ProductId = productId,
            MovementType = StockMovementType.StockIn,
            Quantity = quantity,
            StockBefore = before,
            StockAfter = product.CurrentStock,
            Notes = notes,
            UserId = userId,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task StockOutAsync(int productId, int quantity, string? notes, int userId)
    {
        var product = await _productRepo.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        if (product.CurrentStock < quantity)
            throw new InvalidOperationException("Insufficient stock.");

        var before = product.CurrentStock;
        product.CurrentStock -= quantity;
        await _productRepo.UpdateAsync(product);

        _context.Inventories.Add(new Inventory
        {
            ProductId = productId,
            MovementType = StockMovementType.StockOut,
            Quantity = quantity,
            StockBefore = before,
            StockAfter = product.CurrentStock,
            Notes = notes,
            UserId = userId,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task AdjustStockAsync(int productId, int newStock, string? notes, int userId)
    {
        var product = await _productRepo.GetByIdAsync(productId)
            ?? throw new KeyNotFoundException($"Product {productId} not found.");

        var before = product.CurrentStock;
        product.CurrentStock = newStock;
        await _productRepo.UpdateAsync(product);

        _context.Inventories.Add(new Inventory
        {
            ProductId = productId,
            MovementType = StockMovementType.Adjustment,
            Quantity = Math.Abs(newStock - before),
            StockBefore = before,
            StockAfter = newStock,
            Notes = notes,
            UserId = userId,
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryDto>> GetHistoryAsync(int productId)
    {
        return await _context.Inventories
            .Include(i => i.Product)
            .Where(i => i.ProductId == productId)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InventoryDto(
                i.Id, i.ProductId, i.Product!.Name,
                i.MovementType, i.Quantity,
                i.StockBefore, i.StockAfter,
                i.Notes, i.CreatedAt))
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockAsync()
    {
        var products = await _productRepo.GetLowStockProductsAsync();
        return products.Select(p => new ProductDto(
            p.Id, p.Name, p.Barcode, p.Description,
            p.CategoryId, p.Category?.Name,
            p.PurchasePrice, p.SellingPrice,
            p.CurrentStock, p.MinimumStock,
            p.ImagePath, p.IsActive));
    }
}
