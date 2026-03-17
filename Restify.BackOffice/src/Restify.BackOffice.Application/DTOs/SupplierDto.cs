namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de proveedor
/// </summary>
public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Request para crear proveedor
/// </summary>
public class CreateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Request para actualizar proveedor
/// </summary>
public class UpdateSupplierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? TaxId { get; set; }
    public string? ContactName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Notes { get; set; }
    public bool IsActive { get; set; }
}
