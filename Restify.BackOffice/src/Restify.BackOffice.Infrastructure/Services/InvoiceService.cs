using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUserService _currentUserService;

    public InvoiceService(
        IInvoiceRepository invoiceRepository,
        IOrderRepository orderRepository,
        ICurrentUserService currentUserService)
    {
        _invoiceRepository = invoiceRepository;
        _orderRepository = orderRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<InvoiceDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        if (invoice == null)
            return Result<InvoiceDto>.Failure("Factura no encontrada");

        return Result<InvoiceDto>.Success(invoice.ToDto());
    }

    public async Task<Result<InvoiceDto>> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByInvoiceNumberAsync(invoiceNumber, cancellationToken);

        if (invoice == null)
            return Result<InvoiceDto>.Failure("Factura no encontrada");

        return Result<InvoiceDto>.Success(invoice.ToDto());
    }

    public async Task<Result<InvoiceDto>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByOrderIdAsync(orderId, cancellationToken);

        if (invoice == null)
            return Result<InvoiceDto>.Failure("No se encontró factura para este pedido");

        return Result<InvoiceDto>.Success(invoice.ToDto());
    }

    public async Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetAllAsync(cancellationToken);
        var dtos = invoices.Select(i => i.ToSummaryDto());

        return Result<IEnumerable<InvoiceSummaryDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<InvoiceSummaryDto>>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetByStatusAsync(status, cancellationToken);
        var dtos = invoices.Select(i => i.ToSummaryDto());

        return Result<IEnumerable<InvoiceSummaryDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<InvoiceSummaryDto>>> GetByPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetByPaymentMethodAsync(paymentMethod, cancellationToken);
        var dtos = invoices.Select(i => i.ToSummaryDto());

        return Result<IEnumerable<InvoiceSummaryDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<InvoiceSummaryDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var invoices = await _invoiceRepository.GetByDateRangeAsync(from, to, cancellationToken);
        var dtos = invoices.Select(i => i.ToSummaryDto());

        return Result<IEnumerable<InvoiceSummaryDto>>.Success(dtos);
    }

    public async Task<Result<InvoiceDto>> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        // Validations
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order == null)
            return Result<InvoiceDto>.Failure("Pedido no encontrado");

        // Check if order already has an invoice
        var existingInvoice = await _invoiceRepository.GetByOrderIdAsync(request.OrderId, cancellationToken);
        if (existingInvoice != null)
            return Result<InvoiceDto>.Failure("Este pedido ya tiene una factura");

        // Check if order is in a valid state for invoicing
        if (order.Status != OrderStatus.Ready && order.Status != OrderStatus.Served && order.Status != OrderStatus.Completed)
            return Result<InvoiceDto>.Failure("El pedido debe estar listo o servido para facturar");

        if (!order.Items.Any())
            return Result<InvoiceDto>.Failure("El pedido no tiene items");

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var issuedBy = _currentUserService.Email ?? "Sistema";

        // Generate invoice number
        var invoiceNumber = await _invoiceRepository.GenerateInvoiceNumberAsync(cancellationToken);

        // Create invoice
        var invoice = request.ToEntity(order, invoiceNumber, tenantId, issuedBy);
        invoice.RecalculateTotals();

        var createdInvoice = await _invoiceRepository.CreateAsync(invoice, cancellationToken);

        // Update order status to Completed if not already
        if (order.Status != OrderStatus.Completed)
        {
            order.Status = OrderStatus.Completed;
            order.CompletedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order, cancellationToken);
        }

        return Result<InvoiceDto>.Success(createdInvoice.ToDto());
    }

    public async Task<Result<InvoiceDto>> UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        if (invoice == null)
            return Result<InvoiceDto>.Failure("Factura no encontrada");

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<InvoiceDto>.Failure("No se puede modificar una factura pagada");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Result<InvoiceDto>.Failure("No se puede modificar una factura anulada");

        invoice.Update(request);

        var updatedInvoice = await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        return Result<InvoiceDto>.Success(updatedInvoice.ToDto());
    }

    public async Task<Result<InvoiceDto>> ProcessPaymentAsync(Guid id, ProcessPaymentRequest request, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        if (invoice == null)
            return Result<InvoiceDto>.Failure("Factura no encontrada");

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<InvoiceDto>.Failure("Esta factura ya está pagada");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Result<InvoiceDto>.Failure("No se puede procesar el pago de una factura anulada");

        // Validate amount if provided
        if (request.AmountPaid.HasValue && request.AmountPaid.Value != invoice.Total)
            return Result<InvoiceDto>.Failure($"El monto pagado no coincide con el total de la factura (Esperado: {invoice.Total}, Recibido: {request.AmountPaid.Value})");

        invoice.ProcessPayment(request);

        var updatedInvoice = await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        return Result<InvoiceDto>.Success(updatedInvoice.ToDto());
    }

    public async Task<Result<InvoiceDto>> CancelAsync(Guid id, CancelInvoiceRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
            return Result<InvoiceDto>.Failure("Debe proporcionar un motivo para anular la factura");

        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        if (invoice == null)
            return Result<InvoiceDto>.Failure("Factura no encontrada");

        if (invoice.Status == InvoiceStatus.Cancelled)
            return Result<InvoiceDto>.Failure("Esta factura ya está anulada");

        if (invoice.Status == InvoiceStatus.Refunded)
            return Result<InvoiceDto>.Failure("No se puede anular una factura reembolsada");

        invoice.Cancel(request);

        var updatedInvoice = await _invoiceRepository.UpdateAsync(invoice, cancellationToken);

        return Result<InvoiceDto>.Success(updatedInvoice.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(id, cancellationToken);

        if (invoice == null)
            return Result<bool>.Failure("Factura no encontrada");

        if (invoice.Status == InvoiceStatus.Paid)
            return Result<bool>.Failure("No se puede eliminar una factura pagada. Debe anularla primero.");

        var result = await _invoiceRepository.DeleteAsync(id, cancellationToken);

        return Result<bool>.Success(result);
    }

    public async Task<Result<InvoiceStatisticsDto>> GetStatisticsAsync(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        from ??= DateTime.UtcNow.Date;
        to ??= DateTime.UtcNow.Date.AddDays(1).AddSeconds(-1);

        var invoices = await _invoiceRepository.GetByDateRangeAsync(from.Value, to.Value, cancellationToken);

        // Only count paid invoices for statistics
        var paidInvoices = invoices.Where(i => i.Status == InvoiceStatus.Paid).ToList();

        var stats = new InvoiceStatisticsDto
        {
            TotalInvoices = paidInvoices.Count,
            TotalRevenue = paidInvoices.Sum(i => i.Total),
            AverageTicket = paidInvoices.Any() ? paidInvoices.Average(i => i.Total) : 0,
            PaymentMethodBreakdown = paidInvoices
                .GroupBy(i => i.PaymentMethod)
                .Select(g => new PaymentMethodStats
                {
                    Method = g.Key.ToString(),
                    Count = g.Count(),
                    Total = g.Sum(i => i.Total)
                })
                .ToList()
        };

        return Result<InvoiceStatisticsDto>.Success(stats);
    }
}
