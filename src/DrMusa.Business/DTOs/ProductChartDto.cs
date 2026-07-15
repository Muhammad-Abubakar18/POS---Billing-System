namespace DrMusa.Business.DTOs;

public record ProductChartDto(
    string ProductName,
    int TotalQuantitySold,
    decimal TotalRevenue
);
