using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICreditNoteRepository
{
    Task<CreditNote?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<CreditNote>> GetByInvoiceIdAsync(Guid invoiceId, CancellationToken ct = default);
    Task<IReadOnlyList<CreditNote>> GetAllAsync(CancellationToken ct = default);
    Task<CreditNote> CreateAsync(CreditNote creditNote, CancellationToken ct = default);
    Task UpdateAsync(CreditNote creditNote, CancellationToken ct = default);
}
