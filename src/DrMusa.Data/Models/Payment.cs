using DrMusa.Common.Enums;

namespace DrMusa.Data.Models;

public class Payment
{
    public int Id { get; set; }
    public int SaleId { get; set; }
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; } = DateTime.Now;
    public string? Reference { get; set; }

    // Navigation
    public Sale? Sale { get; set; }
}
