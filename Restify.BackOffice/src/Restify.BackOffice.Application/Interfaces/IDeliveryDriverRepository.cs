using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeliveryDriverRepository
{
    Task<DeliveryDriver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryDriver>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DeliveryDriver?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryDriver>> GetByCooperativeIdAsync(Guid cooperativeId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<DeliveryDriver>> GetAvailableDriversAsync(CancellationToken cancellationToken = default);
    Task<DeliveryDriver> AddAsync(DeliveryDriver entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(DeliveryDriver entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(DeliveryDriver entity, CancellationToken cancellationToken = default);
}
