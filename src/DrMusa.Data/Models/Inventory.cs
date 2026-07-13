using DrMusa.Common.Enums;

namespace DrMusa.Data.Models;

public class Inventory
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int Quantity { get; set; }
    public int StockBefore { get; set; }
    public int StockAfter { get; set; }
    public string? Reference { get; set; }
    public string? Notes { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Navigation
    public Product? Product { get; set; }
    public User? User { get; set; }
}
