using DrMusa.Data.Models;

namespace DrMusa.Data.Repositories;

public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetWithItemsAsync(int saleId);
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<IEnumerable<Sale>> GetByCustomerAsync(int customerId);
    Task<Sale?> GetByInvoiceNumberAsync(string invoiceNumber);
    Task<decimal> GetTotalSalesAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetTopSellingProductsAsync(int count);
    Task<IEnumerable<dynamic>> GetSalesGraphDataAsync(DateTime from, DateTime to);
}
