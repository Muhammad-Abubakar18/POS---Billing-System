using DrMusa.Data.Models;

namespace DrMusa.Data.Repositories;

/// <summary>Repository interface for Category-specific queries.</summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> SearchAsync(string searchTerm);
    Task<bool> HasActiveProductsAsync(int categoryId);
    Task<IEnumerable<Category>> GetAllWithProductCountAsync();
}
