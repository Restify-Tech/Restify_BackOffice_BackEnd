using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderDto>>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderDto>>> GetByTableIdAsync(Guid tableId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<OrderDto>>> GetActiveOrdersAsync(CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<OrderDto>> UpdateItemStatusAsync(Guid orderId, Guid itemId, UpdateOrderItemStatusRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
