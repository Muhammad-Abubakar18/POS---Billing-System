using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync();
    Task<ProductDto?> GetByIdAsync(int id);
    Task<ProductDto?> GetByBarcodeAsync(string barcode);
    Task<IEnumerable<ProductDto>> SearchAsync(string searchTerm);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task UpdateAsync(int id, CreateProductDto dto);
    Task DeleteAsync(int id);
}
