using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeductionTypeRepository
{
    Task<DeductionType?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeductionType>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeductionType>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<DeductionType> AddAsync(DeductionType entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DeductionType entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeductionType entity, CancellationToken cancellationToken = default);
}
