using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(DrMusaDbContext context) : base(context) { }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.Username.ToLower() == username.ToLower() && u.IsActive);
    }

    public async Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null)
    {
        var query = _dbSet.Where(u => u.Username.ToLower() == username.ToLower());
        if (excludeId.HasValue)
            query = query.Where(u => u.Id != excludeId.Value);

        return !await query.AnyAsync();
    }
}
