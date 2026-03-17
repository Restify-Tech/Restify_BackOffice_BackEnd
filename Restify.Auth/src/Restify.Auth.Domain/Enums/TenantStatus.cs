namespace Restify.Auth.Domain.Enums;

/// <summary>
/// Estado del Tenant (Restaurante)
/// </summary>
public enum TenantStatus
{
    /// <summary>
    /// Tenant activo
    /// </summary>
    Active = 1,

    /// <summary>
    /// Tenant inactivo
    /// </summary>
    Inactive = 2,

    /// <summary>
    /// Tenant suspendido por falta de pago u otra razón
    /// </summary>
    Suspended = 3,

    /// <summary>
    /// Periodo de prueba
    /// </summary>
    Trial = 4
}
