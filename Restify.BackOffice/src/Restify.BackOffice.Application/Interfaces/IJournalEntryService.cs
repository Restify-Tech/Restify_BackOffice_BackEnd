using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IJournalEntryService
{
    Task<Result<JournalEntryDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<JournalEntryDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<JournalEntryDto>>> GetByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default);
    Task<Result<JournalEntryDto>> CreateAsync(CreateJournalEntryRequest request, CancellationToken cancellationToken = default);
    Task<Result<JournalEntryDto>> PostAsync(Guid id, string postedBy, CancellationToken cancellationToken = default);
    Task<Result<JournalEntryDto>> ReverseAsync(Guid id, string reversedBy, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
