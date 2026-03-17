using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByStatusAsync(InvoiceStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByPaymentMethodAsync(PaymentMethod paymentMethod, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default);
    Task<Invoice> CreateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice> UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateInvoiceNumberAsync(CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}
