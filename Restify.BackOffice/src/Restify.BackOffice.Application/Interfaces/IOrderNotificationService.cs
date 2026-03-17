using Restify.BackOffice.Application.DTOs;

namespace Restify.BackOffice.Application.Interfaces;

/// <summary>
/// Abstraction for sending real-time order notifications via SignalR
/// </summary>
public interface IOrderNotificationService
{
    Task NotifyOrderCreatedAsync(Guid tenantId, OrderDto order, CancellationToken cancellationToken = default);
    Task NotifyOrderStatusChangedAsync(Guid tenantId, OrderDto order, CancellationToken cancellationToken = default);
    Task NotifyOrderItemStatusChangedAsync(Guid tenantId, OrderDto order, Guid itemId, CancellationToken cancellationToken = default);
    Task NotifyDispatchApprovedAsync(Guid tenantId, DispatchApprovalDto approval, CancellationToken cancellationToken = default);
    Task NotifyDispatchRejectedAsync(Guid tenantId, DispatchApprovalDto approval, CancellationToken cancellationToken = default);
    Task NotifyPaymentReceivedAsync(Guid tenantId, PaymentDto payment, CancellationToken cancellationToken = default);
    Task NotifyDeliveryAssignedAsync(Guid tenantId, Guid driverId, DeliveryDto delivery, CancellationToken cancellationToken = default);
    Task NotifyDeliveryStatusChangedAsync(Guid tenantId, DeliveryDto delivery, CancellationToken cancellationToken = default);
    Task NotifyDriverLocationUpdatedAsync(Guid tenantId, DriverLocationDto location, CancellationToken cancellationToken = default);
    Task NotifyTransferPaymentPendingAsync(Guid tenantId, TransferPaymentRequestDto transferRequest, CancellationToken cancellationToken = default);
}
