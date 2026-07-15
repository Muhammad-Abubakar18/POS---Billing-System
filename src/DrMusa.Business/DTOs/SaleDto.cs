using DrMusa.Common.Enums;

namespace DrMusa.Business.DTOs;

public record SaleItemDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent,
    decimal TotalPrice
);

public record CreateSaleItemDto(
    int ProductId,
    int Quantity,
    decimal UnitPrice,
    decimal DiscountPercent
);

public record CreateSaleDto(
    int? CustomerId,
    int UserId,
    IList<CreateSaleItemDto> Items,
    decimal DiscountAmount,
    decimal TaxPercent,
    decimal PaidAmount,
    PaymentMethod PaymentMethod,
    OrderType OrderType,
    string? Notes,
    bool HasReceipt
);

public record SaleDto(
    int Id,
    string InvoiceNumber,
    int? CustomerId,
    string? CustomerName,
    DateTime SaleDate,
    decimal SubTotal,
    decimal DiscountAmount,
    decimal TaxPercent,
    decimal TaxAmount,
    decimal TotalAmount,
    decimal PaidAmount,
    decimal ChangeAmount,
    PaymentMethod PaymentMethod,
    OrderType OrderType,
    SaleStatus Status,
    bool HasReceipt,
    IList<SaleItemDto> Items
);

public record TopSellingProductDto(
    int ProductId,
    string ProductName,
    int TotalQuantitySold,
    decimal TotalRevenue
);

public record SalesGraphDataDto(
    DateTime Date,
    decimal Amount
);

public record DashboardDto(
    decimal TodaySales,
    decimal MonthlySales,
    int TotalProducts,
    int TotalCustomers,
    int TotalSuppliers,
    int LowStockCount,
    IList<SaleDto> RecentTransactions,
    IList<ProductDto> LowStockProducts,
    IList<TopSellingProductDto> TopSellingProducts,
    IList<SalesGraphDataDto> SalesGraphData
);
