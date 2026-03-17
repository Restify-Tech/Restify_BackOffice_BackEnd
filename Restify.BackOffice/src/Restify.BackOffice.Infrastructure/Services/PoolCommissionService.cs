using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

/// <summary>
/// Servicio para calcular y registrar comisiones de entregas del pool centralizado.
/// La comisión se calcula sobre el monto del pedido usando el porcentaje de la zona.
/// </summary>
public class PoolCommissionService : IPoolCommissionService
{
    private readonly BackOfficeDbContext _context;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<PoolCommissionService> _logger;

    public PoolCommissionService(
        BackOfficeDbContext context,
        IDeliveryRepository deliveryRepository,
        ILogger<PoolCommissionService> logger)
    {
        _context = context;
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task<Result<PoolDeliveryCommission>> CreateCommissionAsync(
        Guid deliveryId,
        decimal commissionPercentage,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery == null)
            return Result<PoolDeliveryCommission>.Failure("Entrega no encontrada");

        if (!delivery.IsPoolDelivery)
            return Result<PoolDeliveryCommission>.Failure("La entrega no es del pool centralizado");

        if (!delivery.DriverId.HasValue)
            return Result<PoolDeliveryCommission>.Failure("La entrega no tiene repartidor asignado");

        if (delivery.Order == null)
            return Result<PoolDeliveryCommission>.Failure("No se pudo cargar el pedido asociado a la entrega");

        // Usar el porcentaje almacenado en la entrega si existe, sino el proporcionado
        var percentage = delivery.PoolCommissionPercentage ?? commissionPercentage;
        if (percentage <= 0)
            return Result<PoolDeliveryCommission>.Failure("El porcentaje de comisión debe ser mayor a cero");

        var orderAmount = delivery.Order.Total;
        var commissionAmount = Math.Round(orderAmount * percentage / 100m, 2);

        var commission = new PoolDeliveryCommission
        {
            Id = Guid.NewGuid(),
            DeliveryId = deliveryId,
            DriverId = delivery.DriverId.Value,
            OrderAmount = orderAmount,
            DeliveryFee = delivery.DeliveryFee,
            CommissionPercentage = percentage,
            CommissionAmount = commissionAmount,
            Status = PoolCommissionStatus.Pending
        };

        // Actualizar los datos de comisión en la entrega
        delivery.PoolCommissionPercentage = percentage;
        delivery.PoolCommissionAmount = commissionAmount;
        await _deliveryRepository.UpdateAsync(delivery, cancellationToken);

        await _context.PoolDeliveryCommissions.AddAsync(commission, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Comisión pool creada: Entrega {DeliveryId}, Monto pedido: {OrderAmount}, Porcentaje: {Percentage}%, Comisión: {Commission}",
            deliveryId, orderAmount, percentage, commissionAmount);

        return Result<PoolDeliveryCommission>.Success(commission);
    }
}
