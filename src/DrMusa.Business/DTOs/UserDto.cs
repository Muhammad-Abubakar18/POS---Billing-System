using DrMusa.Common.Enums;

namespace DrMusa.Business.DTOs;

public record UserDto(
    int Id,
    string Username,
    string FullName,
    string? Email,
    string? Phone,
    UserRole Role,
    bool IsActive
);

public record CreateUserDto(
    string Username,
    string Password,
    string FullName,
    string? Email,
    string? Phone,
    UserRole Role
);
