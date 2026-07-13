using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface IInventoryService
{
    Task StockInAsync(int productId, int quantity, string? notes, int userId);
    Task StockOutAsync(int productId, int quantity, string? notes, int userId);
    Task AdjustStockAsync(int productId, int newStock, string? notes, int userId);
    Task<IEnumerable<InventoryDto>> GetHistoryAsync(int productId);
    Task<IEnumerable<ProductDto>> GetLowStockAsync();
}
