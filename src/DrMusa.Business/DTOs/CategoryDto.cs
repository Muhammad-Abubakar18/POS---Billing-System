namespace DrMusa.Business.DTOs;

/// <summary>Read-only DTO returned from CategoryService queries.</summary>
public record CategoryDto(
    int Id,
    string Name,
    string? Description,
    bool IsActive,
    int ProductCount
);

/// <summary>Input DTO for creating or updating a category.</summary>
public record CreateCategoryDto(
    string Name,
    string? Description
);
