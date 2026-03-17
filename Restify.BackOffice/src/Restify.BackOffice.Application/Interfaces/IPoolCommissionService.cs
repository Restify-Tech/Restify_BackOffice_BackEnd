using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Servicio para calcular y registrar comisiones de entregas del pool centralizado
/// </summary>
public interface IPoolCommissionService
{
    /// <summary>
    /// Crea un registro de comisión para una entrega del pool.
    /// Usa el porcentaje de comisión almacenado en la entrega (PoolCommissionPercentage).
    /// Si no tiene, usa el porcentaje proporcionado.
    /// </summary>
    /// <param name="deliveryId">ID de la entrega pool</param>
    /// <param name="commissionPercentage">Porcentaje de comisión (de la zona, obtenido externamente)</param>
    /// <param name="cancellationToken">Token de cancelación</param>
    Task<Result<PoolDeliveryCommission>> CreateCommissionAsync(
        Guid deliveryId,
        decimal commissionPercentage,
        CancellationToken cancellationToken = default);
}
