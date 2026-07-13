using DrMusa.Common.Enums;

namespace DrMusa.Business.DTOs;

public record InventoryDto(
    int Id,
    int ProductId,
    string ProductName,
    StockMovementType MovementType,
    int Quantity,
    int StockBefore,
    int StockAfter,
    string? Notes,
    DateTime CreatedAt
);

public record SettingDto(string Key, string? Value, string? Description);
