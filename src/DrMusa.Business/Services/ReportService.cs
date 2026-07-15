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
                    // Revenue to the business is SubTotal - Discount. Tax is not business revenue.
                    // For the "Profit" report, we should use Net Revenue.
                    revenue += (fullSale.SubTotal - fullSale.DiscountAmount);
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

    public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(DateTime startDate, DateTime endDate)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));
        var completedSales = sales.Where(s => s.Status == Common.Enums.SaleStatus.Completed).ToList();
        
        decimal totalRevenue = 0;
        decimal totalTax = completedSales.Sum(s => s.TaxAmount);
        decimal totalDiscount = completedSales.Sum(s => s.DiscountAmount);
        decimal costOfGoods = 0;

        foreach (var sale in completedSales)
        {
            var fullSale = await _saleRepository.GetWithItemsAsync(sale.Id);
            if (fullSale != null && fullSale.SaleItems != null)
            {
                totalRevenue += fullSale.TotalAmount; // Does this include tax? Wait, TotalAmount = subTotal - discount + tax.
                // Raw profit should be (SubTotal - Discount) - Cost, or (TotalRevenue - Tax) - Cost.
                foreach (var item in fullSale.SaleItems)
                {
                    if (item.Product != null)
                    {
                        costOfGoods += item.Quantity * item.Product.PurchasePrice;
                    }
                }
            }
        }

        // According to user: "profit which show raw profit without tax and discount"
        // Raw Profit = (Total SubTotal) - Cost of Goods
        // Let's re-calculate SubTotal from the DB
        decimal totalSubTotal = completedSales.Sum(s => s.SubTotal);
        decimal rawProfit = totalSubTotal - costOfGoods;
        
        // "grand profit which include total grand total"
        // Assuming they mean Grand Total Revenue
        decimal grandTotal = completedSales.Sum(s => s.TotalAmount);

        return new DashboardSummaryDto(grandTotal, rawProfit, totalTax, totalDiscount);
    }

    public async Task<IEnumerable<ProductChartDto>> GetTopSellingProductsAsync(DateTime startDate, DateTime endDate, int count)
    {
        return await GetProductSalesRankingAsync(startDate, endDate, count, descending: true);
    }

    public async Task<IEnumerable<ProductChartDto>> GetLowSellingProductsAsync(DateTime startDate, DateTime endDate, int count)
    {
        return await GetProductSalesRankingAsync(startDate, endDate, count, descending: false);
    }
    
    private async Task<IEnumerable<ProductChartDto>> GetProductSalesRankingAsync(DateTime startDate, DateTime endDate, int count, bool descending)
    {
        var sales = await _saleRepository.GetByDateRangeAsync(startDate.Date, endDate.Date.AddDays(1).AddTicks(-1));
        var completedSales = sales.Where(s => s.Status == Common.Enums.SaleStatus.Completed).ToList();
        
        var productStats = new Dictionary<string, (int Qty, decimal Rev)>();
        
        foreach (var sale in completedSales)
        {
            var fullSale = await _saleRepository.GetWithItemsAsync(sale.Id);
            if (fullSale != null && fullSale.SaleItems != null)
            {
                foreach (var item in fullSale.SaleItems)
                {
                    var pName = item.Product?.Name ?? "Unknown";
                    if (!productStats.ContainsKey(pName))
                        productStats[pName] = (0, 0);
                        
                    var current = productStats[pName];
                    productStats[pName] = (current.Qty + item.Quantity, current.Rev + item.TotalPrice);
                }
            }
        }

        var query = productStats.Select(kvp => new ProductChartDto(kvp.Key, kvp.Value.Qty, kvp.Value.Rev));
        
        if (descending)
            query = query.OrderByDescending(x => x.TotalQuantitySold);
        else
            query = query.OrderBy(x => x.TotalQuantitySold);
            
        return query.Take(count).ToList();
    }

    public async Task<IEnumerable<ProfitTrendDto>> GetProfitTrendAsync(DateTime startDate, DateTime endDate, ReportGroupingType grouping)
    {
        var dailyProfit = await GetDailyProfitReportAsync(startDate, endDate);
        
        if (grouping == ReportGroupingType.Daily)
        {
            return dailyProfit.Select(d => new ProfitTrendDto(d.Date.ToString("MMM dd"), d.Profit)).ToList();
        }
        else if (grouping == ReportGroupingType.Weekly)
        {
            // Simple grouping by year and week
            var cal = System.Globalization.DateTimeFormatInfo.CurrentInfo.Calendar;
            return dailyProfit
                .GroupBy(d => new { Year = d.Date.Year, Week = cal.GetWeekOfYear(d.Date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday) })
                .Select(g => new ProfitTrendDto($"W{g.Key.Week} {g.Key.Year}", g.Sum(x => x.Profit)))
                .ToList();
        }
        else // Monthly
        {
            return dailyProfit
                .GroupBy(d => new { d.Date.Year, d.Date.Month })
                .Select(g => new ProfitTrendDto(new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM yyyy"), g.Sum(x => x.Profit)))
                .ToList();
        }
    }
}
