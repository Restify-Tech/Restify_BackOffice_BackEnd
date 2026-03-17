using Restify.Auth.Domain.Common;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Relación muchos a muchos entre User y Role
/// </summary>
public class UserRole : BaseEntity
{
    /// <summary>
    /// ID del usuario
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// ID del rol
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// Fecha de asignación del rol
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Usuario que asignó el rol
    /// </summary>
    public Guid? AssignedBy { get; set; }

    // Navegación
    public virtual User User { get; set; } = null!;
    public virtual Role Role { get; set; } = null!;
}
