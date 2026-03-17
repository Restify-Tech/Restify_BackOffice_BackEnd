using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPayrollEntryRepository
{
    Task<IReadOnlyList<PayrollEntry>> GetByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default);
    Task CreateRangeAsync(IEnumerable<PayrollEntry> entries, CancellationToken cancellationToken = default);
    Task DeleteByPeriodIdAsync(Guid periodId, CancellationToken cancellationToken = default);
}
