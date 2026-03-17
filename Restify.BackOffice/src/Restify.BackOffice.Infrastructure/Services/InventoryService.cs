using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Application.Mappings;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IInventoryRepository _repository;
    private readonly ICurrentUserService _currentUserService;

    public InventoryService(IInventoryRepository repository, ICurrentUserService currentUserService)
    {
        _repository = repository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<List<InventoryItemDto>>> GetAllItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetAllItemsAsync(cancellationToken);
        return Result<List<InventoryItemDto>>.Success(items.Select(i => i.ToDto()).ToList());
    }

    public async Task<Result<List<InventoryItemSummaryDto>>> GetLowStockItemsAsync(CancellationToken cancellationToken = default)
    {
        var items = await _repository.GetLowStockItemsAsync(cancellationToken);
        return Result<List<InventoryItemSummaryDto>>.Success(items.Select(i => i.ToSummaryDto()).ToList());
    }

    public async Task<Result<InventoryItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(id, cancellationToken);
        return item == null 
            ? Result<InventoryItemDto>.Failure("Item no encontrado") 
            : Result<InventoryItemDto>.Success(item.ToDto());
    }

    public async Task<Result<InventoryItemDto>> GetItemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByProductIdAsync(productId, cancellationToken);
        return item == null 
            ? Result<InventoryItemDto>.Failure("Item no encontrado") 
            : Result<InventoryItemDto>.Success(item.ToDto());
    }

    public async Task<Result<InventoryItemDto>> UpsertItemAsync(UpsertInventoryItemRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetItemByProductIdAsync(request.ProductId, cancellationToken);

        if (existing != null)
        {
            existing.CurrentStock = request.CurrentStock;
            existing.Unit = request.Unit;
            existing.MinimumStock = request.MinimumStock;
            existing.MaximumStock = request.MaximumStock;
            existing.AverageCost = request.AverageCost;
            existing.StorageLocation = request.StorageLocation;
            existing.TrackStock = request.TrackStock;
            existing.CostMethod = request.CostMethod;

            var updated = await _repository.UpdateItemAsync(existing, cancellationToken);
            return Result<InventoryItemDto>.Success(updated.ToDto());
        }

        var newItem = new InventoryItem
        {
            ProductId = request.ProductId,
            CurrentStock = request.CurrentStock,
            Unit = request.Unit,
            MinimumStock = request.MinimumStock,
            MaximumStock = request.MaximumStock,
            AverageCost = request.AverageCost,
            StorageLocation = request.StorageLocation,
            TrackStock = request.TrackStock,
            CostMethod = request.CostMethod
        };

        var created = await _repository.CreateItemAsync(newItem, cancellationToken);
        return Result<InventoryItemDto>.Success(created.ToDto());
    }

    public async Task<Result<bool>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _repository.DeleteItemAsync(id, cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<InventoryMovementDto>> AdjustStockAsync(AdjustInventoryRequest request, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetItemByIdAsync(request.InventoryItemId, cancellationToken);
        if (item == null)
            return Result<InventoryMovementDto>.Failure("Item no encontrado");

        var previousStock = item.CurrentStock;
        var newStock = previousStock + request.Quantity;

        if (newStock < 0)
            return Result<InventoryMovementDto>.Failure("Stock insuficiente");

        var movementType = request.Quantity > 0 ? InventoryMovementType.AdjustmentIn : InventoryMovementType.AdjustmentOut;
        var unitCost = request.UnitCost ?? item.AverageCost;

        var movement = new InventoryMovement
        {
            InventoryItemId = request.InventoryItemId,
            Type = movementType,
            Quantity = Math.Abs(request.Quantity),
            UnitCost = unitCost,
            TotalCost = Math.Abs(request.Quantity) * unitCost,
            PreviousStock = previousStock,
            NewStock = newStock,
            Description = request.Reason,
            Notes = request.Notes,
            MovementDate = DateTime.UtcNow,
            RegisteredBy = _currentUserService.Email
        };

        item.CurrentStock = newStock;
        await _repository.UpdateItemAsync(item, cancellationToken);

        var created = await _repository.CreateMovementAsync(movement, cancellationToken);
        return Result<InventoryMovementDto>.Success(created.ToDto());
    }

    public async Task<Result<List<InventoryMovementDto>>> GetMovementsByProductAsync(Guid productId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default)
    {
        var movements = await _repository.GetMovementsByProductAsync(productId, from, to, cancellationToken);
        return Result<List<InventoryMovementDto>>.Success(movements.Select(m => m.ToDto()).ToList());
    }

    public async Task<Result<List<InventoryMovementSummaryDto>>> GetRecentMovementsAsync(int limit = 50, CancellationToken cancellationToken = default)
    {
        var movements = await _repository.GetRecentMovementsAsync(limit, cancellationToken);
        return Result<List<InventoryMovementSummaryDto>>.Success(movements.Select(m => m.ToSummaryDto()).ToList());
    }

    public async Task<Result<InventoryStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        var allItems = await _repository.GetAllItemsAsync(cancellationToken);
        var lowStock = allItems.Where(i => i.IsLowStock).ToList();
        var outOfStock = allItems.Where(i => i.IsOutOfStock).ToList();

        var stats = new InventoryStatisticsDto
        {
            TotalItems = allItems.Count,
            LowStockItems = lowStock.Count,
            OutOfStockItems = outOfStock.Count,
            TotalInventoryValue = allItems.Sum(i => i.TotalValue),
            LowStockAlerts = lowStock.Take(10).Select(i => i.ToSummaryDto()).ToList(),
            OutOfStockAlerts = outOfStock.Take(10).Select(i => i.ToSummaryDto()).ToList()
        };

        return Result<InventoryStatisticsDto>.Success(stats);
    }
}
