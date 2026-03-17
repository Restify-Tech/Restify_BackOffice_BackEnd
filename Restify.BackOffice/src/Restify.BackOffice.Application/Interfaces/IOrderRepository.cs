using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetByTableIdAsync(Guid tableId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetActiveOrdersAsync(CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
}
