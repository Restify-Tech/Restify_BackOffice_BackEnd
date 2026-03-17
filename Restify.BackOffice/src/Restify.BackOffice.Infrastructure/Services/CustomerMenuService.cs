using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Infrastructure.Services;

public class CustomerMenuService : ICustomerMenuService
{
    private readonly BackOfficeDbContext _context;
    private readonly ILogger<CustomerMenuService> _logger;

    public CustomerMenuService(
        BackOfficeDbContext context,
        ILogger<CustomerMenuService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<MenuCategoryDto>>> GetCategoriesAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories
            .IgnoreQueryFilters()
            .Where(c => c.TenantId == tenantId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync(cancellationToken);

        var productCounts = await _context.Products
            .IgnoreQueryFilters()
            .Where(p => p.TenantId == tenantId && p.IsActive && p.IsAvailable)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { CategoryId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var result = categories.Select(c => new MenuCategoryDto(
            c.Id,
            c.Name,
            c.Description,
            c.Icon,
            c.ImageUrl,
            c.DisplayOrder,
            productCounts.FirstOrDefault(pc => pc.CategoryId == c.Id)?.Count ?? 0
        ));

        return Result<IEnumerable<MenuCategoryDto>>.Success(result);
    }

    public async Task<Result<IEnumerable<MenuProductDto>>> GetProductsAsync(Guid tenantId, Guid? categoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .Where(p => p.TenantId == tenantId && p.IsActive && p.IsAvailable);

        if (categoryId.HasValue)
            query = query.Where(p => p.CategoryId == categoryId.Value);

        var products = await query
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(cancellationToken);

        var dtos = products.Select(ToMenuProductDto);
        return Result<IEnumerable<MenuProductDto>>.Success(dtos);
    }

    public async Task<Result<MenuProductDto>> GetProductByIdAsync(Guid tenantId, Guid productId, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .IgnoreQueryFilters()
            .Include(p => p.Category)
            .Include(p => p.Modifiers)
            .FirstOrDefaultAsync(p => p.Id == productId && p.TenantId == tenantId && p.IsActive, cancellationToken);

        if (product == null)
            return Result<MenuProductDto>.Failure("Producto no encontrado");

        return Result<MenuProductDto>.Success(ToMenuProductDto(product));
    }

    public async Task<Result<CustomerOrderStatusDto>> CreateOrderAsync(Guid tenantId, CustomerCreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Items == null || !request.Items.Any())
            return Result<CustomerOrderStatusDto>.Failure("El pedido debe tener al menos un item");

        var orderType = (OrderType)request.Type;
        if (orderType == OrderType.DineIn && request.TableId == null)
            return Result<CustomerOrderStatusDto>.Failure("Se requiere una mesa para pedidos en el restaurante");

        if (request.TableId.HasValue)
        {
            var table = await _context.Tables
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(t => t.Id == request.TableId.Value && t.TenantId == tenantId, cancellationToken);

            if (table == null)
                return Result<CustomerOrderStatusDto>.Failure("Mesa no encontrada");
        }

        // Validate products
        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var products = await _context.Products
            .IgnoreQueryFilters()
            .Where(p => productIds.Contains(p.Id) && p.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        if (products.Count != productIds.Count)
            return Result<CustomerOrderStatusDto>.Failure("Uno o mas productos no fueron encontrados");

        var unavailable = products.Where(p => !p.IsAvailable || !p.IsActive).ToList();
        if (unavailable.Any())
            return Result<CustomerOrderStatusDto>.Failure($"Producto no disponible: {unavailable.First().Name}");

        var orderNumber = await GenerateOrderNumberAsync(tenantId, cancellationToken);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            OrderNumber = orderNumber,
            Type = orderType,
            Status = OrderStatus.Pending,
            PaymentStatus = PaymentStatus.Unpaid,
            CustomerName = request.CustomerName,
            CustomerPhone = request.CustomerPhone,
            CustomerId = request.CustomerId,
            TableId = request.TableId,
            Notes = request.Notes,
            CreatedAt = DateTime.UtcNow
        };

        decimal subtotal = 0;

        foreach (var itemReq in request.Items)
        {
            var product = products.First(p => p.Id == itemReq.ProductId);
            var itemSubtotal = product.Price * itemReq.Quantity;

            var orderItem = new OrderItem
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                OrderId = order.Id,
                ProductId = product.Id,
                Quantity = itemReq.Quantity,
                UnitPrice = product.Price,
                Subtotal = itemSubtotal,
                Status = OrderItemStatus.Pending,
                Notes = itemReq.Notes,
                CreatedAt = DateTime.UtcNow
            };

            if (itemReq.Modifiers != null)
            {
                foreach (var modReq in itemReq.Modifiers)
                {
                    orderItem.Modifiers.Add(new OrderItemModifier
                    {
                        Id = Guid.NewGuid(),
                        OrderItemId = orderItem.Id,
                        ModifierName = modReq.ModifierName,
                        PriceAdjustment = modReq.PriceAdjustment,
                        CreatedAt = DateTime.UtcNow
                    });
                    itemSubtotal += modReq.PriceAdjustment * itemReq.Quantity;
                    orderItem.Subtotal = itemSubtotal;
                }
            }

            order.Items.Add(orderItem);
            subtotal += itemSubtotal;
        }

        order.Subtotal = subtotal;
        order.Tax = 0;
        order.Discount = 0;
        order.Total = subtotal;

        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Customer order {OrderNumber} created for tenant {TenantId} with {ItemCount} items, total {Total}",
            order.OrderNumber, tenantId, order.Items.Count, order.Total);

