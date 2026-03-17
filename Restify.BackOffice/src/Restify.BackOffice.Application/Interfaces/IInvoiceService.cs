using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IInvoiceService
{
    Task<Result<InvoiceDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<InvoiceSummaryDto>>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<InvoiceSummaryDto>>> GetByPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<InvoiceSummaryDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> UpdateAsync(Guid id, UpdateInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> ProcessPaymentAsync(Guid id, ProcessPaymentRequest request, CancellationToken cancellationToken = default);
    Task<Result<InvoiceDto>> CancelAsync(Guid id, CancelInvoiceRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<InvoiceStatisticsDto>> GetStatisticsAsync(DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
}
