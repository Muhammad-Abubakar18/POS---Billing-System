using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Repositories;

/// <summary>EF Core implementation of ICategoryRepository.</summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(DrMusaDbContext context) : base(context) { }

    public async Task<IEnumerable<Category>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower().Trim();
        return await _dbSet
            .Where(c => c.IsActive &&
                        (c.Name.ToLower().Contains(term) ||
                         (c.Description != null && c.Description.ToLower().Contains(term))))
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<bool> HasActiveProductsAsync(int categoryId)
    {
        return await _context.Products
            .AnyAsync(p => p.CategoryId == categoryId && p.IsActive);
    }

    public async Task<IEnumerable<Category>> GetAllWithProductCountAsync()
    {
        return await _dbSet
            .Include(c => c.Products)
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
