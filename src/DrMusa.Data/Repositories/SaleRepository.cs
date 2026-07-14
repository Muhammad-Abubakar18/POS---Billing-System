using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Repositories;

public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(DrMusaDbContext context) : base(context) { }

    public async Task<Sale?> GetWithItemsAsync(int saleId)
        => await _dbSet.Include(s => s.SaleItems)
                        .ThenInclude(si => si.Product)
                       .Include(s => s.Customer)
                       .Include(s => s.Payments)
                       .FirstOrDefaultAsync(s => s.Id == saleId);

    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime from, DateTime to)
        => await _dbSet.Where(s => s.SaleDate >= from && s.SaleDate <= to)
                       .OrderByDescending(s => s.SaleDate)
                       .ToListAsync();

    public async Task<IEnumerable<Sale>> GetByCustomerAsync(int customerId)
        => await _dbSet.Where(s => s.CustomerId == customerId)
                       .OrderByDescending(s => s.SaleDate)
                       .ToListAsync();

    public async Task<Sale?> GetByInvoiceNumberAsync(string invoiceNumber)
        => await _dbSet.Include(s => s.SaleItems)
                        .ThenInclude(si => si.Product)
                       .FirstOrDefaultAsync(s => s.InvoiceNumber == invoiceNumber);

    public async Task<decimal> GetTotalSalesAsync(DateTime from, DateTime to)
    {
        // SQLite cannot apply SUM() on decimal columns in SQL — aggregate on client side
        var amounts = await _dbSet
            .Where(s => s.SaleDate >= from && s.SaleDate <= to &&
                        s.Status == Common.Enums.SaleStatus.Completed)
            .Select(s => s.TotalAmount)
            .ToListAsync();

        return amounts.Sum();
    }

    public async Task<IEnumerable<(int ProductId, string ProductName, int TotalQuantitySold, decimal TotalRevenue)>> GetTopSellingProductsAsync(int count)
    {
        var saleItems = await _context.Set<SaleItem>()
            .Include(si => si.Product)
            .ToListAsync();

        var topSelling = saleItems
            .GroupBy(si => new { si.ProductId, Name = si.Product!.Name })
            .Select(g => (
                ProductId: g.Key.ProductId,
                ProductName: g.Key.Name,
                TotalQuantitySold: g.Sum(si => si.Quantity),
                TotalRevenue: g.Sum(si => si.TotalPrice)
            ))
            .OrderByDescending(x => x.TotalQuantitySold)
            .Take(count)
            .ToList();

        return topSelling;
    }

    public async Task<IEnumerable<(DateTime Date, decimal Amount)>> GetSalesGraphDataAsync(DateTime from, DateTime to)
    {
        var sales = await _dbSet
            .Where(s => s.SaleDate >= from && s.SaleDate <= to && s.Status == Common.Enums.SaleStatus.Completed)
            .Select(s => new { s.SaleDate, s.TotalAmount })
            .ToListAsync();

        var grouped = sales
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => (
                Date: g.Key,
                Amount: g.Sum(s => s.TotalAmount)
            ))
            .OrderBy(g => g.Date)
            .ToList();

        return grouped;
    }
}
