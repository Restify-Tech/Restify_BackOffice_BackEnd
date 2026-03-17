using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

/// <summary>
/// Servicio de asignación automática de repartidores a entregas.
/// Soporta Standalone (propios), Networked (pool) y Hybrid (propios + pool fallback).
/// Criterios de selección: Status == Available, verificado, proximidad al restaurante, rating.
/// </summary>
public class DeliveryAssignmentService : IDeliveryAssignmentService
{
    private readonly BackOfficeDbContext _context;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IDeliveryDriverRepository _driverRepository;
    private readonly IOrderNotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<DeliveryAssignmentService> _logger;

    public DeliveryAssignmentService(
        BackOfficeDbContext context,
        IDeliveryRepository deliveryRepository,
        IDeliveryDriverRepository driverRepository,
        IOrderNotificationService notificationService,
        ICurrentUserService currentUserService,
        ILogger<DeliveryAssignmentService> logger)
    {
        _context = context;
        _deliveryRepository = deliveryRepository;
        _driverRepository = driverRepository;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<DeliveryAssignmentResultDto>> AssignDriverAsync(
        Guid deliveryId,
        int deliveryOperationMode,
        Guid? deliveryZoneId,
        CancellationToken cancellationToken = default)
    {
        var delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (delivery == null)
            return Result<DeliveryAssignmentResultDto>.Failure("Entrega no encontrada");

        if (delivery.Status != DeliveryStatus.Pending && delivery.Status != DeliveryStatus.AwaitingPoolAssignment)
            return Result<DeliveryAssignmentResultDto>.Failure("La entrega no está en estado pendiente para asignación");

        if (delivery.DriverId.HasValue)
            return Result<DeliveryAssignmentResultDto>.Failure("La entrega ya tiene un repartidor asignado");

        DeliveryDriver? selectedDriver = null;
        bool isPoolDelivery = false;

        switch (deliveryOperationMode)
        {
            case 1: // Standalone: solo repartidores del tenant
                selectedDriver = await FindBestTenantDriverAsync(delivery, cancellationToken);
                if (selectedDriver == null)
                    return Result<DeliveryAssignmentResultDto>.Failure("No hay repartidores propios disponibles");
                break;

            case 2: // Networked: solo pool
                if (!deliveryZoneId.HasValue)
                    return Result<DeliveryAssignmentResultDto>.Failure("La zona de delivery es requerida para modo Networked");

                selectedDriver = await FindBestPoolDriverAsync(deliveryZoneId.Value, delivery, cancellationToken);
                if (selectedDriver == null)
                    return Result<DeliveryAssignmentResultDto>.Failure("No hay repartidores del pool disponibles en esta zona");
                isPoolDelivery = true;
                break;

            case 3: // Hybrid: propios primero, luego pool
                selectedDriver = await FindBestTenantDriverAsync(delivery, cancellationToken);
                if (selectedDriver == null)
                {
                    if (!deliveryZoneId.HasValue)
                        return Result<DeliveryAssignmentResultDto>.Failure("No hay repartidores propios disponibles y la zona de delivery es requerida para escalar al pool");

                    selectedDriver = await FindBestPoolDriverAsync(deliveryZoneId.Value, delivery, cancellationToken);
                    if (selectedDriver == null)
                        return Result<DeliveryAssignmentResultDto>.Failure("No hay repartidores disponibles (propios ni del pool)");
                    isPoolDelivery = true;
                }
                break;

            default:
                return Result<DeliveryAssignmentResultDto>.Failure($"Modo de operación de delivery no válido: {deliveryOperationMode}");
        }

        // Asignar el repartidor a la entrega
        delivery.DriverId = selectedDriver.Id;
        delivery.Status = DeliveryStatus.Assigned;
        delivery.AssignedAt = DateTime.UtcNow;
        delivery.IsPoolDelivery = isPoolDelivery;
        delivery.DeliveryZoneId = deliveryZoneId;

        // Actualizar estado del repartidor
        selectedDriver.Status = DriverStatus.OnDelivery;
        await _driverRepository.UpdateAsync(selectedDriver, cancellationToken);

        await _deliveryRepository.UpdateAsync(delivery, cancellationToken);

        // Recargar con propiedades de navegación
        delivery = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        var deliveryDto = delivery!.ToDto();

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        await _notificationService.NotifyDeliveryAssignedAsync(tenantId, selectedDriver.Id, deliveryDto, cancellationToken);

        var result = new DeliveryAssignmentResultDto(
            DriverId: selectedDriver.Id,
            DriverName: $"{selectedDriver.FirstName} {selectedDriver.LastName}",
            IsPoolDelivery: isPoolDelivery,
            DeliveryZoneId: deliveryZoneId
        );

        _logger.LogInformation(
            "Entrega {DeliveryId} asignada automáticamente al repartidor {DriverId} ({DriverName}). Pool: {IsPool}",
            deliveryId, selectedDriver.Id, result.DriverName, isPoolDelivery);

        return Result<DeliveryAssignmentResultDto>.Success(result);
    }

    /// <summary>
    /// Busca el mejor repartidor del tenant (no pool) que esté disponible.
    /// Criterios: Available, IsActive, IsVerified, proximidad al restaurante, rating.
    /// </summary>
    private async Task<DeliveryDriver?> FindBestTenantDriverAsync(Delivery delivery, CancellationToken cancellationToken)
    {
        var availableDrivers = await _context.DeliveryDrivers
            .Where(d => d.Status == DriverStatus.Available
                && d.IsActive
                && d.IsVerified
                && !d.IsPoolDriver)
            .ToListAsync(cancellationToken);

        if (!availableDrivers.Any())
            return null;

        return RankDrivers(availableDrivers, delivery).FirstOrDefault();
    }

    /// <summary>
    /// Busca el mejor repartidor del pool centralizado en la zona indicada.
    /// Criterios: Available, IsActive, VerificationStatus == Approved, misma zona, proximidad, rating.
    /// Usa IgnoreQueryFilters porque los pool drivers tienen TenantId = Guid.Empty.
    /// </summary>
    private async Task<DeliveryDriver?> FindBestPoolDriverAsync(Guid deliveryZoneId, Delivery delivery, CancellationToken cancellationToken)
    {
        var availableDrivers = await _context.DeliveryDrivers
            .IgnoreQueryFilters()
            .Where(d => d.IsPoolDriver
                && d.Status == DriverStatus.Available
                && d.IsActive
                && d.VerificationStatus == DriverVerificationStatus.Approved
                && d.DeliveryZoneId == deliveryZoneId)
            .ToListAsync(cancellationToken);

        if (!availableDrivers.Any())
            return null;

        return RankDrivers(availableDrivers, delivery).FirstOrDefault();
    }

    /// <summary>
    /// Ordena los repartidores por:
    /// 1. Proximidad al restaurante (si ambos tienen GPS)
    /// 2. Rating (mayor primero)
    /// 3. Total de entregas (más experiencia primero)
    /// </summary>
    private static IEnumerable<DeliveryDriver> RankDrivers(IEnumerable<DeliveryDriver> drivers, Delivery delivery)
    {
        var restaurantLat = delivery.RestaurantLatitude;
        var restaurantLon = delivery.RestaurantLongitude;

        return drivers.OrderBy(d =>
        {
            // Si tenemos GPS del restaurante y del repartidor, calcular distancia
            if (restaurantLat.HasValue && restaurantLon.HasValue
                && d.CurrentLatitude.HasValue && d.CurrentLongitude.HasValue)
            {
                return CalculateDistanceKm(
                    restaurantLat.Value, restaurantLon.Value,
                    d.CurrentLatitude.Value, d.CurrentLongitude.Value);
            }
            // Sin GPS, poner al final del ranking de proximidad
            return double.MaxValue;
        })
        .ThenByDescending(d => d.Rating ?? 0)
        .ThenByDescending(d => d.TotalDeliveries);
    }

    /// <summary>
    /// Calcula distancia aproximada en km usando la fórmula de Haversine.
    /// </summary>
    private static double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        const double earthRadiusKm = 6371.0;

        var dLat = DegreesToRadians(lat2 - lat1);
        var dLon = DegreesToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees) => degrees * Math.PI / 180.0;
}
