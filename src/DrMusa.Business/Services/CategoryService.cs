using DrMusa.Business.DTOs;
using DrMusa.Business.Interfaces;
using DrMusa.Data.Models;
using DrMusa.Data.Repositories;

namespace DrMusa.Business.Services;

/// <summary>Business service implementing ICategoryService for Module 4.</summary>
public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryService(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllAsync()
    {
        var categories = await _categoryRepo.GetAllWithProductCountAsync();
        return categories.Select(ToDto);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id)
    {
        var c = await _categoryRepo.GetByIdAsync(id);
        return c is null ? null : ToDtoSimple(c);
    }

    public async Task<IEnumerable<CategoryDto>> SearchAsync(string searchTerm)
    {
        var results = await _categoryRepo.SearchAsync(searchTerm);
        return results.Select(ToDtoSimple);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        var created = await _categoryRepo.AddAsync(category);
        return ToDtoSimple(created);
    }

    public async Task UpdateAsync(int id, CreateCategoryDto dto)
    {
        var category = await _categoryRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category {id} not found.");

        category.Name = dto.Name.Trim();
        category.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();

        await _categoryRepo.UpdateAsync(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categoryRepo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Category {id} not found.");

        // Guard: cannot delete a category that still has active products
        if (await _categoryRepo.HasActiveProductsAsync(id))
            throw new InvalidOperationException(
                $"Cannot delete category '{category.Name}' — it still has active products assigned to it. " +
                "Please reassign or deactivate those products first.");

        // Soft delete
        category.IsActive = false;
        await _categoryRepo.UpdateAsync(category);
    }

    // --- Mappers ---

    private static CategoryDto ToDto(Category c) => new(
        c.Id,
        c.Name,
        c.Description,
        c.IsActive,
        c.Products.Count(p => p.IsActive)
    );

    private static CategoryDto ToDtoSimple(Category c) => new(
        c.Id,
        c.Name,
        c.Description,
        c.IsActive,
        c.Products.Count(p => p.IsActive)
    );
}
