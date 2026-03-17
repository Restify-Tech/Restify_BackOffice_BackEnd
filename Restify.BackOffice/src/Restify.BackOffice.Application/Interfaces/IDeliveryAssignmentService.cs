using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Servicio de asignación automática de repartidores a entregas.
/// Soporta tres modos de operación: Standalone, Networked (pool) y Hybrid.
/// </summary>
public interface IDeliveryAssignmentService
{
    /// <summary>
    /// Asigna automáticamente un repartidor a la entrega según el modo de operación.
    /// Standalone (1): solo repartidores del tenant.
    /// Networked (2): solo repartidores del pool con VerificationStatus Approved.
    /// Hybrid (3): intenta propios primero, si no hay disponibles escala al pool.
    /// </summary>
    /// <param name="deliveryId">ID de la entrega</param>
    /// <param name="deliveryOperationMode">Modo de operación (1=Standalone, 2=Networked, 3=Hybrid)</param>
    /// <param name="deliveryZoneId">Zona de delivery (requerida para pool)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task<Result<DeliveryAssignmentResultDto>> AssignDriverAsync(
        Guid deliveryId,
        int deliveryOperationMode,
        Guid? deliveryZoneId,
        CancellationToken cancellationToken = default);
}
