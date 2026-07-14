using DrMusa.Data.Models;

namespace DrMusa.Data.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> IsUsernameUniqueAsync(string username, int? excludeId = null);
}
