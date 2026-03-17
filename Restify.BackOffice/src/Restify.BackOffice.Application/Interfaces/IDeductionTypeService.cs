using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeductionTypeService
{
    Task<Result<DeductionTypeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeductionTypeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<DeductionTypeDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Result<DeductionTypeDto>> CreateAsync(CreateDeductionTypeRequest request, CancellationToken cancellationToken = default);
    Task<Result<DeductionTypeDto>> UpdateAsync(Guid id, UpdateDeductionTypeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
