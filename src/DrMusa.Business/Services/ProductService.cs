using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Data.Models;
using DrMusa.Data.Repositories;

namespace DrMusa.Business.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;

    public ProductService(IProductRepository productRepo)
    {
        _productRepo = productRepo;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync()
    {
        var products = await _productRepo.GetAllAsync();
        return products.Select(ToDto);
    }

    public async Task<ProductDto?> GetByIdAsync(int id)
    {
        var p = await _productRepo.GetByIdAsync(id);
        return p is null ? null : ToDto(p);
    }

    public async Task<ProductDto?> GetByBarcodeAsync(string barcode)
    {
        var p = await _productRepo.GetByBarcodeAsync(barcode);
        return p is null ? null : ToDto(p);
    }

    public async Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm)
    {
        var results = await _productRepo.SearchAsync(searchTerm);
        return results.Select(ToDto);
    }


    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        var product = new Product
        {
            Name = dto.Name,
            Barcode = dto.Barcode,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            PurchasePrice = dto.PurchasePrice,
            SellingPrice = dto.SellingPrice,
            CurrentStock = 0,
            MinimumStock = 5,
            ImagePath = dto.ImagePath,
            IsActive = true,
            CreatedAt = DateTime.Now
        };
        var created = await _productRepo.AddAsync(product);
        return ToDto(created);
    }

    public async Task UpdateAsync(int id, CreateProductDto dto)
    {
        var product = await _productRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found.");

        product.Name = dto.Name;
        product.Barcode = dto.Barcode;
        product.Description = dto.Description;
        product.CategoryId = dto.CategoryId;
        product.PurchasePrice = dto.PurchasePrice;
        product.SellingPrice = dto.SellingPrice;
        product.ImagePath = dto.ImagePath;
        product.UpdatedAt = DateTime.Now;

        await _productRepo.UpdateAsync(product);
    }

    public async Task UpdateMinimumStockAsync(int id, int minStock)
    {
        var product = await _productRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found.");

        product.MinimumStock = minStock;
        product.UpdatedAt = DateTime.Now;

        await _productRepo.UpdateAsync(product);
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _productRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Product {id} not found.");
        product.IsActive = false; // Soft delete
        await _productRepo.UpdateAsync(product);
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Barcode, p.Description,
        p.CategoryId, p.Category?.Name,
        p.PurchasePrice, p.SellingPrice,
        p.CurrentStock, p.MinimumStock,
        p.ImagePath, p.IsActive
    );
}
