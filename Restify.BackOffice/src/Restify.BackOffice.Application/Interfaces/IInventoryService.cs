using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IInventoryService
{
    // Inventory Items
    Task<Result<List<InventoryItemDto>>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<Result<List<InventoryItemSummaryDto>>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    Task<Result<InventoryItemDto>> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<InventoryItemDto>> GetItemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<Result<InventoryItemDto>> UpsertItemAsync(UpsertInventoryItemRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
    
    // Adjustments
    Task<Result<InventoryMovementDto>> AdjustStockAsync(AdjustInventoryRequest request, CancellationToken cancellationToken = default);
    
    // Movements
    Task<Result<List<InventoryMovementDto>>> GetMovementsByProductAsync(Guid productId, DateTime? from = null, DateTime? to = null, CancellationToken cancellationToken = default);
    Task<Result<List<InventoryMovementSummaryDto>>> GetRecentMovementsAsync(int limit = 50, CancellationToken cancellationToken = default);
    
    // Statistics
    Task<Result<InventoryStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
