using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IPayrollPeriodRepository
{
    Task<PayrollPeriod?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PayrollPeriod>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PayrollPeriod> AddAsync(PayrollPeriod entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(PayrollPeriod entity, CancellationToken cancellationToken = default);
}
