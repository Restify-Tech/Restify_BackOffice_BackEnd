using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class PurchaseOrderService : IPurchaseOrderService
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ICurrentUserService _currentUserService;

    public PurchaseOrderService(
        IPurchaseOrderRepository repository,
        IInventoryRepository inventoryRepository,
        ICurrentUserService currentUserService)
    {
        _repository = repository;
        _inventoryRepository = inventoryRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<PurchaseOrderSummaryDto>>> GetAllAsync(PurchaseOrderStatus? status = null, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(status, cancellationToken);
        return Result<List<PurchaseOrderSummaryDto>>.Success(entities.Select(e => e.ToSummaryDto()).ToList());
    }

    public async Task<Result<PurchaseOrderDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        return entity == null 
            ? Result<PurchaseOrderDto>.Failure("Orden no encontrada") 
            : Result<PurchaseOrderDto>.Success(entity.ToDto());
    }

    public async Task<Result<PurchaseOrderDto>> CreateAsync(CreatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        var orderNumber = await _repository.GenerateOrderNumberAsync(cancellationToken);

        var items = request.Items.Select(i => new PurchaseOrderItem
        {
            ProductId = i.ProductId,
            QuantityOrdered = i.Quantity,
            QuantityReceived = 0,
            Unit = i.Unit,
            UnitCost = i.UnitCost,
            Subtotal = i.Quantity * i.UnitCost,
            Notes = i.Notes
        }).ToList();

        var subtotal = items.Sum(i => i.Subtotal);
        var tax = subtotal * 0.12m;

        var entity = new PurchaseOrder
        {
            OrderNumber = orderNumber,
            SupplierId = request.SupplierId,
            OrderDate = DateTime.UtcNow,
            ExpectedDeliveryDate = request.ExpectedDeliveryDate,
            Status = PurchaseOrderStatus.Draft,
            Items = items,
            Subtotal = subtotal,
            Tax = tax,
            Discount = request.Discount,
            Total = subtotal + tax - request.Discount,
            Notes = request.Notes,
            OrderedBy = _currentUserService.Email
        };

        var created = await _repository.CreateAsync(entity, cancellationToken);
        return Result<PurchaseOrderDto>.Success(created.ToDto());
    }

    public async Task<Result<PurchaseOrderDto>> UpdateAsync(Guid id, UpdatePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<PurchaseOrderDto>.Failure("Orden no encontrada");

        if (entity.Status != PurchaseOrderStatus.Draft)
            return Result<PurchaseOrderDto>.Failure("Solo se pueden editar órdenes en borrador");

        entity.ExpectedDeliveryDate = request.ExpectedDeliveryDate;
        entity.Discount = request.Discount;
        entity.Notes = request.Notes;

        entity.Items.Clear();
        foreach (var item in request.Items)
        {
            entity.Items.Add(new PurchaseOrderItem
            {
                ProductId = item.ProductId,
                QuantityOrdered = item.Quantity,
                Unit = item.Unit,
                UnitCost = item.UnitCost,
                Subtotal = item.Quantity * item.UnitCost,
                Notes = item.Notes
            });
        }

        entity.Subtotal = entity.Items.Sum(i => i.Subtotal);
        entity.Tax = entity.Subtotal * 0.12m;
        entity.Total = entity.Subtotal + entity.Tax - entity.Discount;

        var updated = await _repository.UpdateAsync(entity, cancellationToken);
        return Result<PurchaseOrderDto>.Success(updated.ToDto());
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<bool>.Failure("Orden no encontrada");

        if (entity.Status != PurchaseOrderStatus.Draft)
            return Result<bool>.Failure("Solo se pueden eliminar órdenes en borrador");

        await _repository.DeleteAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PurchaseOrderDto>> SendAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<PurchaseOrderDto>.Failure("Orden no encontrada");

        if (entity.Status != PurchaseOrderStatus.Draft)
            return Result<PurchaseOrderDto>.Failure("Solo se pueden enviar órdenes en borrador");

        entity.Status = PurchaseOrderStatus.Sent;
        var updated = await _repository.UpdateAsync(entity, cancellationToken);
        return Result<PurchaseOrderDto>.Success(updated.ToDto());
    }

    public async Task<Result<PurchaseOrderDto>> ReceiveAsync(Guid id, ReceivePurchaseOrderRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<PurchaseOrderDto>.Failure("Orden no encontrada");

        // Actualizar cantidades recibidas y crear movimientos de inventario
        foreach (var receivedItem in request.Items)
        {
            var item = entity.Items.FirstOrDefault(i => i.Id == receivedItem.ItemId);
            if (item == null) continue;

            item.QuantityReceived += receivedItem.QuantityReceived;

            // Actualizar inventario
            var inventoryItem = await _inventoryRepository.GetItemByProductIdAsync(item.ProductId, cancellationToken);
            if (inventoryItem != null)
            {
                var previousStock = inventoryItem.CurrentStock;
                inventoryItem.CurrentStock += receivedItem.QuantityReceived;
                inventoryItem.LastPurchaseCost = item.UnitCost;
                inventoryItem.LastPurchaseDate = DateTime.UtcNow;

                // Recalcular costo promedio (weighted average)
                var totalValue = (previousStock * inventoryItem.AverageCost) + (receivedItem.QuantityReceived * item.UnitCost);
                inventoryItem.AverageCost = inventoryItem.CurrentStock > 0 ? totalValue / inventoryItem.CurrentStock : item.UnitCost;

                await _inventoryRepository.UpdateItemAsync(inventoryItem, cancellationToken);

                // Crear movimiento
                var movement = new InventoryMovement
                {
                    InventoryItemId = inventoryItem.Id,
                    Type = InventoryMovementType.Purchase,
                    Quantity = receivedItem.QuantityReceived,
                    UnitCost = item.UnitCost,
                    TotalCost = receivedItem.QuantityReceived * item.UnitCost,
                    PreviousStock = previousStock,
                    NewStock = inventoryItem.CurrentStock,
                    ReferenceId = entity.Id,
                    ReferenceType = "PurchaseOrder",
                    Description = $"Compra - Orden #{entity.OrderNumber}",
                    MovementDate = request.DeliveryDate ?? DateTime.UtcNow,
                    RegisteredBy = _currentUserService.Email
                };

                await _inventoryRepository.CreateMovementAsync(movement, cancellationToken);
            }
        }

        // Actualizar estado de la orden
        var allItemsReceived = entity.Items.All(i => i.IsFullyReceived);
        var someItemsReceived = entity.Items.Any(i => i.QuantityReceived > 0);

        entity.Status = allItemsReceived ? PurchaseOrderStatus.Received : 
                       someItemsReceived ? PurchaseOrderStatus.PartiallyReceived : 
                       entity.Status;

        entity.ActualDeliveryDate = request.DeliveryDate;
        entity.SupplierInvoiceNumber = request.SupplierInvoiceNumber;
        entity.ReceivedBy = _currentUserService.Email;

        var updated = await _repository.UpdateAsync(entity, cancellationToken);
        return Result<PurchaseOrderDto>.Success(updated.ToDto());
    }

    public async Task<Result<PurchaseOrderDto>> CancelAsync(Guid id, string reason, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return Result<PurchaseOrderDto>.Failure("Orden no encontrada");

        if (entity.Status == PurchaseOrderStatus.Received)
            return Result<PurchaseOrderDto>.Failure("No se puede cancelar una orden ya recibida");

        entity.Status = PurchaseOrderStatus.Cancelled;
        entity.Notes = $"{entity.Notes}\n\nCANCELADA: {reason}";

        var updated = await _repository.UpdateAsync(entity, cancellationToken);
        return Result<PurchaseOrderDto>.Success(updated.ToDto());
    }
}
