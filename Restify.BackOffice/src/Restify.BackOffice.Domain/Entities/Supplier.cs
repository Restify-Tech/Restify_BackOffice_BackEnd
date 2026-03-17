using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Proveedor de productos
/// </summary>
public class Supplier : TenantEntity
{
    /// <summary>
    /// Nombre del proveedor
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// RUC o identificación fiscal
    /// </summary>
    public string? TaxId { get; set; }
    
    /// <summary>
    /// Persona de contacto
    /// </summary>
    public string? ContactName { get; set; }
    
    /// <summary>
    /// Email
    /// </summary>
    public string? Email { get; set; }
    
    /// <summary>
    /// Teléfono
    /// </summary>
    public string? Phone { get; set; }
    
    /// <summary>
    /// Dirección
    /// </summary>
    public string? Address { get; set; }
    
    /// <summary>
    /// Notas adicionales
    /// </summary>
    public string? Notes { get; set; }
    
    /// <summary>
    /// Si está activo
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Órdenes de compra de este proveedor
    /// </summary>
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
}
