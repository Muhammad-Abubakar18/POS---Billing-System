using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(DrMusaDbContext context) : base(context) { }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
        => await _dbSet.Include(p => p.Category)
                       .FirstOrDefaultAsync(p => p.Barcode == barcode && p.IsActive);

    public override async Task<Product?> GetByIdAsync(int id)
        => await _dbSet.Include(p => p.Category)
                       .FirstOrDefaultAsync(p => p.Id == id && p.IsActive);

    public override async Task<IEnumerable<Product>> GetAllAsync()
        => await _dbSet.Include(p => p.Category)
                       .Where(p => p.IsActive)
                       .OrderBy(p => p.Name)
                       .ToListAsync();

    public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        => await _dbSet.Where(p => p.CategoryId == categoryId && p.IsActive).ToListAsync();

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
        => await _dbSet.Where(p => p.CurrentStock <= p.MinimumStock && p.IsActive).ToListAsync();

    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
        => await _dbSet.Include(p => p.Category)
                       .Where(p => p.IsActive &&
                                   (p.Name.Contains(searchTerm) ||
                                    (p.Barcode != null && p.Barcode.Contains(searchTerm))))
                       .ToListAsync();
}
