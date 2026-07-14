using DrMusa.Business.DTOs;

namespace DrMusa.Business.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllAsync();
    Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync();
    Task<SupplierDto?> GetByIdAsync(int id);
    Task<SupplierDto> CreateAsync(SupplierDto supplierDto);
    Task UpdateAsync(SupplierDto supplierDto);
    Task DeleteAsync(int id);
    Task ToggleStatusAsync(int id);
}
