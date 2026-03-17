using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAccountingPeriodRepository
{
    Task<AccountingPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountingPeriod>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AccountingPeriod?> GetByYearMonthAsync(int year, int month, CancellationToken cancellationToken = default);
    Task<AccountingPeriod> AddAsync(AccountingPeriod entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(AccountingPeriod entity, CancellationToken cancellationToken = default);
}
