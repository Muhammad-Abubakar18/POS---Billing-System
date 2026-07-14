using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Data.Repositories;

namespace DrMusa.Business.Services;

public class ReportService : IReportService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;

    public ReportService(ISaleRepository saleRepository, IProductRepository productRepository)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<SalesReportDto>> GetDailySalesReportAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));

        var dailyData = sales
            .Where(s => s.Status == Common.Enums.SaleStatus.Completed)
            .GroupBy(s => s.SaleDate.Date)
            .Select(g => new SalesReportDto
            {
                Date = g.Key,
                TotalInvoices = g.Count(),
                TotalSales = g.Sum(s => s.TotalAmount + s.DiscountAmount),
                TotalDiscount = g.Sum(s => s.DiscountAmount),
                TotalTax = g.Sum(s => s.TaxAmount),
                NetAmount = g.Sum(s => s.TotalAmount)
            })
            .OrderBy(r => r.Date)
            .ToList();

        return dailyData;
    }

    public async Task<IEnumerable<ProfitReportDto>> GetDailyProfitReportAsync(DateTime startDate, DateTime endDate)
    {
        // Ideally, Profit involves Cost from SaleItems. For simplicity, we assume we fetch SaleItems 
        // by modifying the query or iterating. Let's get all sales and then fetch items.
        // But ISaleRepository doesn't fetch items in GetByDateRangeAsync.
        // For accurate profit we would need a specific repository method or use the Context directly.
        // As a workaround, we use the total sales amount vs a simple assumed margin or if we have it in Sale.
        // Wait, SaleRepository.GetByDateRangeAsync does not include SaleItems.
        // We will fetch all products to calculate the current profit based on Product.PurchasePrice.
        var sales = await _saleRepository.GetByDateRangeAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));
        var completedSales = sales.Where(s => s.Status == Common.Enums.SaleStatus.Completed).ToList();
        
        var dailyData = new List<ProfitReportDto>();

        foreach (var group in completedSales.GroupBy(s => s.SaleDate.Date))
        {
            decimal revenue = 0;
            decimal cost = 0;
            int itemsSold = 0;

            foreach (var sale in group)
            {
                var fullSale = await _saleRepository.GetWithItemsAsync(sale.Id);
                if (fullSale != null && fullSale.SaleItems != null)
                {
                    revenue += fullSale.TotalAmount;
                    foreach (var item in fullSale.SaleItems)
                    {
                        itemsSold += item.Quantity;
                        if (item.Product != null)
                        {
                            cost += item.Quantity * item.Product.PurchasePrice;
                        }
                    }
                }
            }

            dailyData.Add(new ProfitReportDto
            {
                Date = group.Key,
                ItemsSold = itemsSold,
                Revenue = revenue,
                Cost = cost
            });
        }

        return dailyData.OrderBy(r => r.Date).ToList();
    }

    public async Task<IEnumerable<InventoryReportDto>> GetInventoryReportAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return products.Select(p => new InventoryReportDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            CategoryName = p.Category?.Name ?? "Uncategorized", // Assuming Category is loaded or we handle it
            CurrentStock = p.CurrentStock,
            MinimumStock = p.MinimumStock,
            PurchasePrice = p.PurchasePrice
        }).OrderBy(r => r.ProductName).ToList();
    }

    public async Task<IEnumerable<InventoryReportDto>> GetLowStockReportAsync()
    {
        var lowStock = await _productRepository.GetLowStockProductsAsync();

        return lowStock.Select(p => new InventoryReportDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            CategoryName = p.Category?.Name ?? "Uncategorized",
            CurrentStock = p.CurrentStock,
            MinimumStock = p.MinimumStock,
            PurchasePrice = p.PurchasePrice
        }).OrderBy(r => r.CurrentStock).ToList();
    }

    public async Task<decimal> GetTotalSalesForPeriodAsync(DateTime startDate, DateTime endDate)
    {
        return await _saleRepository.GetTotalSalesAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));
    }

    public async Task<decimal> GetTotalProfitForPeriodAsync(DateTime startDate, DateTime endDate)
    {
        var profitReports = await GetDailyProfitReportAsync(startDate, endDate);
        return profitReports.Sum(p => p.Profit);
    }
}
