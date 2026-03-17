using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IJournalEntryRepository
{
    Task<JournalEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<JournalEntry>> GetByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default);
    Task<string> GetNextEntryNumberAsync(CancellationToken cancellationToken = default);
    Task<JournalEntry> AddAsync(JournalEntry entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(JournalEntry entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(JournalEntry entity, CancellationToken cancellationToken = default);
}
