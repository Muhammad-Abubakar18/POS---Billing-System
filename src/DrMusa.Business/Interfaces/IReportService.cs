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
}
