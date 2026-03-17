using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAccountingAccountRepository
{
    Task<AccountingAccount?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AccountingAccount>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AccountingAccount?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task<AccountingAccount> AddAsync(AccountingAccount entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(AccountingAccount entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(AccountingAccount entity, CancellationToken cancellationToken = default);
}
