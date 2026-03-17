using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Domain.Enums;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class InventoryRepository : IInventoryRepository
{
    private readonly BackOfficeDbContext _context;

    public InventoryRepository(BackOfficeDbContext context) => _context = context;

    public async Task<List<InventoryItem>> GetAllItemsAsync(CancellationToken ct = default) =>
        await _context.InventoryItems.Include(i => i.Product).ToListAsync(ct);

    public async Task<InventoryItem?> GetItemByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.InventoryItems.Include(i => i.Product).FirstOrDefaultAsync(i => i.Id == id, ct);

    public async Task<InventoryItem?> GetItemByProductIdAsync(Guid productId, CancellationToken ct = default) =>
        await _context.InventoryItems.Include(i => i.Product).FirstOrDefaultAsync(i => i.ProductId == productId, ct);

    public async Task<InventoryItem> CreateItemAsync(InventoryItem item, CancellationToken ct = default)
    {
        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync(ct);
        return item;
    }

    public async Task<InventoryItem> UpdateItemAsync(InventoryItem item, CancellationToken ct = default)
    {
        _context.InventoryItems.Update(item);
        await _context.SaveChangesAsync(ct);
        return item;
    }

    public async Task DeleteItemAsync(Guid id, CancellationToken ct = default)
    {
        var item = await _context.InventoryItems.FindAsync(new object[] { id }, ct);
        if (item != null)
        {
            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<List<InventoryItem>> GetLowStockItemsAsync(CancellationToken ct = default) =>
        await _context.InventoryItems
            .Include(i => i.Product)
            .Where(i => i.CurrentStock <= i.MinimumStock)
            .ToListAsync(ct);

    public async Task<InventoryMovement> CreateMovementAsync(InventoryMovement movement, CancellationToken ct = default)
    {
        _context.InventoryMovements.Add(movement);
        await _context.SaveChangesAsync(ct);
        return movement;
    }

    public async Task<List<InventoryMovement>> GetMovementsByProductAsync(Guid productId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var query = _context.InventoryMovements
            .Include(m => m.InventoryItem).ThenInclude(i => i.Product)
            .Where(m => m.InventoryItem.ProductId == productId);

        if (from.HasValue) query = query.Where(m => m.MovementDate >= from.Value);
        if (to.HasValue) query = query.Where(m => m.MovementDate <= to.Value);

        return await query.OrderByDescending(m => m.MovementDate).ToListAsync(ct);
    }

    public async Task<List<InventoryMovement>> GetRecentMovementsAsync(int limit, CancellationToken ct = default) =>
        await _context.InventoryMovements
            .Include(m => m.InventoryItem).ThenInclude(i => i.Product)
            .OrderByDescending(m => m.MovementDate)
            .Take(limit)
            .ToListAsync(ct);
}

public class SupplierRepository : ISupplierRepository
{
    private readonly BackOfficeDbContext _context;

    public SupplierRepository(BackOfficeDbContext context) => _context = context;

    public async Task<List<Supplier>> GetAllAsync(CancellationToken ct = default) =>
        await _context.Suppliers.OrderBy(s => s.Name).ToListAsync(ct);

    public async Task<Supplier?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.Suppliers.FindAsync(new object[] { id }, ct);

    public async Task<Supplier> CreateAsync(Supplier entity, CancellationToken ct = default)
    {
        _context.Suppliers.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<Supplier> UpdateAsync(Supplier entity, CancellationToken ct = default)
    {
        _context.Suppliers.Update(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.Suppliers.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.Suppliers.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }
}

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly BackOfficeDbContext _context;

    public PurchaseOrderRepository(BackOfficeDbContext context) => _context = context;

    public async Task<List<PurchaseOrder>> GetAllAsync(PurchaseOrderStatus? status, CancellationToken ct = default)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items).ThenInclude(i => i.Product)
            .AsQueryable();

        if (status.HasValue) query = query.Where(po => po.Status == status.Value);

        return await query.OrderByDescending(po => po.OrderDate).ToListAsync(ct);
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items).ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(po => po.Id == id, ct);

    public async Task<PurchaseOrder> CreateAsync(PurchaseOrder entity, CancellationToken ct = default)
    {
        _context.PurchaseOrders.Add(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task<PurchaseOrder> UpdateAsync(PurchaseOrder entity, CancellationToken ct = default)
    {
        _context.PurchaseOrders.Update(entity);
        await _context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.PurchaseOrders.FindAsync(new object[] { id }, ct);
        if (entity != null)
        {
            _context.PurchaseOrders.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
    }

    public async Task<string> GenerateOrderNumberAsync(CancellationToken ct = default)
    {
        var today = DateTime.UtcNow;
        var prefix = $"PO{today:yyyyMMdd}";
        var count = await _context.PurchaseOrders.CountAsync(po => po.OrderNumber.StartsWith(prefix), ct);
        return $"{prefix}-{(count + 1):D4}";
    }
}
