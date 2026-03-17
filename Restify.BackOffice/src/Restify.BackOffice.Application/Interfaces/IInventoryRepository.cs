using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Application.Interfaces;

public interface IInventoryRepository
{
    Task<List<InventoryItem>> GetAllItemsAsync(CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetItemByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<InventoryItem?> GetItemByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<InventoryItem> CreateItemAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task<InventoryItem> UpdateItemAsync(InventoryItem item, CancellationToken cancellationToken = default);
    Task DeleteItemAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<InventoryItem>> GetLowStockItemsAsync(CancellationToken cancellationToken = default);
    
    Task<InventoryMovement> CreateMovementAsync(InventoryMovement movement, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetMovementsByProductAsync(Guid productId, DateTime? from, DateTime? to, CancellationToken cancellationToken = default);
    Task<List<InventoryMovement>> GetRecentMovementsAsync(int limit, CancellationToken cancellationToken = default);
}

public interface ISupplierRepository
{
    Task<List<Supplier>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Supplier?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Supplier> CreateAsync(Supplier entity, CancellationToken cancellationToken = default);
    Task<Supplier> UpdateAsync(Supplier entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}

public interface IPurchaseOrderRepository
{
    Task<List<PurchaseOrder>> GetAllAsync(PurchaseOrderStatus? status, CancellationToken cancellationToken = default);
    Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PurchaseOrder> CreateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
    Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default);
}
