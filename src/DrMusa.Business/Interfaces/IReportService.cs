using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface IReportService
{
    Task<IEnumerable<SalesReportDto>> GetDailySalesReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ProfitReportDto>> GetDailyProfitReportAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync();
    Task<IEnumerable<InventoryReportDto>> GetLowStockReportAsync();
    Task<decimal> GetTotalSalesForPeriodAsync(DateTime startDate, DateTime endDate);
    Task<decimal> GetTotalProfitForPeriodAsync(DateTime startDate, DateTime endDate);

    // Dashboard Analytics Methods
    Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<ProductChartDto>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int count);
    Task<IEnumerable<ProductChartDto>> GetLowSellingProductsAsync(DateTime startDate, DateTime endDate, int count);
    Task<IEnumerable<ProfitTrendDto>> GetProfitTrendAsync(DateTime startDate, DateTime endDate, ReportGroupingType grouping);
}
