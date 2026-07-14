using DrMusa.Data.Models;

namespace DrMusa.Business.DTOs;

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public static SupplierDto FromEntity(Supplier entity)
    {
        return new SupplierDto
        {
            Id = entity.Id,
            Name = entity.Name,
            ContactPerson = entity.ContactPerson,
            Phone = entity.Phone,
            Email = entity.Email,
            Address = entity.Address,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt
        };
    }

    public Supplier ToEntity()
    {
        return new Supplier
        {
            Id = Id,
            Name = Name,
            ContactPerson = ContactPerson,
            Phone = Phone,
            Email = Email,
            Address = Address,
            IsActive = IsActive,
            CreatedAt = CreatedAt
        };
    }
}
