using Restify.Auth.Domain.Common;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Domain.Interfaces;

namespace Restify.Auth.Domain.Entities;

/// <summary>
/// Representa un usuario del sistema
/// </summary>
public class User : AuditableEntity, ITenantEntity
{
    /// <summary>
    /// ID del Tenant al que pertenece el usuario
    /// </summary>
    public Guid TenantId { get; set; }

    /// <summary>
    /// Email del usuario (único por tenant)
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash de la contraseña
    /// </summary>
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Nombre del usuario
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Apellido del usuario
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Teléfono del usuario
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// URL del avatar del usuario
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Estado del usuario
    /// </summary>
    public UserStatus Status { get; set; } = UserStatus.Active;

    /// <summary>
    /// Fecha del último acceso
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Número de intentos fallidos de login
    /// </summary>
    public int FailedLoginAttempts { get; set; } = 0;

    /// <summary>
    /// Fecha hasta la cual el usuario está bloqueado
    /// </summary>
    public DateTime? LockoutEndAt { get; set; }

    /// <summary>
    /// Token para refrescar el JWT
    /// </summary>
    public string? RefreshToken { get; set; }

    /// <summary>
    /// Fecha de expiración del refresh token
    /// </summary>
    public DateTime? RefreshTokenExpiresAt { get; set; }

    /// <summary>
    /// Token para verificación de email
    /// </summary>
    public string? EmailVerificationToken { get; set; }

    /// <summary>
    /// Indica si el email ha sido verificado
    /// </summary>
    public bool EmailVerified { get; set; } = false;

    /// <summary>
    /// Token para reseteo de contraseña
    /// </summary>
    public string? PasswordResetToken { get; set; }

    /// <summary>
    /// Fecha de expiración del token de reseteo
    /// </summary>
    public DateTime? PasswordResetTokenExpiresAt { get; set; }

    // Navegación
    public virtual Tenant Tenant { get; set; } = null!;
    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    /// <summary>
    /// Nombre completo del usuario
    /// </summary>
    public string FullName => $"{FirstName} {LastName}".Trim();

    /// <summary>
    /// Verifica si el usuario está bloqueado
    /// </summary>
    public bool IsLocked => LockoutEndAt.HasValue && LockoutEndAt > DateTime.UtcNow;
}
