using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Common.Utilities;
using DrMusa.Data.Models;
using DrMusa.Data.Repositories;

namespace DrMusa.Business.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _userRepo.GetAllAsync();
        return users.Select(ToDto).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await _userRepo.GetByIdAsync(id);
        return user == null ? null : ToDto(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("Username is required.");
            
        if (!await _userRepo.IsUsernameUniqueAsync(dto.Username))
            throw new InvalidOperationException("Username already exists.");

        var user = new User
        {
            Username = dto.Username,
            FullName = dto.FullName,
            PasswordHash = PasswordHelper.HashPassword(dto.Password),
            Role = dto.Role,
            IsActive = dto.IsActive,
            CreatedAt = DateTime.Now
        };

        await _userRepo.AddAsync(user);
        return ToDto(user);
    }

    public async Task UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _userRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");

        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentException("Username is required.");

        if (!await _userRepo.IsUsernameUniqueAsync(dto.Username, id))
            throw new InvalidOperationException("Username already exists.");

        user.Username = dto.Username;
        user.FullName = dto.FullName;
        user.Role = dto.Role;
        user.IsActive = dto.IsActive;

        await _userRepo.UpdateAsync(user);
    }

    public async Task DeleteAsync(int id)
    {
        // Don't allow deleting the last owner or maybe a soft delete
        var user = await _userRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
        
        await _userRepo.DeleteAsync(id);
    }

    public async Task ResetPasswordAsync(int id, string newPassword)
    {
        var user = await _userRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException("User not found.");
        
        if (string.IsNullOrWhiteSpace(newPassword))
            throw new ArgumentException("Password cannot be empty.");

        user.PasswordHash = PasswordHelper.HashPassword(newPassword);
        await _userRepo.UpdateAsync(user);
    }

    private static UserDto ToDto(User u) => new(
        u.Id,
        u.Username,
        u.FullName,
        u.Email,
        u.Phone,
        u.Role,
        u.IsActive,
        u.LastLoginAt
    );
}
