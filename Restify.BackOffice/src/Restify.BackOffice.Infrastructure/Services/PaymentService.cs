using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Messaging.Events;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentGatewayFactory _gatewayFactory;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderNotificationService _notificationService;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IOrderRepository orderRepository,
        IPaymentGatewayFactory gatewayFactory,
        ICurrentUserService currentUserService,
        IOrderNotificationService notificationService,
        IEventPublisher eventPublisher,
        ILogger<PaymentService> logger)
    {
        _paymentRepository = paymentRepository;
        _orderRepository = orderRepository;
        _gatewayFactory = gatewayFactory;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<Result<PaymentDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(id, cancellationToken);

        if (payment == null)
            return Result<PaymentDto>.Failure("Pago no encontrado");

        return Result<PaymentDto>.Success(ToDto(payment));
    }

    public async Task<Result<IEnumerable<PaymentDto>>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<IEnumerable<PaymentDto>>.Failure("Pedido no encontrado");

        var payments = await _paymentRepository.GetByOrderIdAsync(orderId, cancellationToken);
        var dtos = payments.Select(ToDto);

        return Result<IEnumerable<PaymentDto>>.Success(dtos);
    }

    public async Task<Result<PaymentDto>> ProcessPaymentAsync(ProcessOrderPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            return Result<PaymentDto>.Failure("Pedido no encontrado");

        if (order.Status == OrderStatus.Cancelled)
            return Result<PaymentDto>.Failure("No se puede procesar pago de un pedido cancelado");

        if (order.PaymentStatus == PaymentStatus.Paid)
            return Result<PaymentDto>.Failure("Este pedido ya está completamente pagado");

        if (request.Amount <= 0)
            return Result<PaymentDto>.Failure("El monto debe ser mayor a cero");

        var method = (PaymentMethodType)request.Method;

        // Determine gateway based on payment method
        var gateway = GetGatewayForMethod(method);

        // Process through gateway
        var gatewayRequest = new PaymentGatewayRequest(
            Amount: request.Amount,
            Currency: "USD",
            Description: $"Pago pedido {order.OrderNumber}",
            PayerName: request.PayerName,
            PayerEmail: null,
            PayerIdentification: request.PayerIdentification,
            Metadata: new Dictionary<string, string>
            {
                { "OrderId", order.Id.ToString() },
                { "OrderNumber", order.OrderNumber }
            });

        var gatewayResult = await gateway.ProcessPaymentAsync(gatewayRequest, cancellationToken);

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        // Create payment entity
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = order.Id,
            Amount = request.Amount,
            Method = method,
            Status = gatewayResult.IsSuccess ? PaymentTransactionStatus.Completed : PaymentTransactionStatus.Failed,
            GatewayTransactionId = request.GatewayTransactionId ?? gatewayResult.TransactionId,
            GatewayResponse = gatewayResult.RawResponse,
            PayerName = request.PayerName,
            PayerIdentification = request.PayerIdentification,
            ProcessedAt = gatewayResult.IsSuccess ? DateTime.UtcNow : null,
            FailureReason = gatewayResult.ErrorMessage,
            CreatedAt = DateTime.UtcNow
        };

        var createdPayment = await _paymentRepository.AddAsync(payment, cancellationToken);

        if (gatewayResult.IsSuccess)
        {
            await UpdateOrderPaymentStatus(order, cancellationToken);
        }

        _logger.LogInformation(
            "Pago {PaymentId} procesado para pedido {OrderNumber}: {Status}",
            createdPayment.Id, order.OrderNumber, createdPayment.Status);

        var paymentDto = ToDto(createdPayment);

        // Notify real-time clients about payment
        if (gatewayResult.IsSuccess)
        {
            await _notificationService.NotifyPaymentReceivedAsync(tenantId, paymentDto, cancellationToken);

            // Publish event for PaymentGatewayAPI (async sync)
            await _eventPublisher.PublishAsync(new PaymentProcessedEvent
            {
                PaymentId = createdPayment.Id,
                TenantId = tenantId,
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                Amount = createdPayment.Amount,
                Method = createdPayment.Method.ToString(),
                PayerName = createdPayment.PayerName,
                PayerIdentification = createdPayment.PayerIdentification
            }, cancellationToken);
        }

        return Result<PaymentDto>.Success(paymentDto);
    }

    public async Task<Result<PaymentDto>> ProcessDirectPaymentAsync(ProcessDirectPaymentRequest request, CancellationToken cancellationToken = default)
    {
        Order? order = null;

        // Find order by OrderCode (OrderNumber)
        if (!string.IsNullOrWhiteSpace(request.OrderCode))
        {
            order = await _orderRepository.GetByOrderNumberAsync(request.OrderCode, cancellationToken);
        }

        if (order == null)
            return Result<PaymentDto>.Failure("Pedido no encontrado. Verifique el código del pedido.");

        if (order.Status == OrderStatus.Cancelled)
            return Result<PaymentDto>.Failure("No se puede procesar pago de un pedido cancelado");

        if (order.PaymentStatus == PaymentStatus.Paid)
            return Result<PaymentDto>.Failure("Este pedido ya está completamente pagado");

        if (request.Amount <= 0)
            return Result<PaymentDto>.Failure("El monto debe ser mayor a cero");

        // Delegate to standard payment flow
        var processRequest = new ProcessOrderPaymentRequest(
            OrderId: order.Id,
            Amount: request.Amount,
            Method: request.Method,
            PayerName: null,
            PayerIdentification: request.PayerIdentification,
            GatewayTransactionId: null);

        return await ProcessPaymentAsync(processRequest, cancellationToken);
    }

    public async Task<Result<PaymentDto>> ProcessSplitPaymentAsync(SplitPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            return Result<PaymentDto>.Failure("Pedido no encontrado");

        if (order.Status == OrderStatus.Cancelled)
            return Result<PaymentDto>.Failure("No se puede procesar pago de un pedido cancelado");

        if (order.PaymentStatus == PaymentStatus.Paid)
            return Result<PaymentDto>.Failure("Este pedido ya está completamente pagado");

        if (request.Items == null || !request.Items.Any())
            return Result<PaymentDto>.Failure("El pago dividido debe tener al menos un item");

        // Validate that all order items exist
        foreach (var item in request.Items)
        {
            var orderItem = order.Items.FirstOrDefault(i => i.Id == item.OrderItemId);
            if (orderItem == null)
                return Result<PaymentDto>.Failure($"Item de pedido no encontrado: {item.OrderItemId}");

            if (item.Amount <= 0)
                return Result<PaymentDto>.Failure("El monto de cada item debe ser mayor a cero");
        }

        var totalAmount = request.Items.Sum(i => i.Amount);
        var method = (PaymentMethodType)request.Method;

        // Process through gateway
        var gateway = GetGatewayForMethod(method);

        var gatewayRequest = new PaymentGatewayRequest(
            Amount: totalAmount,
            Currency: "USD",
            Description: $"Pago dividido pedido {order.OrderNumber}",
            PayerName: request.PayerName,
            PayerEmail: null,
            PayerIdentification: request.PayerIdentification,
            Metadata: new Dictionary<string, string>
            {
                { "OrderId", order.Id.ToString() },
                { "OrderNumber", order.OrderNumber },
                { "SplitPayment", "true" }
            });

        var gatewayResult = await gateway.ProcessPaymentAsync(gatewayRequest, cancellationToken);

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        // Create payment with items
        var payment = new Payment
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderId = order.Id,
            Amount = totalAmount,
            Method = method,
            Status = gatewayResult.IsSuccess ? PaymentTransactionStatus.Completed : PaymentTransactionStatus.Failed,
            GatewayTransactionId = gatewayResult.TransactionId,
            GatewayResponse = gatewayResult.RawResponse,
            PayerName = request.PayerName,
            PayerIdentification = request.PayerIdentification,
            ProcessedAt = gatewayResult.IsSuccess ? DateTime.UtcNow : null,
            FailureReason = gatewayResult.ErrorMessage,
            CreatedAt = DateTime.UtcNow
        };

        // Add split payment items
        foreach (var itemRequest in request.Items)
        {
            var paymentItem = new PaymentItem
            {
                Id = Guid.NewGuid(),
                PaymentId = payment.Id,
                OrderItemId = itemRequest.OrderItemId,
                Amount = itemRequest.Amount,
                CreatedAt = DateTime.UtcNow
            };
            payment.Items.Add(paymentItem);
        }

        var createdPayment = await _paymentRepository.AddAsync(payment, cancellationToken);

        if (gatewayResult.IsSuccess)
        {
            await UpdateOrderPaymentStatus(order, cancellationToken);
        }

        _logger.LogInformation(
            "Pago dividido {PaymentId} procesado para pedido {OrderNumber}: {Status} ({ItemCount} items)",
            createdPayment.Id, order.OrderNumber, createdPayment.Status, payment.Items.Count);

        return Result<PaymentDto>.Success(ToDto(createdPayment));
    }

    public async Task<Result<PaymentDto>> RefundPaymentAsync(Guid paymentId, RefundPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId, cancellationToken);
        if (payment == null)
            return Result<PaymentDto>.Failure("Pago no encontrado");

        if (payment.Status != PaymentTransactionStatus.Completed)
            return Result<PaymentDto>.Failure("Solo se pueden reembolsar pagos completados");

        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<PaymentDto>.Failure("Debe proporcionar un motivo para el reembolso");

        // Process refund through gateway
        var gateway = GetGatewayForMethod(payment.Method);
        var gatewayResult = await gateway.RefundPaymentAsync(
            payment.GatewayTransactionId ?? string.Empty,
            payment.Amount,
            cancellationToken);

        if (!gatewayResult.IsSuccess)
            return Result<PaymentDto>.Failure($"Error al procesar reembolso: {gatewayResult.ErrorMessage}");

        // Update payment status
        payment.Status = PaymentTransactionStatus.Refunded;
        payment.FailureReason = $"Reembolso: {request.Reason}";
        payment.UpdatedAt = DateTime.UtcNow;

        await _paymentRepository.UpdateAsync(payment, cancellationToken);

        // Recalculate order payment status
        var order = await _orderRepository.GetByIdAsync(payment.OrderId, cancellationToken);
        if (order != null)
        {
            await UpdateOrderPaymentStatus(order, cancellationToken);
        }

        _logger.LogInformation(
            "Pago {PaymentId} reembolsado para pedido {OrderId}. Motivo: {Reason}",
            paymentId, payment.OrderId, request.Reason);

        return Result<PaymentDto>.Success(ToDto(payment));
    }

    #region Private Methods

    private IPaymentGateway GetGatewayForMethod(PaymentMethodType method)
    {
        // Cash and Transfer use manual gateway (immediate approval)
        return method switch
        {
            PaymentMethodType.Cash => _gatewayFactory.GetGateway("Manual"),
            PaymentMethodType.Transfer => _gatewayFactory.GetGateway("Manual"),
            PaymentMethodType.Other => _gatewayFactory.GetGateway("Manual"),
            // For card payments and digital wallets, use default (Manual for now, configurable later)
            _ => _gatewayFactory.GetDefaultGateway()
        };
    }

    private async Task UpdateOrderPaymentStatus(Order order, CancellationToken cancellationToken)
    {
        var payments = await _paymentRepository.GetByOrderIdAsync(order.Id, cancellationToken);
        var completedPayments = payments.Where(p => p.Status == PaymentTransactionStatus.Completed).ToList();
        var totalPaid = completedPayments.Sum(p => p.Amount);

        if (totalPaid >= order.Total)
        {
            order.PaymentStatus = PaymentStatus.Paid;

            // If order is Pending and fully paid, advance to Confirmed
            if (order.Status == OrderStatus.Pending)
            {
                order.Status = OrderStatus.Confirmed;
            }
        }
        else if (totalPaid > 0)
        {
            order.PaymentStatus = PaymentStatus.PartiallyPaid;
        }
        else
        {
            order.PaymentStatus = PaymentStatus.Unpaid;
        }

        order.UpdatedAt = DateTime.UtcNow;
        await _orderRepository.UpdateAsync(order, cancellationToken);
    }

    private static PaymentDto ToDto(Payment payment)
    {
        return new PaymentDto(
            Id: payment.Id,
            OrderId: payment.OrderId,
            OrderNumber: payment.Order?.OrderNumber ?? string.Empty,
            InvoiceId: payment.InvoiceId,
            Amount: payment.Amount,
            Method: payment.Method.ToString(),
            Status: payment.Status.ToString(),
            GatewayTransactionId: payment.GatewayTransactionId,
            PayerName: payment.PayerName,
            PayerIdentification: payment.PayerIdentification,
            ProcessedAt: payment.ProcessedAt,
            FailureReason: payment.FailureReason,
            Items: payment.Items.Select(i => new PaymentItemDto(
                Id: i.Id,
                OrderItemId: i.OrderItemId,
                ProductName: i.OrderItem?.Product?.Name ?? string.Empty,
                Amount: i.Amount)),
            CreatedAt: payment.CreatedAt);
    }

    #endregion
}
