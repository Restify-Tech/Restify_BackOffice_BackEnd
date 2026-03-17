using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICustomerService
{
    Task<Result<CustomerDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CustomerListDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<CustomerDto>> CreateAsync(CreateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result<CustomerDto>> UpdateAsync(Guid id, UpdateCustomerRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
