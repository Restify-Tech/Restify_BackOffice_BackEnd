using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class DeliveryService : IDeliveryService
{
    private readonly IDeliveryRepository _repository;
    private readonly IOrderRepository _orderRepository;
    private readonly IDeliveryDriverRepository _driverRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderNotificationService _notificationService;
    private readonly ILogger<DeliveryService> _logger;

    public DeliveryService(
        IDeliveryRepository repository,
        IOrderRepository orderRepository,
        IDeliveryDriverRepository driverRepository,
        ICurrentUserService currentUserService,
        IOrderNotificationService notificationService,
        ILogger<DeliveryService> logger)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _driverRepository = driverRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<DeliveryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<DeliveryDto>.Failure("Entrega no encontrada");

        return Result<DeliveryDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<DeliveryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeliveryDto>>.Success(dtos);
    }

    public async Task<Result<DeliveryDto>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByOrderIdAsync(orderId, cancellationToken);
        if (entity == null)
            return Result<DeliveryDto>.Failure("Entrega no encontrada para este pedido");

        return Result<DeliveryDto>.Success(entity.ToDto());
    }

    public async Task<Result<IEnumerable<DeliveryDto>>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetByDriverIdAsync(driverId, cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeliveryDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<DeliveryDto>>> GetPendingAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetPendingAsync(cancellationToken);
        var dtos = entities.Select(e => e.ToDto());
        return Result<IEnumerable<DeliveryDto>>.Success(dtos);
    }

    public async Task<Result<DeliveryDto>> CreateAsync(CreateDeliveryRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            return Result<DeliveryDto>.Failure("Pedido no encontrado");

        if (order.PaymentStatus != PaymentStatus.Paid)
            return Result<DeliveryDto>.Failure("El pedido debe estar pagado para crear una entrega");

        if (order.Type != OrderType.Delivery)
            return Result<DeliveryDto>.Failure("El pedido debe ser de tipo Delivery");

        var existingDelivery = await _repository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existingDelivery != null)
            return Result<DeliveryDto>.Failure("Ya existe una entrega para este pedido");

        if (string.IsNullOrWhiteSpace(request.DeliveryAddress))
            return Result<DeliveryDto>.Failure("La dirección de entrega es requerida");

        var entity = request.ToEntity();
        var created = await _repository.AddAsync(entity, cancellationToken);

        _logger.LogInformation("Entrega {DeliveryId} creada para pedido {OrderNumber}", created.Id, order.OrderNumber);

        return Result<DeliveryDto>.Success(created.ToDto());
    }

    public async Task<Result<DeliveryDto>> AssignAsync(Guid id, AssignDeliveryRequest request, CancellationToken cancellationToken = default)
    {
        var delivery = await _repository.GetByIdAsync(id, cancellationToken);
        if (delivery == null)
            return Result<DeliveryDto>.Failure("Entrega no encontrada");

        if (delivery.Status != DeliveryStatus.Pending)
            return Result<DeliveryDto>.Failure("Solo se pueden asignar entregas pendientes");

        var driver = await _driverRepository.GetByIdAsync(request.DriverId, cancellationToken);
        if (driver == null)
            return Result<DeliveryDto>.Failure("Motorizado no encontrado");

        if (!driver.IsActive || !driver.IsVerified)
            return Result<DeliveryDto>.Failure("El motorizado no está activo o verificado");

        if (driver.Status != DriverStatus.Available)
            return Result<DeliveryDto>.Failure("El motorizado no está disponible");

        // Check if driver already has an active delivery
        var activeDelivery = await _repository.GetActiveByDriverAsync(request.DriverId, cancellationToken);
        if (activeDelivery != null)
            return Result<DeliveryDto>.Failure("El motorizado ya tiene una entrega activa");

        delivery.DriverId = request.DriverId;
        delivery.Status = DeliveryStatus.Assigned;
        delivery.AssignedAt = DateTime.UtcNow;

        // Update driver status
        driver.Status = DriverStatus.OnDelivery;
        await _driverRepository.UpdateAsync(driver, cancellationToken);

        await _repository.UpdateAsync(delivery, cancellationToken);

        // Reload with navigation properties
        delivery = await _repository.GetByIdAsync(id, cancellationToken);
        var dto = delivery!.ToDto();

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        await _notificationService.NotifyDeliveryAssignedAsync(tenantId, request.DriverId, dto, cancellationToken);

        _logger.LogInformation("Entrega {DeliveryId} asignada a motorizado {DriverId}", id, request.DriverId);

        return Result<DeliveryDto>.Success(dto);
    }

    public async Task<Result<DeliveryDto>> UpdateStatusAsync(Guid id, UpdateDeliveryStatusRequest request, CancellationToken cancellationToken = default)
    {
        var delivery = await _repository.GetByIdAsync(id, cancellationToken);
        if (delivery == null)
            return Result<DeliveryDto>.Failure("Entrega no encontrada");

        var newStatus = (DeliveryStatus)request.Status;

        // Validate status transitions
        var validTransition = (delivery.Status, newStatus) switch
        {
            (DeliveryStatus.Assigned, DeliveryStatus.PickedUp) => true,
            (DeliveryStatus.PickedUp, DeliveryStatus.InTransit) => true,
            (DeliveryStatus.InTransit, DeliveryStatus.Delivered) => true,
            (DeliveryStatus.Assigned, DeliveryStatus.Failed) => true,
            (DeliveryStatus.PickedUp, DeliveryStatus.Failed) => true,
            (DeliveryStatus.InTransit, DeliveryStatus.Failed) => true,
            (DeliveryStatus.Pending, DeliveryStatus.Cancelled) => true,
            (DeliveryStatus.Assigned, DeliveryStatus.Cancelled) => true,
            _ => false
        };

        if (!validTransition)
            return Result<DeliveryDto>.Failure($"No se puede cambiar el estado de {delivery.Status} a {newStatus}");

        delivery.Status = newStatus;

        switch (newStatus)
        {
            case DeliveryStatus.PickedUp:
                delivery.PickedUpAt = DateTime.UtcNow;
                break;
            case DeliveryStatus.Delivered:
                delivery.DeliveredAt = DateTime.UtcNow;
                if (delivery.DriverId.HasValue)
                {
                    var driver = await _driverRepository.GetByIdAsync(delivery.DriverId.Value, cancellationToken);
                    if (driver != null)
                    {
                        driver.Status = DriverStatus.Available;
                        driver.TotalDeliveries++;
                        await _driverRepository.UpdateAsync(driver, cancellationToken);
                    }
                }
                break;
            case DeliveryStatus.Failed:
                delivery.FailureReason = request.FailureReason;
                if (delivery.DriverId.HasValue)
                {
                    var failedDriver = await _driverRepository.GetByIdAsync(delivery.DriverId.Value, cancellationToken);
                    if (failedDriver != null)
                    {
                        failedDriver.Status = DriverStatus.Available;
                        await _driverRepository.UpdateAsync(failedDriver, cancellationToken);
                    }
                }
                break;
            case DeliveryStatus.Cancelled:
                if (delivery.DriverId.HasValue)
                {
                    var cancelledDriver = await _driverRepository.GetByIdAsync(delivery.DriverId.Value, cancellationToken);
                    if (cancelledDriver != null)
                    {
                        cancelledDriver.Status = DriverStatus.Available;
                        await _driverRepository.UpdateAsync(cancelledDriver, cancellationToken);
                    }
                }
                break;
        }

        await _repository.UpdateAsync(delivery, cancellationToken);

        delivery = await _repository.GetByIdAsync(id, cancellationToken);
        var dto = delivery!.ToDto();

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        await _notificationService.NotifyDeliveryStatusChangedAsync(tenantId, dto, cancellationToken);

        _logger.LogInformation("Entrega {DeliveryId} actualizada a estado {Status}", id, newStatus);

        return Result<DeliveryDto>.Success(dto);
    }

    public async Task<Result<DeliveryDto>> UpdateLocationAsync(Guid id, UpdateDriverLocationRequest request, CancellationToken cancellationToken = default)
    {
        var delivery = await _repository.GetByIdAsync(id, cancellationToken);
        if (delivery == null)
            return Result<DeliveryDto>.Failure("Entrega no encontrada");

        delivery.DriverLatitude = request.Latitude;
        delivery.DriverLongitude = request.Longitude;
        delivery.LastLocationUpdate = DateTime.UtcNow;

        await _repository.UpdateAsync(delivery, cancellationToken);

        // También actualizar la ubicación GPS del DeliveryDriver
        if (delivery.DriverId.HasValue)
        {
            var driver = await _driverRepository.GetByIdAsync(delivery.DriverId.Value, cancellationToken);
            if (driver != null)
            {
                driver.CurrentLatitude = (double)request.Latitude;
                driver.CurrentLongitude = (double)request.Longitude;
                driver.LastLocationUpdateAt = DateTime.UtcNow;
                await _driverRepository.UpdateAsync(driver, cancellationToken);
            }
        }

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var locationDto = new DriverLocationDto(delivery.Id, request.Latitude, request.Longitude, DateTime.UtcNow);
        await _notificationService.NotifyDriverLocationUpdatedAsync(tenantId, locationDto, cancellationToken);

        return Result<DeliveryDto>.Success(delivery.ToDto());
    }

    public async Task<Result<DeliveryTrackingDto>> GetTrackingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var delivery = await _repository.GetByIdAsync(id, cancellationToken);
        if (delivery == null)
            return Result<DeliveryTrackingDto>.Failure("Entrega no encontrada");

        return Result<DeliveryTrackingDto>.Success(delivery.ToTrackingDto());
    }
}
