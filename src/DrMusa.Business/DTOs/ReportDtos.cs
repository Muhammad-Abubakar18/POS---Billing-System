namespace DrMusa.Business.DTOs;

public class SalesReportDto
{
    public DateTime Date { get; set; }
    public int TotalInvoices { get; set; }
    public decimal TotalSales { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalTax { get; set; }
    public decimal NetAmount { get; set; }
}

public class ProfitReportDto
{
    public DateTime Date { get; set; }
    public int ItemsSold { get; set; }
    public decimal Revenue { get; set; }
    public decimal Cost { get; set; }
    public decimal Profit => Revenue - Cost;
    public decimal ProfitMargin => Revenue > 0 ? (Profit / Revenue) * 100 : 0;
}

public class InventoryReportDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal TotalValue => CurrentStock > 0 ? CurrentStock * PurchasePrice : 0;
    public string Status => CurrentStock <= MinimumStock ? "Low Stock" : "In Stock";
}
