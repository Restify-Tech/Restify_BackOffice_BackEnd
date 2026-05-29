using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.Core.Application.DTOs.Common;

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

    public async Task<(IEnumerable<Order> Items, int TotalCount)> GetPagedAsync(
        GetOrdersQuery query, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> q = _context.Orders
            .Include(o => o.Table)
            .Include(o => o.Items)
            .AsNoTracking();

        if (query.Status.HasValue)
            q = q.Where(o => o.Status == query.Status.Value);

        if (query.OrderType.HasValue)
            q = q.Where(o => o.Type == query.OrderType.Value);

        if (query.DateFrom.HasValue)
            q = q.Where(o => o.CreatedAt >= query.DateFrom.Value);

        if (query.DateTo.HasValue)
            q = q.Where(o => o.CreatedAt <= query.DateTo.Value);

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            var term = query.SearchTerm.ToLower();
            q = q.Where(o =>
                o.OrderNumber.ToLower().Contains(term) ||
                (o.CustomerName != null && o.CustomerName.ToLower().Contains(term)) ||
                (o.TakenBy != null && o.TakenBy.ToLower().Contains(term)));
        }

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(o => o.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task<OrderStatisticsDto> GetStatisticsAsync(
        DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> q = _context.Orders.AsNoTracking();

        if (from.HasValue) q = q.Where(o => o.CreatedAt >= from.Value);
        if (to.HasValue)   q = q.Where(o => o.CreatedAt <= to.Value);

        var orders = await q.Select(o => new { o.Status, o.Type, o.Total }).ToListAsync(cancellationToken);

        var completed = orders.Where(o => o.Status == OrderStatus.Completed).ToList();
        var totalRevenue = completed.Sum(o => o.Total);

        return new OrderStatisticsDto
        {
            TotalOrders      = orders.Count,
            PendingOrders    = orders.Count(o => o.Status == OrderStatus.Pending),
            PreparingOrders  = orders.Count(o => o.Status == OrderStatus.Preparing),
            ReadyOrders      = orders.Count(o => o.Status == OrderStatus.Ready),
            CompletedOrders  = completed.Count,
            CancelledOrders  = orders.Count(o => o.Status == OrderStatus.Cancelled),
            TotalRevenue     = totalRevenue,
            AverageOrderValue = completed.Count > 0 ? totalRevenue / completed.Count : 0,
            OrdersByType = orders
                .GroupBy(o => o.Type.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    public async Task<OrderTodayStatsDto> GetTodayStatsAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        var tomorrow = today.AddDays(1);

        var activeStatuses = new[] { OrderStatus.Pending, OrderStatus.Confirmed, OrderStatus.Preparing, OrderStatus.Ready, OrderStatus.Served };

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.CreatedAt >= today && o.CreatedAt < tomorrow)
            .Select(o => new { o.Status, o.Total, Hour = o.CreatedAt.Hour })
            .ToListAsync(cancellationToken);

        var completed = orders.Where(o => o.Status == OrderStatus.Completed).ToList();
        var todayRevenue = completed.Sum(o => o.Total);

        return new OrderTodayStatsDto
        {
            TodayOrders      = orders.Count,
            ActiveOrders     = orders.Count(o => activeStatuses.Contains(o.Status)),
            TodayRevenue     = todayRevenue,
            TodayAverageTicket = completed.Count > 0 ? todayRevenue / completed.Count : 0,
            CompletedToday   = completed.Count,
            CancelledToday   = orders.Count(o => o.Status == OrderStatus.Cancelled),
            OrdersByHour     = orders
                .GroupBy(o => o.Hour)
                .OrderBy(g => g.Key)
                .Select(g => new HourlyOrderCountDto { Hour = g.Key, Count = g.Count() })
                .ToList()
        };
    }
}
