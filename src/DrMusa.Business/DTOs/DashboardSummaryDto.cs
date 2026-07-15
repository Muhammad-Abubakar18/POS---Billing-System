namespace DrMusa.Business.DTOs;

public record DashboardSummaryDto(
    decimal GrandTotal,
    decimal RawProfit,
    decimal TotalTax,
    decimal TotalDiscount
);
