using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class SplitPaymentService : ISplitPaymentService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILogger<SplitPaymentService> _logger;

    public SplitPaymentService(
        BackOfficeDbContext context,
        ICurrentUserService currentUserService,
        ILogger<SplitPaymentService> logger)
    {
        _context = context;
        _currentUserService = currentUserService;
        _logger = logger;
    }

    public async Task<Result<SplitPaymentDto>> InitiateAsync(CreateSplitPaymentRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Validar que el pedido exista (el QueryFilter ya filtra por TenantId)
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);

            if (order == null)
                return Result<SplitPaymentDto>.Failure("Pedido no encontrado");

            // Validar que no exista un split activo para este pedido
            var existingSplit = await _context.SplitPayments
                .FirstOrDefaultAsync(sp =>
                    sp.OrderId == request.OrderId &&
                    sp.Status != SplitPaymentStatus.Cancelled &&
                    sp.Status != SplitPaymentStatus.Completed,
                    cancellationToken);

            if (existingSplit != null)
                return Result<SplitPaymentDto>.Failure("Ya existe un pago dividido activo para este pedido");

            // Validar items
            if (request.Items == null || !request.Items.Any())
                return Result<SplitPaymentDto>.Failure("Se requiere al menos un item de pago");

            var itemsList = request.Items.ToList();
            var totalFromItems = itemsList.Sum(i => i.Amount);

            if (Math.Abs(totalFromItems - order.Total) > 0.01m)
                return Result<SplitPaymentDto>.Failure(
                    $"La suma de los items ({totalFromItems:F2}) no coincide con el total del pedido ({order.Total:F2})");

            var splitPayment = new SplitPayment
            {
                OrderId = request.OrderId,
                TotalAmount = order.Total,
                SplitCount = request.SplitCount,
                Status = SplitPaymentStatus.Pending,
                CreatedByUserId = _currentUserService.UserId?.ToString(),
                Items = itemsList.Select(i => new SplitPaymentItem
                {
                    Amount = i.Amount,
                    PaymentMethod = i.PaymentMethod,
                    PaidBy = i.PaidBy,
                    Status = SplitPaymentItemStatus.Pending
                }).ToList()
            };

            _context.SplitPayments.Add(splitPayment);
            await _context.SaveChangesAsync(cancellationToken);

            return Result<SplitPaymentDto>.Success(MapToDto(splitPayment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar pago dividido para pedido {OrderId}", request.OrderId);
            return Result<SplitPaymentDto>.Failure("Error interno al crear el pago dividido");
        }
    }

    public async Task<Result<SplitPaymentDto>> ProcessItemAsync(ProcessSplitItemRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            var splitPayment = await _context.SplitPayments
                .Include(sp => sp.Items)
                .FirstOrDefaultAsync(sp => sp.Id == request.SplitPaymentId, cancellationToken);

            if (splitPayment == null)
                return Result<SplitPaymentDto>.Failure("Pago dividido no encontrado");

            if (splitPayment.Status == SplitPaymentStatus.Cancelled)
                return Result<SplitPaymentDto>.Failure("El pago dividido esta cancelado");

            if (splitPayment.Status == SplitPaymentStatus.Completed)
                return Result<SplitPaymentDto>.Failure("El pago dividido ya esta completado");

            var items = splitPayment.Items.OrderBy(i => i.CreatedAt).ToList();

            if (request.ItemIndex < 0 || request.ItemIndex >= items.Count)
                return Result<SplitPaymentDto>.Failure($"Indice de item invalido: {request.ItemIndex}");

            var item = items[request.ItemIndex];

            if (item.Status == SplitPaymentItemStatus.Paid)
                return Result<SplitPaymentDto>.Failure("Este item ya fue pagado");

            if (item.Status == SplitPaymentItemStatus.Cancelled)
                return Result<SplitPaymentDto>.Failure("Este item esta cancelado");

            // Marcar item como pagado
            item.Status = SplitPaymentItemStatus.Paid;
            item.PaidAt = DateTime.UtcNow;
            item.PaymentMethod = request.PaymentMethod;
            item.Amount = request.Amount;

            // Si todos los items estan pagados, marcar el split como completado
            var allPaid = splitPayment.Items.All(i => i.Status == SplitPaymentItemStatus.Paid);
            splitPayment.Status = allPaid ? SplitPaymentStatus.Completed : SplitPaymentStatus.PartiallyPaid;

            await _context.SaveChangesAsync(cancellationToken);

            return Result<SplitPaymentDto>.Success(MapToDto(splitPayment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al procesar item {ItemIndex} del pago dividido {SplitPaymentId}",
                request.ItemIndex, request.SplitPaymentId);
            return Result<SplitPaymentDto>.Failure("Error interno al procesar el item del pago dividido");
        }
    }

    public async Task<Result<SplitPaymentDto>> GetByOrderAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        try
        {
            var splitPayment = await _context.SplitPayments
                .Include(sp => sp.Items)
                .Where(sp => sp.OrderId == orderId)
                .OrderByDescending(sp => sp.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (splitPayment == null)
                return Result<SplitPaymentDto>.Failure("No se encontro un pago dividido para este pedido");

            return Result<SplitPaymentDto>.Success(MapToDto(splitPayment));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener pago dividido del pedido {OrderId}", orderId);
            return Result<SplitPaymentDto>.Failure("Error interno al obtener el pago dividido");
        }
    }

    public async Task<Result<bool>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var splitPayment = await _context.SplitPayments
                .FirstOrDefaultAsync(sp => sp.Id == id, cancellationToken);

            if (splitPayment == null)
                return Result<bool>.Failure("Pago dividido no encontrado");

            if (splitPayment.Status == SplitPaymentStatus.Completed)
                return Result<bool>.Failure("No se puede cancelar un pago dividido ya completado");

            if (splitPayment.Status == SplitPaymentStatus.Cancelled)
                return Result<bool>.Failure("El pago dividido ya esta cancelado");

            splitPayment.Status = SplitPaymentStatus.Cancelled;
            await _context.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al cancelar pago dividido {SplitPaymentId}", id);
            return Result<bool>.Failure("Error interno al cancelar el pago dividido");
        }
    }

    private static SplitPaymentDto MapToDto(SplitPayment sp)
    {
        var items = sp.Items
            .OrderBy(i => i.CreatedAt)
            .Select((item, index) => new SplitPaymentItemDto(
                item.Id,
                index,
                item.Amount,
                item.PaymentMethod.ToString(),
                item.PaidBy,
                item.PaidAt,
                item.Status.ToString()))
            .ToList();

        return new SplitPaymentDto(
            sp.Id,
            sp.OrderId,
            sp.TotalAmount,
            sp.SplitCount,
            sp.Status.ToString(),
            sp.CreatedByUserId,
            items,
            sp.CreatedAt);
    }
}
