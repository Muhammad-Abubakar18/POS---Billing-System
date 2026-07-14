namespace DrMusa.Business.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string? Barcode,
    string? Description,
    int CategoryId,
    string? CategoryName,
    decimal PurchasePrice,
    decimal SellingPrice,
    int CurrentStock,
    int MinimumStock,
    string? ImagePath,
    bool IsActive
);

public record CreateProductDto(
    string Name,
    string? Barcode,
    string? Description,
    int CategoryId,
    decimal PurchasePrice,
    decimal SellingPrice,
    string? ImagePath
);
