using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAccountingAccountService
{
    Task<Result<AccountingAccountDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AccountingAccountDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AccountingAccountDto>> CreateAsync(CreateAccountingAccountRequest request, CancellationToken cancellationToken = default);
    Task<Result<AccountingAccountDto>> UpdateAsync(Guid id, UpdateAccountingAccountRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
