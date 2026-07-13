using DrMusa.Business.DTOs;
using DrMusa.Data.Models;

namespace DrMusa.Business.Mappers;

/// <summary>Manual mapper helpers — replace with AutoMapper if needed in the future.</summary>
public static class ProductMapper
{
    public static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Barcode, p.Description,
        p.CategoryId, p.Category?.Name,
        p.PurchasePrice, p.SellingPrice,
        p.CurrentStock, p.MinimumStock,
        p.ImagePath, p.IsActive
    );

    public static Product ToEntity(CreateProductDto dto) => new()
    {
        Name = dto.Name,
        Barcode = dto.Barcode,
        Description = dto.Description,
        CategoryId = dto.CategoryId,
        PurchasePrice = dto.PurchasePrice,
        SellingPrice = dto.SellingPrice,
        CurrentStock = dto.CurrentStock,
        MinimumStock = dto.MinimumStock,
        ImagePath = dto.ImagePath,
        IsActive = true,
        CreatedAt = DateTime.Now
    };

    public static void UpdateEntity(Product entity, CreateProductDto dto)
    {
        entity.Name = dto.Name;
        entity.Barcode = dto.Barcode;
        entity.Description = dto.Description;
        entity.CategoryId = dto.CategoryId;
        entity.PurchasePrice = dto.PurchasePrice;
        entity.SellingPrice = dto.SellingPrice;
        entity.CurrentStock = dto.CurrentStock;
        entity.MinimumStock = dto.MinimumStock;
        entity.ImagePath = dto.ImagePath;
        entity.UpdatedAt = DateTime.Now;
    }
}
