using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAccountingPeriodService
{
    Task<Result<AccountingPeriodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AccountingPeriodDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AccountingPeriodDto>> CreateAsync(CreateAccountingPeriodRequest request, CancellationToken cancellationToken = default);
    Task<Result<AccountingPeriodDto>> CloseAsync(Guid id, string closedBy, CancellationToken cancellationToken = default);
    Task<Result<AccountingPeriodDto>> LockAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AccountingPeriodDto>> ReopenAsync(Guid id, CancellationToken cancellationToken = default);
}
