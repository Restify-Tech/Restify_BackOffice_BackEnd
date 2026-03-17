using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ISupplierService
{
    Task<Result<List<SupplierDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<SupplierDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<SupplierDto>> CreateAsync(CreateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result<SupplierDto>> UpdateAsync(Guid id, UpdateSupplierRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
