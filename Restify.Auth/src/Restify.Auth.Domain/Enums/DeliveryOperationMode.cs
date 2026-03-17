namespace Restify.Auth.Domain.Enums;

/// <summary>
/// Modo de operación de delivery del tenant
/// </summary>
public enum DeliveryOperationMode
{
    /// <summary>
    /// Solo repartidores propios del restaurante
    /// </summary>
    Standalone = 1,

    /// <summary>
    /// Solo pool centralizado de RestoSaaS
    /// </summary>
    Networked = 2,

    /// <summary>
    /// Propios + pool como respaldo
    /// </summary>
    Hybrid = 3
}
