using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IDeliveryRepository
{
    Task<Delivery?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Delivery?> GetByOrderIdAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetByDriverIdAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetByStatusAsync(DeliveryStatus status, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetPendingAsync(CancellationToken cancellationToken = default);
    Task<Delivery?> GetActiveByDriverAsync(Guid driverId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Delivery>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Delivery> AddAsync(Delivery entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Delivery entity, CancellationToken cancellationToken = default);
}
