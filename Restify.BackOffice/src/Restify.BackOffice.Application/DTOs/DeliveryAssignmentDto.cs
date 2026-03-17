namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// Resultado de la asignación automática de un repartidor a una entrega
/// </summary>
public record DeliveryAssignmentResultDto(
    Guid DriverId,
    string DriverName,
    bool IsPoolDelivery,
    Guid? DeliveryZoneId
);

/// <summary>
/// Request para asignación automática de repartidor
/// </summary>
public record AutoAssignDeliveryRequest(
    /// <summary>
    /// Modo de operación de delivery del tenant:
    /// 1 = Standalone (solo repartidores propios)
    /// 2 = Networked (solo pool)
    /// 3 = Hybrid (propios primero, luego pool)
    /// </summary>
    int DeliveryOperationMode,

    /// <summary>
    /// Zona de delivery (requerida para modos Networked y Hybrid)
    /// </summary>
    Guid? DeliveryZoneId
);

/// <summary>
/// Request para actualizar ubicación GPS desde el hub o REST
/// </summary>
public record UpdateLocationRequest(
    double Latitude,
    double Longitude
);

/// <summary>
/// Datos de ubicación GPS en tiempo real para broadcast
/// </summary>
public record DeliveryLocationBroadcast(
    Guid DeliveryId,
    Guid DriverId,
    double Latitude,
    double Longitude,
    DateTime UpdatedAt
);

/// <summary>
/// Request para crear una comisión de entrega pool
/// </summary>
public record CreatePoolCommissionRequest(
    /// <summary>
    /// Porcentaje de comisión (de la zona de delivery, obtenido del servicio Auth)
    /// </summary>
    decimal CommissionPercentage
);
