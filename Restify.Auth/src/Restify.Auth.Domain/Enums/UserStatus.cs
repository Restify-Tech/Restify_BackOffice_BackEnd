namespace Restify.Auth.Domain.Enums;

/// <summary>
/// Estado del usuario
/// </summary>
public enum UserStatus
{
    /// <summary>
    /// Usuario activo
    /// </summary>
    Active = 1,

    /// <summary>
    /// Usuario inactivo
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Usuario bloqueado por intentos fallidos
    /// </summary>
    Locked = 3,

    /// <summary>
    /// Pendiente de verificación de email
    /// </summary>
    PendingVerification = 4
}