        return Result<CustomerOrderStatusDto>.Success(ToOrderStatusDto(order, products));
    }

    public async Task<Result<CustomerOrderStatusDto>> GetOrderStatusAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders
            .IgnoreQueryFilters()
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Table)
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken);

        if (order == null)
            return Result<CustomerOrderStatusDto>.Failure("Pedido no encontrado");

        var dto = new CustomerOrderStatusDto(
            order.Id,
            order.OrderNumber,
            order.Status.ToString(),
            order.PaymentStatus.ToString(),
            order.Table?.Number,
            order.Items.Select(i => new CustomerOrderItemStatusDto(
                i.Id, i.Product.Name, i.Quantity, i.Status.ToString()
            )),
            order.Total,
            order.CreatedAt,
            order.UpdatedAt);

        return Result<CustomerOrderStatusDto>.Success(dto);
    }

    public async Task<Result<IEnumerable<CustomerOrderStatusDto>>> GetCustomerOrdersAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        var orders = await _context.Orders
            .IgnoreQueryFilters()
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Table)
            .Where(o => o.CustomerId == customerId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);

        var dtos = orders.Select(o => new CustomerOrderStatusDto(
            o.Id,
            o.OrderNumber,
            o.Status.ToString(),
            o.PaymentStatus.ToString(),
            o.Table?.Number,
            o.Items.Select(i => new CustomerOrderItemStatusDto(
                i.Id, i.Product.Name, i.Quantity, i.Status.ToString()
            )),
            o.Total,
            o.CreatedAt,
            o.UpdatedAt));

        return Result<IEnumerable<CustomerOrderStatusDto>>.Success(dtos);
    }

    #region Private Methods

    private async Task<string> GenerateOrderNumberAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _context.Orders
            .IgnoreQueryFilters()
            .Where(o => o.TenantId == tenantId && o.CreatedAt.Date == DateTime.UtcNow.Date)
            .CountAsync(cancellationToken);

        return $"ORD-{today}-{(count + 1):D4}";
    }

    private static MenuProductDto ToMenuProductDto(Product product)
    {
        return new MenuProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.ImageUrl,
            product.Price,
            product.IsAvailable,
            product.DisplayOrder,
            product.CategoryId,
            product.Category.Name,
            product.Modifiers.Where(m => m.IsActive).Select(m => new MenuProductModifierDto(
                m.Id, m.Name, m.Description, m.PriceAdjustment, m.IsRequired
            ))
        );
    }

    private static CustomerOrderStatusDto ToOrderStatusDto(Order order, List<Product> products)
    {
        return new CustomerOrderStatusDto(
            order.Id,
            order.OrderNumber,
            order.Status.ToString(),
            order.PaymentStatus.ToString(),
            null,
            order.Items.Select(i =>
            {
                var product = products.FirstOrDefault(p => p.Id == i.ProductId);
                return new CustomerOrderItemStatusDto(
                    i.Id,
                    product?.Name ?? "Producto",
                    i.Quantity,
                    i.Status.ToString());
            }),
            order.Total,
            order.CreatedAt,
            order.UpdatedAt);
    }

    #endregion
}
