using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPayrollPeriodService
{
    Task<Result<PayrollPeriodDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<PayrollPeriodDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<PayrollPeriodDto>> CreateAsync(CreatePayrollPeriodRequest request, CancellationToken cancellationToken = default);
    Task<Result<PayrollPeriodDto>> CalculatePayrollAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<PayrollPeriodDto>> ApproveAsync(Guid id, string approvedBy, CancellationToken cancellationToken = default);
    Task<Result<PayrollPeriodDto>> MarkPaidAsync(Guid id, string paidBy, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
