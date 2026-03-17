using Microsoft.AspNetCore.SignalR;
using Restify.BackOffice.Api.Hubs;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;

namespace Restify.BackOffice.Api.Services;

public class OrderNotificationService : IOrderNotificationService
{
    private readonly IHubContext<OrderHub> _hubContext;
    private readonly ILogger<OrderNotificationService> _logger;

    public OrderNotificationService(
        IHubContext<OrderHub> hubContext,
        ILogger<OrderNotificationService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyOrderCreatedAsync(Guid tenantId, OrderDto order, CancellationToken cancellationToken = default)
    {
        var kitchenGroup = $"tenant:{tenantId}:kitchen";
        var ordersGroup = $"tenant:{tenantId}:orders";

        await Task.WhenAll(
            _hubContext.Clients.Group(kitchenGroup).SendAsync("OrderCreated", order, cancellationToken),
            _hubContext.Clients.Group(ordersGroup).SendAsync("OrderCreated", order, cancellationToken)
        );

        _logger.LogDebug("Notified OrderCreated for order {OrderNumber} to tenant {TenantId}", order.OrderNumber, tenantId);
    }

    public async Task NotifyOrderStatusChangedAsync(Guid tenantId, OrderDto order, CancellationToken cancellationToken = default)
    {
        var kitchenGroup = $"tenant:{tenantId}:kitchen";
        var ordersGroup = $"tenant:{tenantId}:orders";
        var dispatchGroup = $"tenant:{tenantId}:dispatch";

        await Task.WhenAll(
            _hubContext.Clients.Group(kitchenGroup).SendAsync("OrderStatusChanged", order, cancellationToken),
            _hubContext.Clients.Group(ordersGroup).SendAsync("OrderStatusChanged", order, cancellationToken),
            _hubContext.Clients.Group(dispatchGroup).SendAsync("OrderStatusChanged", order, cancellationToken)
        );

        _logger.LogDebug("Notified OrderStatusChanged for order {OrderNumber} to status {Status}", order.OrderNumber, order.StatusName);
    }

    public async Task NotifyOrderItemStatusChangedAsync(Guid tenantId, OrderDto order, Guid itemId, CancellationToken cancellationToken = default)
    {
        var kitchenGroup = $"tenant:{tenantId}:kitchen";

        var payload = new { Order = order, ItemId = itemId };
        await _hubContext.Clients.Group(kitchenGroup).SendAsync("OrderItemStatusChanged", payload, cancellationToken);

        _logger.LogDebug("Notified OrderItemStatusChanged for item {ItemId} in order {OrderNumber}", itemId, order.OrderNumber);
    }

    public async Task NotifyDispatchApprovedAsync(Guid tenantId, DispatchApprovalDto approval, CancellationToken cancellationToken = default)
    {
        var dispatchGroup = $"tenant:{tenantId}:dispatch";
        var ordersGroup = $"tenant:{tenantId}:orders";

        await Task.WhenAll(
            _hubContext.Clients.Group(dispatchGroup).SendAsync("DispatchApproved", approval, cancellationToken),
            _hubContext.Clients.Group(ordersGroup).SendAsync("DispatchApproved", approval, cancellationToken)
        );

        _logger.LogDebug("Notified DispatchApproved for order {OrderNumber}", approval.OrderNumber);
    }

    public async Task NotifyDispatchRejectedAsync(Guid tenantId, DispatchApprovalDto approval, CancellationToken cancellationToken = default)
    {
        var dispatchGroup = $"tenant:{tenantId}:dispatch";
        var ordersGroup = $"tenant:{tenantId}:orders";

        await Task.WhenAll(
            _hubContext.Clients.Group(dispatchGroup).SendAsync("DispatchRejected", approval, cancellationToken),
            _hubContext.Clients.Group(ordersGroup).SendAsync("DispatchRejected", approval, cancellationToken)
        );

        _logger.LogDebug("Notified DispatchRejected for order {OrderNumber}", approval.OrderNumber);
    }

    public async Task NotifyPaymentReceivedAsync(Guid tenantId, PaymentDto payment, CancellationToken cancellationToken = default)
    {
        var ordersGroup = $"tenant:{tenantId}:orders";
        var dispatchGroup = $"tenant:{tenantId}:dispatch";

        await Task.WhenAll(
            _hubContext.Clients.Group(ordersGroup).SendAsync("PaymentReceived", payment, cancellationToken),
            _hubContext.Clients.Group(dispatchGroup).SendAsync("PaymentReceived", payment, cancellationToken)
        );

        _logger.LogDebug("Notified PaymentReceived for payment {PaymentId} on order {OrderId}", payment.Id, payment.OrderId);
    }

    public async Task NotifyDeliveryAssignedAsync(Guid tenantId, Guid driverId, DeliveryDto delivery, CancellationToken cancellationToken = default)
    {
        var deliveryGroup = $"tenant:{tenantId}:delivery";
        var driverGroup = $"driver:{driverId}";

        await Task.WhenAll(
            _hubContext.Clients.Group(deliveryGroup).SendAsync("DeliveryAssigned", delivery, cancellationToken),
            _hubContext.Clients.Group(driverGroup).SendAsync("DeliveryAssigned", delivery, cancellationToken)
        );

        _logger.LogDebug("Notified DeliveryAssigned for delivery {DeliveryId} to driver {DriverId}", delivery.Id, driverId);
    }

    public async Task NotifyDeliveryStatusChangedAsync(Guid tenantId, DeliveryDto delivery, CancellationToken cancellationToken = default)
    {
        var deliveryGroup = $"tenant:{tenantId}:delivery";

        await _hubContext.Clients.Group(deliveryGroup).SendAsync("DeliveryStatusChanged", delivery, cancellationToken);

        _logger.LogDebug("Notified DeliveryStatusChanged for delivery {DeliveryId} to status {Status}", delivery.Id, delivery.Status);
    }

    public async Task NotifyDriverLocationUpdatedAsync(Guid tenantId, DriverLocationDto location, CancellationToken cancellationToken = default)
    {
        var deliveryGroup = $"tenant:{tenantId}:delivery";

        await _hubContext.Clients.Group(deliveryGroup).SendAsync("DriverLocationUpdated", location, cancellationToken);

        _logger.LogDebug("Notified DriverLocationUpdated for delivery {DeliveryId}", location.DeliveryId);
    }

    public async Task NotifyTransferPaymentPendingAsync(Guid tenantId, TransferPaymentRequestDto transferRequest, CancellationToken cancellationToken = default)
    {
        var ordersGroup = $"tenant:{tenantId}:orders";

        await _hubContext.Clients.Group(ordersGroup).SendAsync("TransferPaymentPending", transferRequest, cancellationToken);

        _logger.LogDebug("Notified TransferPaymentPending for order {OrderId}", transferRequest.OrderId);
    }
}
