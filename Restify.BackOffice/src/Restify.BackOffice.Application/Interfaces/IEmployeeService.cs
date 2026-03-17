using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IEmployeeService
{
    Task<Result<EmployeeDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<EmployeeDto>>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> UpdateAsync(Guid id, UpdateEmployeeRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> ActivateAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<EmployeeDto>> DeactivateAsync(Guid id, CancellationToken cancellationToken = default);
}
