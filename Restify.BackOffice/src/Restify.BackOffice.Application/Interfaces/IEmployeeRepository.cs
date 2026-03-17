using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Employee>> GetActiveAsync(CancellationToken cancellationToken = default);
    Task<Employee?> GetByIdentificationAsync(string identificationNumber, CancellationToken cancellationToken = default);
    Task<Employee> AddAsync(Employee entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Employee entity, CancellationToken cancellationToken = default);
}
