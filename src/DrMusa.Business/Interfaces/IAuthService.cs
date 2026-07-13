using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface IAuthService
{
    Task<UserDto?> LoginAsync(string username, string password);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task LogoutAsync(int userId);
    Task<UserDto?> GetUserByIdAsync(int userId);
}
