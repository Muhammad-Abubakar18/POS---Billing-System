using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Utilities;
using DrMusa.Data.Context;
using DrMusa.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace DrMusa.Business.Services;

public class AuthService : IAuthService
{
    private readonly DrMusaDbContext _context;

    public AuthService(DrMusaDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto?> LoginAsync(string username, string password)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

        if (user == null) return null;
        if (!PasswordHelper.VerifyPassword(password, user.PasswordHash)) return null;

        user.LastLoginAt = DateTime.Now;

        _context.UserLogs.Add(new UserLog
        {
            UserId = user.Id,
            Action = "LOGIN",
            Details = $"User '{username}' logged in successfully.",
            CreatedAt = DateTime.Now
        });

        await _context.SaveChangesAsync();

        return new UserDto(user.Id, user.Username, user.FullName,
                           user.Email, user.Phone, user.Role, user.IsActive, user.LastLoginAt);
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;
        if (!PasswordHelper.VerifyPassword(currentPassword, user.PasswordHash)) return false;

        user.PasswordHash = PasswordHelper.HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task LogoutAsync(int userId)
    {
        _context.UserLogs.Add(new UserLog
        {
            UserId = userId,
            Action = "LOGOUT",
            Details = "User logged out.",
            CreatedAt = DateTime.Now
        });
        await _context.SaveChangesAsync();
    }

    public async Task<UserDto?> GetUserByIdAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null || !user.IsActive) return null;
        return new UserDto(user.Id, user.Username, user.FullName,
                           user.Email, user.Phone, user.Role, user.IsActive, user.LastLoginAt);
    }
}
