using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface ISaleService
{
    Task<SaleDto> CreateSaleAsync(CreateSaleDto dto);
    Task<SaleDto?> GetByIdAsync(int id);
    Task<SaleDto?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<bool> CancelSaleAsync(int saleId);
    Task<DashboardDto> GetDashboardDataAsync();
}
