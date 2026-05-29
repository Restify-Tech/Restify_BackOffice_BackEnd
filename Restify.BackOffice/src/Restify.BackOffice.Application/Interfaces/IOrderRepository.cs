using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

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
    Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(GetOrdersQuery query, CancellationToken cancellationToken = default);
    Task<OrderStatisticsDto> GetStatisticsAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<OrderTodayStatsDto> GetTodayStatsAsync(CancellationToken cancellationToken = default);
}
