using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class TransferApprovalService : ITransferApprovalService
{
    private readonly ITransferPaymentRequestRepository _repository;
    private readonly IOrderRepository _orderRepository;
    private readonly IPaymentService _paymentService;
    private readonly IOrderNotificationService _notificationService;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<TransferApprovalService> _logger;

    public TransferApprovalService(
        ITransferPaymentRequestRepository repository,
        IOrderRepository orderRepository,
        IPaymentService paymentService,
        IOrderNotificationService notificationService,
        ICurrentUserService currentUserService,
        ILogger<TransferApprovalService> logger)
    {
        _repository = repository;
        _orderRepository = orderRepository;
        _paymentService = paymentService;
        _notificationService = notificationService;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<TransferPaymentRequestDto>> CreateAsync(CreateTransferPaymentRequest request, CancellationToken ct = default)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, ct);
        if (order == null)
            return Result<TransferPaymentRequestDto>.Failure("Pedido no encontrado");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var entity = new TransferPaymentRequest
        {
            TenantId = tenantId,
            OrderId = request.OrderId,
            Amount = request.Amount,
            ImageUrl = request.ImageUrl,
            BankReference = request.BankReference,
            PayerName = request.PayerName,
            PayerIdentification = request.PayerIdentification,
            Status = TransferApprovalStatus.Pending
        };

        var created = await _repository.CreateAsync(entity, ct);

        // Notify cashier via SignalR
        await _notificationService.NotifyTransferPaymentPendingAsync(tenantId, MapToDto(created), ct);

        _logger.LogInformation("Transfer payment request created for order {OrderId}: {Amount}", request.OrderId, request.Amount);

        return Result<TransferPaymentRequestDto>.Success(MapToDto(created));
    }

    public async Task<Result<TransferPaymentRequestDto>> ReviewAsync(Guid id, ReviewTransferPaymentRequest request, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return Result<TransferPaymentRequestDto>.Failure("Solicitud de transferencia no encontrada");

        if (entity.Status != TransferApprovalStatus.Pending)
            return Result<TransferPaymentRequestDto>.Failure("Esta solicitud ya fue revisada");

        var currentUser = _currentUserService.UserId?.ToString() ?? "system";

        if (request.Approved)
        {
            entity.Status = TransferApprovalStatus.Approved;
            entity.ReviewedBy = currentUser;
            entity.ReviewedAt = DateTime.UtcNow;

            // Process the payment through the normal flow
            var paymentRequest = new ProcessOrderPaymentRequest(
                OrderId: entity.OrderId,
                Amount: entity.Amount,
                Method: (int)PaymentMethodType.Transfer,
                PayerName: entity.PayerName,
                PayerIdentification: entity.PayerIdentification,
                GatewayTransactionId: entity.BankReference);

            var paymentResult = await _paymentService.ProcessPaymentAsync(paymentRequest, ct);
            if (!paymentResult.IsSuccess)
            {
                return Result<TransferPaymentRequestDto>.Failure($"Error al procesar el pago: {paymentResult.Error}");
            }

            _logger.LogInformation("Transfer payment request {Id} approved by {User}", id, currentUser);
        }
        else
        {
            entity.Status = TransferApprovalStatus.Rejected;
            entity.ReviewedBy = currentUser;
            entity.ReviewedAt = DateTime.UtcNow;
            entity.RejectionReason = request.RejectionReason;

            _logger.LogInformation("Transfer payment request {Id} rejected by {User}: {Reason}", id, currentUser, request.RejectionReason);
        }

        await _repository.UpdateAsync(entity, ct);
        return Result<TransferPaymentRequestDto>.Success(MapToDto(entity));
    }

    public async Task<Result<TransferPaymentRequestDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _repository.GetByIdAsync(id, ct);
        if (entity == null)
            return Result<TransferPaymentRequestDto>.Failure("Solicitud no encontrada");
        return Result<TransferPaymentRequestDto>.Success(MapToDto(entity));
    }

    public async Task<Result<IEnumerable<TransferPaymentRequestDto>>> GetPendingAsync(CancellationToken ct = default)
    {
        var entities = await _repository.GetPendingAsync(ct);
        return Result<IEnumerable<TransferPaymentRequestDto>>.Success(entities.Select(MapToDto));
    }

    public async Task<Result<IEnumerable<TransferPaymentRequestDto>>> GetByOrderIdAsync(Guid orderId, CancellationToken ct = default)
    {
        var entities = await _repository.GetByOrderIdAsync(orderId, ct);
        return Result<IEnumerable<TransferPaymentRequestDto>>.Success(entities.Select(MapToDto));
    }

    private static TransferPaymentRequestDto MapToDto(TransferPaymentRequest entity)
    {
        return new TransferPaymentRequestDto(
            Id: entity.Id,
            OrderId: entity.OrderId,
            OrderNumber: entity.Order?.OrderNumber ?? string.Empty,
            Amount: entity.Amount,
            ImageUrl: entity.ImageUrl,
            BankReference: entity.BankReference,
            PayerName: entity.PayerName,
            PayerIdentification: entity.PayerIdentification,
            Status: entity.Status.ToString(),
            ReviewedBy: entity.ReviewedBy,
            ReviewedAt: entity.ReviewedAt,
            RejectionReason: entity.RejectionReason,
            CreatedAt: entity.CreatedAt
        );
    }
}
