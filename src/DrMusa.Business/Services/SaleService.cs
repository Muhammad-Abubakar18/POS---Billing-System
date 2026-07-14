using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Enums;
using DrMusa.Common.Utilities;
using DrMusa.Data.Context;
using DrMusa.Data.Models;
using DrMusa.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Business.Services;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepo;
    private readonly IProductRepository _productRepo;
    private readonly DrMusaDbContext _context;

    public SaleService(ISaleRepository saleRepo, IProductRepository productRepo, DrMusaDbContext context)
    {
        _saleRepo = saleRepo;
        _productRepo = productRepo;
        _context = context;
    }

    public async Task<SaleDto> CreateSaleAsync(CreateSaleDto dto)
    {
        // Calculate totals
        decimal subTotal = 0m;
        var saleItems = new List<SaleItem>();

        foreach (var item in dto.Items)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId)
                ?? throw new KeyNotFoundException($"Product {item.ProductId} not found.");

            if (product.CurrentStock < item.Quantity)
                throw new InvalidOperationException($"Insufficient stock for '{product.Name}'.");

            var lineTotal = item.UnitPrice * item.Quantity;
            lineTotal -= lineTotal * item.DiscountPercent / 100;
            subTotal += lineTotal;

            saleItems.Add(new SaleItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                TotalPrice = lineTotal
            });

            // Deduct stock
            product.CurrentStock -= item.Quantity;
            await _productRepo.UpdateAsync(product);

            // Record inventory movement
            _context.Inventories.Add(new Inventory
            {
                ProductId = item.ProductId,
                MovementType = StockMovementType.StockOut,
                Quantity = item.Quantity,
                StockBefore = product.CurrentStock + item.Quantity,
                StockAfter = product.CurrentStock,
                Reference = "SALE",
                UserId = dto.UserId,
                CreatedAt = DateTime.Now
            });
        }

        var discountAmt = subTotal * dto.DiscountPercent / 100;
        var taxAmt = (subTotal - discountAmt) * dto.TaxPercent / 100;
        var total = subTotal - discountAmt + taxAmt;
        var change = dto.PaidAmount - total;

        var sale = new Sale
        {
            InvoiceNumber = InvoiceNumberGenerator.Generate(),
            CustomerId = dto.CustomerId,
            UserId = dto.UserId,
            SaleDate = DateTime.Now,
            SubTotal = subTotal,
            DiscountPercent = dto.DiscountPercent,
            DiscountAmount = discountAmt,
            TaxPercent = dto.TaxPercent,
            TaxAmount = taxAmt,
            TotalAmount = total,
            PaidAmount = dto.PaidAmount,
            ChangeAmount = change,
            PaymentMethod = dto.PaymentMethod,
            OrderType = dto.OrderType,
            Status = SaleStatus.Completed,
            Notes = dto.Notes,
            HasReceipt = dto.HasReceipt,
            SaleItems = saleItems
        };

        await _saleRepo.AddAsync(sale);
        await _context.SaveChangesAsync();

        return ToDto(sale);
    }

    public async Task<SaleDto?> GetByIdAsync(int id)
    {
        var s = await _saleRepo.GetWithItemsAsync(id);
        return s is null ? null : ToDto(s);
    }

    public async Task<SaleDto?> GetByInvoiceNumberAsync(string invoiceNumber)
    {
        var s = await _saleRepo.GetByInvoiceNumberAsync(invoiceNumber);
        return s is null ? null : ToDto(s);
    }

    public async Task<IEnumerable<SaleDto>> GetByDateRangeAsync(DateTime from, DateTime to)
    {
        var sales = await _saleRepo.GetByDateRangeAsync(from, to);
        return sales.Select(ToDto);
    }

    public async Task<bool> CancelSaleAsync(int saleId)
    {
        var sale = await _saleRepo.GetWithItemsAsync(saleId);
        if (sale == null || sale.Status == SaleStatus.Cancelled) return false;

        sale.Status = SaleStatus.Cancelled;

        // Restore stock
        foreach (var item in sale.SaleItems)
        {
            var product = await _productRepo.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                product.CurrentStock += item.Quantity;
                await _productRepo.UpdateAsync(product);
            }
        }
        await _saleRepo.UpdateAsync(sale);
        return true;
    }

    public async Task<DashboardDto> GetDashboardDataAsync()
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);

        var todaySales = await _saleRepo.GetTotalSalesAsync(today, today.AddDays(1));
        var monthlySales = await _saleRepo.GetTotalSalesAsync(monthStart, today.AddDays(1));
        var totalProducts = await _context.Products.CountAsync(p => p.IsActive);
        var totalCustomers = await _context.Customers.CountAsync(c => c.IsActive);
        var totalSuppliers = await _context.Suppliers.CountAsync(s => s.IsActive);
        var lowStockProducts = (await _productRepo.GetLowStockProductsAsync()).ToList();
        var recent = await _saleRepo.GetByDateRangeAsync(today.AddDays(-7), today.AddDays(1));

        var topSellingRaw = await _saleRepo.GetTopSellingProductsAsync(5);
        var topSelling = topSellingRaw.Select(t => new TopSellingProductDto(
            t.ProductId,
            t.ProductName,
            t.TotalQuantitySold,
            t.TotalRevenue
        )).ToList();

        var graphDataRaw = await _saleRepo.GetSalesGraphDataAsync(today.AddDays(-6), today.AddDays(1));
        var graphData = graphDataRaw.Select(g => new SalesGraphDataDto(
            g.Date,
            g.Amount
        )).ToList();

        return new DashboardDto(
            todaySales, monthlySales,
            totalProducts, totalCustomers, totalSuppliers,
            lowStockProducts.Count,
            recent.Take(10).Select(ToDto).ToList(),
            lowStockProducts.Select(p => new ProductDto(
                p.Id, p.Name, p.Barcode, p.Description,
                p.CategoryId, p.Category?.Name,
                p.PurchasePrice, p.SellingPrice,
                p.CurrentStock, p.MinimumStock,
                p.ImagePath, p.IsActive)).ToList(),
            topSelling,
            graphData
        );
    }

    private static SaleDto ToDto(Sale s) => new(
        s.Id, s.InvoiceNumber,
        s.CustomerId, s.Customer?.Name,
        s.SaleDate, s.SubTotal,
        s.DiscountAmount, s.TaxAmount,
        s.TotalAmount, s.PaidAmount, s.ChangeAmount,
        s.PaymentMethod, s.OrderType, s.Status, s.HasReceipt,
        s.SaleItems.Select(si => new SaleItemDto(
            si.ProductId, si.Product?.Name ?? string.Empty,
            si.Quantity, si.UnitPrice,
            si.DiscountPercent, si.TotalPrice
        )).ToList()
    );
}
