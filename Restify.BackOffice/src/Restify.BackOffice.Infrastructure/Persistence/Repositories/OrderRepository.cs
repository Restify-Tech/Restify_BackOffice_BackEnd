using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly BackOfficeDbContext _context;

    public OrderRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Modifiers)
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order?> GetByOrderNumberAsync(string orderNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Modifiers)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber, cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
            .Where(o => o.Status == status)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetByTableIdAsync(Guid tableId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
            .Where(o => o.TableId == tableId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Order>> GetActiveOrdersAsync(CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[]
        {
            OrderStatus.Pending,
            OrderStatus.Confirmed,
            OrderStatus.Preparing,
            OrderStatus.Ready,
            OrderStatus.Served
        };

        return await _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
                .ThenInclude(i => i.Product)
            .Include(o => o.Items)
                .ThenInclude(i => i.Modifiers)
            .Where(o => activeStatuses.Contains(o.Status))
            .OrderBy(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        await _context.Orders.AddAsync(order, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return (await GetByIdAsync(order.Id, cancellationToken))!;
    }

    public async Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync(cancellationToken);
        
        return (await GetByIdAsync(order.Id, cancellationToken))!;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _context.Orders.FindAsync(new object[] { id }, cancellationToken);
        if (order != null)
        {
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var todayOrdersCount = await _context.Orders
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .CountAsync(cancellationToken);

        var sequence = (todayOrdersCount + 1).ToString("D4");
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{sequence}";
    }
}
