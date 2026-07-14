using DrMusa.Common.Enums;

namespace DrMusa.Data.Models;

public class Sale
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int? CustomerId { get; set; }
    public int? UserId { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.Now;
    public decimal SubTotal { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal ChangeAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public OrderType OrderType { get; set; } = OrderType.DineIn;
    public SaleStatus Status { get; set; } = SaleStatus.Completed;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public bool HasReceipt { get; set; } = false;

    // Navigation
    public Customer? Customer { get; set; }
    public User? User { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
