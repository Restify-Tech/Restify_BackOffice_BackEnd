using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Representa un permiso en el sistema
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Código único del permiso (ej: "orders.create", "catalog.edit")
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// Nombre legible del permiso
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del permiso
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Módulo al que pertenece el permiso
    /// </summary>
    public string Module { get; set; } = string.Empty;

    /// <summary>
    /// Orden para mostrar en la UI
    /// </summary>
    public int DisplayOrder { get; set; } = 0;

    /// <summary>
    /// Indica si el permiso está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Indica si este permiso se asigna por defecto al rol Admin de un nuevo tenant
    /// </summary>
    public bool IsTenantDefault { get; set; } = false;

    // Navegación
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
