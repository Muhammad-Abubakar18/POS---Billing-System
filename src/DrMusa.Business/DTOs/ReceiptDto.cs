using System;
using System.Collections.Generic;

namespace DrMusa.Business.DTOs;

public record ReceiptDto
{
    public string RestaurantName { get; init; } = "DrMusa POS";
    public string RestaurantAddress { get; init; } = "123 Main Street";
    public string RestaurantPhone { get; init; } = "555-0199";
    public string? TaxNumber { get; init; }
    
    public string InvoiceNumber { get; init; } = string.Empty;
    public DateTime Date { get; init; }
    public string Cashier { get; init; } = string.Empty;
    public string OrderType { get; init; } = string.Empty;
    
    public IList<ReceiptItemDto> Items { get; init; } = new List<ReceiptItemDto>();
    
    public decimal SubTotal { get; init; }
    public decimal Discount { get; init; }
    public decimal Tax { get; init; }
    public decimal GrandTotal { get; init; }
    public decimal PaidAmount { get; init; }
    public decimal Change { get; init; }
    
    public string FooterMessage { get; init; } = "Thank You Visit Again";
}

public record ReceiptItemDto
{
    public int Quantity { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public decimal Total { get; init; }
}
