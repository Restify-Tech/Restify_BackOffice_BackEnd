using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Customer> AddAsync(Customer entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Customer entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Customer entity, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string email, Guid? excludeId = null, CancellationToken cancellationToken = default);
}
