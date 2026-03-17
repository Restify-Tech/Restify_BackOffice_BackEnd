using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly BackOfficeDbContext _context;

    public ReportRepository(BackOfficeDbContext context)
    {
        _context = context;
    }

    // ========== SALES QUERIES ==========

    public async Task<(decimal totalSales, int invoiceCount, decimal averageTicket)> GetSalesTotalsAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Paid
                && i.CreatedAt.Date >= from.Date
                && i.CreatedAt.Date <= to.Date);

        var totalSales = await query.SumAsync(i => i.Total, cancellationToken);
        var invoiceCount = await query.CountAsync(cancellationToken);
        var averageTicket = invoiceCount > 0 ? totalSales / invoiceCount : 0;

        return (totalSales, invoiceCount, averageTicket);
    }

    public async Task<List<(DateTime date, decimal sales, int invoiceCount, int customerCount)>> GetDailySalesAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Paid
                && i.CreatedAt.Date >= from.Date
                && i.CreatedAt.Date <= to.Date)
            .GroupBy(i => i.CreatedAt.Date)
            .Select(g => new
            {
                Date = g.Key,
                Sales = g.Sum(i => i.Total),
                InvoiceCount = g.Count(),
                CustomerCount = g.Count(i => !string.IsNullOrEmpty(i.CustomerName))
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.Date, r.Sales, r.InvoiceCount, r.CustomerCount)).ToList();
    }

    public async Task<List<(int paymentMethod, int invoiceCount, decimal totalAmount)>> GetSalesByPaymentMethodAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Paid
                && i.CreatedAt.Date >= from.Date
                && i.CreatedAt.Date <= to.Date)
            .GroupBy(i => i.PaymentMethod)
            .Select(g => new
            {
                PaymentMethod = (int)g.Key,
                InvoiceCount = g.Count(),
                TotalAmount = g.Sum(i => i.Total)
            })
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.PaymentMethod, r.InvoiceCount, r.TotalAmount)).ToList();
    }

    public async Task<List<(int hour, int orderCount, decimal totalSales)>> GetSalesByHourAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Paid
                && i.CreatedAt.Date >= from.Date
                && i.CreatedAt.Date <= to.Date)
            .GroupBy(i => i.CreatedAt.Hour)
            .Select(g => new
            {
                Hour = g.Key,
                OrderCount = g.Count(),
                TotalSales = g.Sum(i => i.Total)
            })
            .OrderBy(x => x.Hour)
            .ToListAsync(cancellationToken);

        return result.Select(r => (r.Hour, r.OrderCount, r.TotalSales)).ToList();
    }

    // ========== PRODUCT QUERIES ==========

    public async Task<List<(Guid productId, string productName, string? categoryName, int quantity, decimal revenue)>> GetTopSellingProductsAsync(
        DateTime from,
        DateTime to,
        int limit,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Agrupar primero por ProductId y ProductName (que están en InvoiceItem)
        var result = await _context.InvoiceItems
            .Where(ii => ii.Invoice.TenantId == tenantId
                && ii.Invoice.Status == InvoiceStatus.Paid
                && ii.Invoice.CreatedAt.Date >= from.Date
                && ii.Invoice.CreatedAt.Date <= to.Date)
            .GroupBy(ii => new { ii.ProductId, ii.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                Quantity = g.Sum(ii => ii.Quantity),
                Revenue = g.Sum(ii => ii.Subtotal)
            })
            .OrderByDescending(x => x.Revenue)
            .Take(limit)
            .ToListAsync(cancellationToken);

        // Obtener categorías en segundo paso para evitar problemas con left joins
        var productIds = result.Select(r => r.ProductId).ToList();
        var productCategories = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.CategoryId != null)
            .Select(p => new { p.Id, CategoryName = p.Category!.Name })
            .ToListAsync(cancellationToken);

        var categoryMap = productCategories.ToDictionary(pc => pc.Id, pc => pc.CategoryName);

        return result.Select(r => (
            r.ProductId,
            r.ProductName,
            categoryMap.GetValueOrDefault(r.ProductId),
            r.Quantity,
            r.Revenue
        )).ToList();
    }

    public async Task<List<(Guid productId, string productName, string? categoryName, int quantity, decimal revenue)>> GetAllProductSalesAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.InvoiceItems
            .Where(ii => ii.Invoice.TenantId == tenantId
                && ii.Invoice.Status == InvoiceStatus.Paid
                && ii.Invoice.CreatedAt.Date >= from.Date
                && ii.Invoice.CreatedAt.Date <= to.Date)
            .GroupBy(ii => new { ii.ProductId, ii.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                Quantity = g.Sum(ii => ii.Quantity),
                Revenue = g.Sum(ii => ii.Subtotal)
            })
            .ToListAsync(cancellationToken);

        var productIds = result.Select(r => r.ProductId).ToList();
        var productCategories = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.CategoryId != null)
            .Select(p => new { p.Id, CategoryName = p.Category!.Name })
            .ToListAsync(cancellationToken);

        var categoryMap = productCategories.ToDictionary(pc => pc.Id, pc => pc.CategoryName);

        return result.Select(r => (
            r.ProductId,
            r.ProductName,
            categoryMap.GetValueOrDefault(r.ProductId),
            r.Quantity,
            r.Revenue
        )).ToList();
    }

    public async Task<List<(Guid productId, string productName, string? categoryName, int quantity, decimal revenue, DateTime? lastSaleDate)>> GetLowMovementProductsAsync(
        DateTime from,
        DateTime to,
        int maxSalesThreshold,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var productSales = await _context.InvoiceItems
            .Where(ii => ii.Invoice.TenantId == tenantId
                && ii.Invoice.Status == InvoiceStatus.Paid
                && ii.Invoice.CreatedAt.Date >= from.Date
                && ii.Invoice.CreatedAt.Date <= to.Date)
            .GroupBy(ii => new { ii.ProductId, ii.ProductName })
            .Select(g => new
            {
                g.Key.ProductId,
                g.Key.ProductName,
                Quantity = g.Sum(ii => ii.Quantity),
                Revenue = g.Sum(ii => ii.Subtotal),
                LastSaleDate = g.Max(ii => ii.Invoice.CreatedAt)
            })
            .Where(x => x.Quantity <= maxSalesThreshold)
            .ToListAsync(cancellationToken);

        var productIds = productSales.Select(r => r.ProductId).ToList();
        var productCategories = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.CategoryId != null)
            .Select(p => new { p.Id, CategoryName = p.Category!.Name })
            .ToListAsync(cancellationToken);

        var categoryMap = productCategories.ToDictionary(pc => pc.Id, pc => pc.CategoryName);

        return productSales.Select(r => (
            r.ProductId,
            r.ProductName,
            categoryMap.GetValueOrDefault(r.ProductId),
            r.Quantity,
            r.Revenue,
            (DateTime?)r.LastSaleDate
        )).ToList();
    }

    public async Task<List<(Guid categoryId, string categoryName, int productCount, int totalQuantity, decimal totalRevenue)>> GetCategoryPerformanceAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Obtener todas las ventas agrupadas por producto
        var salesByProduct = await _context.InvoiceItems
            .Where(ii => ii.Invoice.TenantId == tenantId
                && ii.Invoice.Status == InvoiceStatus.Paid
                && ii.Invoice.CreatedAt.Date >= from.Date
                && ii.Invoice.CreatedAt.Date <= to.Date)
            .GroupBy(ii => ii.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalQuantity = g.Sum(ii => ii.Quantity),
                TotalRevenue = g.Sum(ii => ii.Subtotal)
            })
            .ToListAsync(cancellationToken);

        // Obtener categorías de productos
        var productIds = salesByProduct.Select(s => s.ProductId).ToList();
        var productWithCategories = await _context.Products
            .Where(p => productIds.Contains(p.Id))
            .Select(p => new { p.Id, p.CategoryId, CategoryName = p.Category!.Name })
            .ToListAsync(cancellationToken);

        // Agrupar por categoría
        var result = productWithCategories
            .Join(salesByProduct, p => p.Id, s => s.ProductId, (p, s) => new { p.CategoryId, p.CategoryName, s.TotalQuantity, s.TotalRevenue, ProductId = p.Id })
            .GroupBy(x => new { x.CategoryId, x.CategoryName })
            .Select(g => (
                g.Key.CategoryId,
                g.Key.CategoryName,
                g.Select(x => x.ProductId).Distinct().Count(),
                g.Sum(x => x.TotalQuantity),
                g.Sum(x => x.TotalRevenue)
            ))
            .ToList();

        return result;
    }

    public async Task<List<(Guid modifierId, string modifierName, int timesOrdered, decimal totalRevenue, List<string> topProducts)>> GetPopularModifiersAsync(
        DateTime from,
        DateTime to,
        int limit,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Usar ProductName guardado en InvoiceItem
        var modifierData = await _context.InvoiceItemModifiers
            .Where(iim => iim.InvoiceItem.Invoice.TenantId == tenantId
                && iim.InvoiceItem.Invoice.Status == InvoiceStatus.Paid
                && iim.InvoiceItem.Invoice.CreatedAt.Date >= from.Date
                && iim.InvoiceItem.Invoice.CreatedAt.Date <= to.Date)
            .Include(iim => iim.InvoiceItem)
            .GroupBy(iim => iim.ModifierName)
            .Select(g => new
            {
                ModifierName = g.Key,
                TimesOrdered = g.Count(),
                TotalRevenue = g.Sum(iim => iim.PriceAdjustment),
                TopProducts = g.Select(iim => iim.InvoiceItem.ProductName).Distinct().Take(3).ToList()
            })
            .OrderByDescending(x => x.TimesOrdered)
            .Take(limit)
            .ToListAsync(cancellationToken);

        // Como no tenemos ModifierId en InvoiceItemModifier, usamos un Guid basado en el nombre
        return modifierData.Select(m => (
            GenerateGuidFromString(m.ModifierName), // Guid determinista
            m.ModifierName,
            m.TimesOrdered,
            m.TotalRevenue,
            m.TopProducts
        )).ToList();
    }

    // ========== EMPLOYEE QUERIES ==========

    public async Task<List<(
        Guid employeeId,
        string employeeName,
        int totalOrders,
        int completedOrders,
        int cancelledOrders,
        decimal totalSales,
        int customerCount,
        double avgServiceTimeMinutes)>> GetEmployeePerformanceAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        // Usar TakenBy como identificador del empleado
        // Agrupar por CreatedBy (user ID) y TakenBy (nombre visible)
        
        var orderStats = await _context.Orders
            .Where(o => o.TenantId == tenantId
                && o.CreatedAt.Date >= from.Date
                && o.CreatedAt.Date <= to.Date
                && !string.IsNullOrEmpty(o.TakenBy))
            .GroupBy(o => new
            {
                EmployeeKey = o.CreatedBy ?? o.TakenBy!, // Usar CreatedBy como ID si está disponible
                EmployeeName = o.TakenBy!
            })
            .Select(g => new
            {
                g.Key.EmployeeKey,
                g.Key.EmployeeName,
                TotalOrders = g.Count(),
                CompletedOrders = g.Count(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Served),
                CancelledOrders = g.Count(o => o.Status == OrderStatus.Cancelled),
                AvgServiceTime = g.Where(o => o.CompletedAt.HasValue)
                    .Average(o => (double?)((o.CompletedAt!.Value - o.CreatedAt).TotalMinutes)) ?? 0
            })
            .ToListAsync(cancellationToken);

        // Obtener ventas por empleado (desde invoices)
        var salesByEmployee = await _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Paid
                && i.CreatedAt.Date >= from.Date
                && i.CreatedAt.Date <= to.Date
                && i.Order != null 
                && !string.IsNullOrEmpty(i.Order.TakenBy))
            .Include(i => i.Order)
            .GroupBy(i => i.Order!.CreatedBy ?? i.Order.TakenBy!)
            .Select(g => new
            {
                EmployeeKey = g.Key,
                TotalSales = g.Sum(i => i.Total),
                CustomerCount = g.Count()
            })
            .ToListAsync(cancellationToken);

        // Combinar datos
        var result = orderStats.Select(o =>
        {
            var sales = salesByEmployee.FirstOrDefault(s => s.EmployeeKey == o.EmployeeKey);
            
            // Intentar parsear el EmployeeKey como Guid, si falla usar un Guid basado en hash del nombre
            Guid employeeGuid;
            if (!Guid.TryParse(o.EmployeeKey, out employeeGuid))
            {
                // Generar Guid determinista basado en el nombre (para consistencia)
                employeeGuid = GenerateGuidFromString(o.EmployeeKey);
            }
            
            return (
                employeeGuid,
                o.EmployeeName,
                o.TotalOrders,
                o.CompletedOrders,
                o.CancelledOrders,
                sales?.TotalSales ?? 0m,
                sales?.CustomerCount ?? 0,
                o.AvgServiceTime
            );
        }).ToList();

        return result;
    }

    // Helper para generar Guid determinista desde string
    private Guid GenerateGuidFromString(string input)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        var hash = md5.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }

    // ========== DASHBOARD QUERIES ==========

    public async Task<(int occupied, int total)> GetTableOccupancyAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var total = await _context.Tables
            .Where(t => t.TenantId == tenantId && t.IsActive)
            .CountAsync(cancellationToken);

        var occupied = await _context.Tables
            .Where(t => t.TenantId == tenantId 
                && t.IsActive 
                && t.Status == TableStatus.Occupied)
            .CountAsync(cancellationToken);

        return (occupied, total);
    }

    public async Task<(int pending, int inProgress, int ready)> GetOrderStatusCountsAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var pending = await _context.Orders
            .Where(o => o.TenantId == tenantId && o.Status == OrderStatus.Pending)
            .CountAsync(cancellationToken);

        var inProgress = await _context.Orders
            .Where(o => o.TenantId == tenantId && o.Status == OrderStatus.Preparing)
            .CountAsync(cancellationToken);

        var ready = await _context.Orders
            .Where(o => o.TenantId == tenantId && o.Status == OrderStatus.Ready)
            .CountAsync(cancellationToken);

        return (pending, inProgress, ready);
    }

    // ========== CONTROL QUERIES ==========

    public async Task<List<(
        Guid invoiceId,
        string invoiceNumber,
        string? orderNumber,
        string? employeeName,
        decimal amount,
        string? cancelReason,
        DateTime cancelledAt,
        string? cancelledBy)>> GetCancelledInvoicesAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Cancelled
                && i.CancelledAt.HasValue
                && i.CancelledAt.Value.Date >= from.Date
                && i.CancelledAt.Value.Date <= to.Date)
            .Include(i => i.Order)
            .Select(i => new
            {
                i.Id,
                i.InvoiceNumber,
                OrderNumber = i.Order != null ? i.Order.OrderNumber : null,
                EmployeeName = i.Order != null ? i.Order.TakenBy : null,
                i.Total,
                i.CancelReason,
                CancelledAt = i.CancelledAt!.Value,
                CancelledBy = i.IssuedBy
            })
            .OrderByDescending(i => i.CancelledAt)
            .ToListAsync(cancellationToken);

        return result.Select(r => (
            r.Id,
            r.InvoiceNumber,
            r.OrderNumber,
            r.EmployeeName,
            r.Total,
            r.CancelReason,
            r.CancelledAt,
            r.CancelledBy
        )).ToList();
    }

    public async Task<List<(
        Guid invoiceId,
        string invoiceNumber,
        string? employeeName,
        decimal originalAmount,
        decimal discountPercentage,
        decimal discountAmount,
        decimal finalAmount,
        DateTime createdAt,
        string? appliedBy)>> GetDiscountsAsync(
        DateTime from,
        DateTime to,
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        var result = await _context.Invoices
            .Where(i => i.TenantId == tenantId
                && i.Status == InvoiceStatus.Paid
                && i.DiscountPercentage > 0
                && i.CreatedAt.Date >= from.Date
                && i.CreatedAt.Date <= to.Date)
            .Include(i => i.Order)
            .Select(i => new
            {
                i.Id,
                i.InvoiceNumber,
                EmployeeName = i.Order != null ? i.Order.TakenBy : null,
                OriginalAmount = i.Subtotal,
                i.DiscountPercentage,
                i.DiscountAmount,
                i.Total,
                i.CreatedAt,
                AppliedBy = i.IssuedBy
            })
            .OrderByDescending(i => i.CreatedAt)
            .ToListAsync(cancellationToken);

        return result.Select(r => (
            r.Id,
            r.InvoiceNumber,
            r.EmployeeName,
            r.OriginalAmount,
            r.DiscountPercentage,
            r.DiscountAmount,
            r.Total,
            r.CreatedAt,
            r.AppliedBy
        )).ToList();
    }
}

