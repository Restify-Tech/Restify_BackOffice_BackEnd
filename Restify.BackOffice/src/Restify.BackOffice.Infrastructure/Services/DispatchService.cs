using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class DispatchService : IDispatchService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderNotificationService _notificationService;
    private readonly ILogger<DispatchService> _logger;

    public DispatchService(
        IOrderRepository orderRepository,
        ICurrentUserService currentUserService,
        IOrderNotificationService notificationService,
        ILogger<DispatchService> logger)
    {
        _orderRepository = orderRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<DispatchQueueItemDto>>> GetDispatchQueueAsync(CancellationToken cancellationToken = default)
    {
        // Get orders that are Ready or AwaitingDispatchApproval
        var readyOrders = await _orderRepository.GetByStatusAsync(OrderStatus.Ready, cancellationToken);
        var awaitingOrders = await _orderRepository.GetByStatusAsync(OrderStatus.AwaitingDispatchApproval, cancellationToken);

        var allOrders = readyOrders.Concat(awaitingOrders)
            .OrderBy(o => o.UpdatedAt ?? o.CreatedAt);

        var dtos = allOrders.Select(o => new DispatchQueueItemDto(
            OrderId: o.Id,
            OrderNumber: o.OrderNumber,
            OrderType: o.Type.ToString(),
            CustomerName: o.CustomerName ?? "Sin nombre",
            TableNumber: o.Table?.Number,
            ItemCount: o.Items.Count,
            Total: o.Total,
            PaymentStatus: o.PaymentStatus.ToString(),
            ReadyAt: o.UpdatedAt ?? o.CreatedAt,
            Notes: o.Notes));

        return Result<IEnumerable<DispatchQueueItemDto>>.Success(dtos);
    }

    public async Task<Result<DispatchApprovalDto>> ApproveDispatchAsync(Guid orderId, ApproveDispatchRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<DispatchApprovalDto>.Failure("Pedido no encontrado");

        if (order.Status != OrderStatus.Ready && order.Status != OrderStatus.AwaitingDispatchApproval)
            return Result<DispatchApprovalDto>.Failure("El pedido debe estar listo o esperando aprobación para despachar");

        var approvedBy = _currentUserService.Email ?? "Sistema";
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        // Create dispatch approval
        var approval = new DispatchApproval
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = orderId,
            Status = DispatchApprovalStatus.Approved,
            ApprovedBy = approvedBy,
            ApprovedAt = DateTime.UtcNow,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Update order status based on order type
        if (order.Type == OrderType.DineIn)
        {
            order.Status = OrderStatus.Served;
        }
        else
        {
            // For Takeaway/Delivery, move to Completed
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
        }

        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        _logger.LogInformation(
            "Despacho aprobado para pedido {OrderNumber} por {ApprovedBy}",
            order.OrderNumber, approvedBy);

        var dto = ToDto(approval, order.OrderNumber);

        // Notify real-time clients
        await _notificationService.NotifyDispatchApprovedAsync(tenantId, dto, cancellationToken);

        return Result<DispatchApprovalDto>.Success(dto);
    }

    public async Task<Result<DispatchApprovalDto>> RejectDispatchAsync(Guid orderId, RejectDispatchRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<DispatchApprovalDto>.Failure("Pedido no encontrado");

        if (order.Status != OrderStatus.Ready && order.Status != OrderStatus.AwaitingDispatchApproval)
            return Result<DispatchApprovalDto>.Failure("El pedido debe estar listo o esperando aprobación para rechazar despacho");

        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<DispatchApprovalDto>.Failure("Debe proporcionar un motivo para rechazar el despacho");

        var approvedBy = _currentUserService.Email ?? "Sistema";
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        // Create dispatch rejection
        var approval = new DispatchApproval
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = orderId,
            Status = DispatchApprovalStatus.Rejected,
            ApprovedBy = approvedBy,
            ApprovedAt = DateTime.UtcNow,
            RejectionReason = request.Reason,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        // Revert order status to Ready (needs to be re-prepared or re-checked)
        order.Status = OrderStatus.Ready;
        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);

        _logger.LogInformation(
            "Despacho rechazado para pedido {OrderNumber} por {ApprovedBy}. Motivo: {Reason}",
            order.OrderNumber, approvedBy, request.Reason);

        var dto = ToDto(approval, order.OrderNumber);

        // Notify real-time clients
        await _notificationService.NotifyDispatchRejectedAsync(tenantId, dto, cancellationToken);

        return Result<DispatchApprovalDto>.Success(dto);
    }

    public async Task<Result<DispatchApprovalDto>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<DispatchApprovalDto>.Failure("Pedido no encontrado");

        // Return a DTO reflecting the current dispatch state based on the order status
        var dto = new DispatchApprovalDto(
            Id: Guid.Empty,
            OrderId: orderId,
            OrderNumber: order.OrderNumber,
            Status: order.Status == OrderStatus.Served || order.Status == OrderStatus.Completed
                ? DispatchApprovalStatus.Approved.ToString()
                : order.Status == OrderStatus.Ready || order.Status == OrderStatus.AwaitingDispatchApproval
                    ? DispatchApprovalStatus.Pending.ToString()
                    : "N/A",
            ApprovedBy: null,
            ApprovedAt: null,
            RejectionReason: null,
            Notes: null,
            CreatedAt: order.CreatedAt);

        return Result<DispatchApprovalDto>.Success(dto);
    }

    #region Private Methods

    private static DispatchApprovalDto ToDto(DispatchApproval approval, string orderNumber)
    {
        return new DispatchApprovalDto(
            Id: approval.Id,
            OrderId: approval.OrderId,
            OrderNumber: orderNumber,
            Status: approval.Status.ToString(),
            ApprovedBy: approval.ApprovedBy,
            ApprovedAt: approval.ApprovedAt,
            RejectionReason: approval.RejectionReason,
            Notes: approval.Notes,
            CreatedAt: approval.CreatedAt);
    }

    #endregion
}
