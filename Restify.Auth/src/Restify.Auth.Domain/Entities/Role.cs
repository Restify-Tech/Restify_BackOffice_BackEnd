using Restify.Auth.Domain.Common;
using Restify.Auth.Domain.Interfaces;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Representa un rol en el sistema
/// </summary>
public class Role : AuditableEntity, ITenantEntity
{
    /// <summary>
    /// ID del Tenant al que pertenece el rol (null para roles globales del sistema)
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Nombre del rol
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Nombre normalizado para búsquedas
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Descripción del rol
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indica si es un rol del sistema (no editable)
    /// </summary>
    public bool IsSystem { get; set; } = false;

    /// <summary>
    /// Indica si el rol está activo
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navegación
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public virtual ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
