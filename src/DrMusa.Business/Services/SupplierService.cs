using DrMusa.Business.DTOs;
using DrMusa.Common.Exceptions;
using DrMusa.Business.Interfaces;
using DrMusa.Data.Repositories;

namespace DrMusa.Business.Services;

public class SupplierService : ISupplierService
{
    private readonly ISupplierRepository _supplierRepository;

    public SupplierService(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(SupplierDto.FromEntity).ToList();
    }

    public async Task<IEnumerable<SupplierDto>> GetActiveSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Where(s => s.IsActive).Select(SupplierDto.FromEntity).ToList();
    }

    public async Task<SupplierDto?> GetByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        return supplier != null ? SupplierDto.FromEntity(supplier) : null;
    }

    public async Task<SupplierDto> CreateAsync(SupplierDto supplierDto)
    {
        if (string.IsNullOrWhiteSpace(supplierDto.Name))
            throw new ValidationException("Supplier Name is required.");

        var allSuppliers = await _supplierRepository.GetAllAsync();
        var existing = allSuppliers.Where(s => s.Name.ToLower() == supplierDto.Name.ToLower());
        if (existing.Any())
            throw new ValidationException("A supplier with this name already exists.");

        var entity = supplierDto.ToEntity();
        entity.CreatedAt = DateTime.Now;
        entity.IsActive = true;

        await _supplierRepository.AddAsync(entity);
        return SupplierDto.FromEntity(entity);
    }

    public async Task UpdateAsync(SupplierDto supplierDto)
    {
        if (string.IsNullOrWhiteSpace(supplierDto.Name))
            throw new ValidationException("Supplier Name is required.");

        var existing = await _supplierRepository.GetByIdAsync(supplierDto.Id);
        if (existing == null)
            throw new NotFoundException("Supplier", supplierDto.Id);

        // Check for duplicate name
        var allSuppliers = await _supplierRepository.GetAllAsync();
        var duplicateCheck = allSuppliers.Where(s => s.Name.ToLower() == supplierDto.Name.ToLower() && s.Id != supplierDto.Id);
        if (duplicateCheck.Any())
            throw new ValidationException("Another supplier with this name already exists.");

        existing.Name = supplierDto.Name;
        existing.ContactPerson = supplierDto.ContactPerson;
        existing.Phone = supplierDto.Phone;
        existing.Email = supplierDto.Email;
        existing.Address = supplierDto.Address;
        
        await _supplierRepository.UpdateAsync(existing);
    }

    public async Task DeleteAsync(int id)
    {
        var supplier = await _supplierRepository.GetSupplierWithPurchasesByIdAsync(id);
        if (supplier == null)
            throw new NotFoundException("Supplier", id);

        if (supplier.Purchases != null && supplier.Purchases.Any())
        {
            throw new ValidationException("Cannot delete supplier as they have associated purchases. Consider deactivating instead.");
        }

        await _supplierRepository.DeleteAsync(id);
    }

    public async Task ToggleStatusAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            throw new NotFoundException("Supplier", id);

        supplier.IsActive = !supplier.IsActive;
        await _supplierRepository.UpdateAsync(supplier);
    }
}
