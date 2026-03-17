namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio para obtener información del usuario actual
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// ID del usuario actual
    /// </summary>
    Guid? UserId { get; }

    /// <summary>
    /// ID del tenant actual
    /// </summary>
    Guid? TenantId { get; }

    /// <summary>
    /// Email del usuario actual
    /// </summary>
    string? Email { get; }

    /// <summary>
    /// Roles del usuario actual
    /// </summary>
    IEnumerable<string> Roles { get; }

    /// <summary>
    /// Permisos del usuario actual
    /// </summary>
    IEnumerable<string> Permissions { get; }

    /// <summary>
    /// Indica si el usuario actual es SuperAdmin (sin tenant)
    /// </summary>
    bool IsSuperAdmin { get; }

    /// <summary>
    /// Tipo de usuario (staff, superadmin, customer, driver)
    /// </summary>
    string? UserType { get; }

    /// <summary>
    /// Verifica si el usuario tiene un permiso específico
    /// </summary>
    bool HasPermission(string permission);

    /// <summary>
    /// Verifica si el usuario tiene un rol específico
    /// </summary>
    bool IsInRole(string role);
}
