using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Repositories;

public class SupplierRepository : Repository<Supplier>, ISupplierRepository
{
    public SupplierRepository(DrMusaDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Supplier>> GetSuppliersWithPurchasesAsync()
    {
        return await _context.Suppliers
            .Include(s => s.Purchases)
            .ToListAsync();
    }

    public async Task<Supplier?> GetSupplierWithPurchasesByIdAsync(int id)
    {
        return await _context.Suppliers
            .Include(s => s.Purchases)
            .FirstOrDefaultAsync(s => s.Id == id);
    }
}
