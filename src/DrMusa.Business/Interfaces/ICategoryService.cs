using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

/// <summary>Business service interface for Category Management (Module 4).</summary>
public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllAsync();
    Task<CategoryDto?> GetByIdAsync(int id);
    Task<IEnumerable<CategoryDto>> SearchAsync(string searchTerm);
    Task<CategoryDto> CreateAsync(CreateCategoryDto dto);
    Task UpdateAsync(int id, CreateCategoryDto dto);

    /// <summary>Soft-deletes a category. Throws if it has active products.</summary>
    Task DeleteAsync(int id);
}
