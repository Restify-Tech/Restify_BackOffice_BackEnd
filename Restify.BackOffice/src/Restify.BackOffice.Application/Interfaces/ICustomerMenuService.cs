using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface ICustomerMenuService
{
    Task<Result<IEnumerable<MenuCategoryDto>>> GetCategoriesAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MenuProductDto>>> GetProductsAsync(Guid tenantId, Guid? categoryId = null, CancellationToken cancellationToken = default);
    Task<Result<MenuProductDto>> GetProductByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default);
    Task<Result<CustomerOrderStatusDto>> CreateOrderAsync(Guid tenantId, CustomerCreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<Result<CustomerOrderStatusDto>> GetOrderStatusAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<CustomerOrderStatusDto>>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken = default);
}
