using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Relación muchos a muchos entre Role y Permission
/// </summary>
public class RolePermission : BaseEntity
{
    /// <summary>
    /// ID del rol
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// ID del permiso
    /// </summary>
    public Guid PermissionId { get; set; }

    /// <summary>
    /// Fecha de asignación del permiso
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Navegación
    public virtual Role Role { get; set; } = null!;
    public virtual Permission Permission { get; set; } = null!;
}
