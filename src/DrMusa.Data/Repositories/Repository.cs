using DrMusa.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Repositories;

/// <summary>Generic EF Core repository implementation.</summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly DrMusaDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DrMusaDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Entity with id {id} not found.");
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task<bool> ExistsAsync(int id) => await _dbSet.FindAsync(id) != null;
}
