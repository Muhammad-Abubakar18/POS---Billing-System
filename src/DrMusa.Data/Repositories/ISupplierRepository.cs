using DrMusa.Data.Models;

namespace DrMusa.Data.Repositories;

public interface ISupplierRepository : IRepository<Supplier>
{
    Task<IEnumerable<Supplier>> GetSuppliersWithPurchasesAsync();
    Task<Supplier?> GetSupplierWithPurchasesByIdAsync(int id);
}
