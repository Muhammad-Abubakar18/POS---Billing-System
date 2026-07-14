using DrMusa.Common.Enums;

namespace DrMusa.Business.DTOs;

public record UserDto(
    int Id,
    string Username,
    string FullName,
    string? Email,
    string? Phone,
    UserRole Role,
    bool IsActive,
    DateTime? LastLoginAt
);

public record CreateUserDto(
    string Username,
    string FullName,
    UserRole Role,
    string Password,
    bool IsActive
);

public record UpdateUserDto(
    string Username,
    string FullName,
    UserRole Role,
    bool IsActive
);
