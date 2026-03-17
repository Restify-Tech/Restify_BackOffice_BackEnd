using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ITableRepository _tableRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IOrderNotificationService _notificationService;

    public OrderService(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ITableRepository tableRepository,
        ICurrentUserService currentUserService,
        IOrderNotificationService notificationService)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _tableRepository = tableRepository;
        _currentUserService = currentUserService;
        _notificationService = notificationService;
    }

    public async Task<Result<OrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);

        if (order == null)
            return Result<OrderDto>.Failure("Pedido no encontrado");

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<OrderDto>> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber, cancellationToken);

        if (order == null)
            return Result<OrderDto>.Failure("Pedido no encontrado");

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);
        var dtos = orders.Select(o => o.ToDto());

        return Result<IEnumerable<OrderDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByStatusAsync(status, cancellationToken);
        var dtos = orders.Select(o => o.ToDto());

        return Result<IEnumerable<OrderDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetByTableIdAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByTableIdAsync(tableId, cancellationToken);
        var dtos = orders.Select(o => o.ToDto());

        return Result<IEnumerable<OrderDto>>.Success(dtos);
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetActiveOrdersAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetActiveOrdersAsync(cancellationToken);
        var dtos = orders.Select(o => o.ToDto());

        return Result<IEnumerable<OrderDto>>.Success(dtos);
    }

    public async Task<Result<OrderDto>> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        // Validaciones
        if (request.Items == null || !request.Items.Any())
            return Result<OrderDto>.Failure("El pedido debe tener al menos un item");

        if (request.TableId.HasValue)
        {
            var table = await _tableRepository.GetByIdAsync(request.TableId.Value, cancellationToken);
            if (table == null)
                return Result<OrderDto>.Failure("Mesa no encontrada");
        }

        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        var takenBy = _currentUserService.Email ?? "Sistema";

        // Generar número de pedido
        var orderNumber = await _orderRepository.GenerateOrderNumberAsync(cancellationToken);

        // Crear orden
        var order = request.ToEntity(orderNumber, tenantId, takenBy);

        // Crear items
        foreach (var itemRequest in request.Items)
        {
            var product = await _productRepository.GetByIdAsync(itemRequest.ProductId, cancellationToken);
            if (product == null)
                return Result<OrderDto>.Failure($"Producto no encontrado: {itemRequest.ProductId}");

            if (!product.IsAvailable)
                return Result<OrderDto>.Failure($"Producto no disponible: {product.Name}");

            var item = itemRequest.ToEntity(order.Id, tenantId, product);
            order.Items.Add(item);
        }

        // Calcular totales
        order.RecalculateTotals();

        // Si es para comer en el restaurante, actualizar el estado de la mesa
        if (request.TableId.HasValue)
        {
            var table = await _tableRepository.GetByIdAsync(request.TableId.Value, cancellationToken);
            if (table != null)
            {
                table.UpdateStatus(new UpdateTableStatusRequest
                {
                    Status = TableStatus.Occupied,
                    CurrentCustomerName = request.CustomerName,
                    CurrentOrderId = order.Id
                });
                await _tableRepository.UpdateAsync(table, cancellationToken);
            }
        }

        var created = await _orderRepository.CreateAsync(order, cancellationToken);
        var dto = created.ToDto();

        // Notify real-time clients
        await _notificationService.NotifyOrderCreatedAsync(tenantId, dto, cancellationToken);

        return Result<OrderDto>.Success(dto);
    }

    public async Task<Result<OrderDto>> UpdateStatusAsync(Guid id, UpdateOrderStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return Result<OrderDto>.Failure("Pedido no encontrado");

        // No permitir cambiar estado de pedidos completados o cancelados
        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            return Result<OrderDto>.Failure("No se puede modificar un pedido completado o cancelado");

        order.UpdateStatus(request);

        // Si se completa o cancela, liberar la mesa
        if ((request.Status == OrderStatus.Completed || request.Status == OrderStatus.Cancelled) && order.TableId.HasValue)
        {
            var table = await _tableRepository.GetByIdAsync(order.TableId.Value, cancellationToken);
            if (table != null && table.CurrentOrderId == order.Id)
            {
                table.UpdateStatus(new UpdateTableStatusRequest
                {
                    Status = TableStatus.Cleaning
                });
                await _tableRepository.UpdateAsync(table, cancellationToken);
            }
        }

        var updated = await _orderRepository.UpdateAsync(order, cancellationToken);
        var dto = updated.ToDto();

        // Notify real-time clients
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        await _notificationService.NotifyOrderStatusChangedAsync(tenantId, dto, cancellationToken);

        return Result<OrderDto>.Success(dto);
    }

    public async Task<Result<OrderDto>> UpdateItemStatusAsync(Guid orderId, Guid itemId, UpdateOrderItemStatusRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(orderId, cancellationToken);
        if (order == null)
            return Result<OrderDto>.Failure("Pedido no encontrado");

        var item = order.Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            return Result<OrderDto>.Failure("Item no encontrado");

        item.Status = request.Status;
        item.UpdatedAt = DateTime.UtcNow;

        // Si todos los items están listos, actualizar el estado de la orden
        if (order.Items.All(i => i.Status == OrderItemStatus.Ready) && order.Status == OrderStatus.Preparing)
        {
            order.Status = OrderStatus.Ready;
        }

        var updated = await _orderRepository.UpdateAsync(order, cancellationToken);
        var dto = updated.ToDto();

        // Notify real-time clients
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;
        await _notificationService.NotifyOrderItemStatusChangedAsync(tenantId, dto, itemId, cancellationToken);

        // If all items ready caused order status change, also notify that
        if (order.Status == OrderStatus.Ready)
        {
            await _notificationService.NotifyOrderStatusChangedAsync(tenantId, dto, cancellationToken);
        }

        return Result<OrderDto>.Success(dto);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return Result<bool>.Failure("Pedido no encontrado");

        // Solo permitir eliminar pedidos pendientes
        if (order.Status != OrderStatus.Pending)
            return Result<bool>.Failure("Solo se pueden eliminar pedidos pendientes");

        await _orderRepository.DeleteAsync(id, cancellationToken);

        return Result<bool>.Success(true);
    }
}
