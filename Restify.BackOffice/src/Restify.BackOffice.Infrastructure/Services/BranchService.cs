using Microsoft.EntityFrameworkCore;
using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Application.Interfaces;
using Restify.BackOffice.Domain.Entities;
using Restify.BackOffice.Infrastructure.Persistence;
using Restify.Core.Application.DTOs.Common;
using Restify.Core.Application.Interfaces;

namespace Restify.BackOffice.Infrastructure.Services;

public class BranchService : IBranchService
{
    private readonly BackOfficeDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public BranchService(
        BackOfficeDbContext context,
        ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<Result<IEnumerable<BranchDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var branches = await _context.Branches
            .AsNoTracking()
            .Include(b => b.Tables.Where(t => t.IsActive))
            .Include(b => b.Employees.Where(e => e.IsActive))
            .OrderBy(b => b.Name)
            .ToListAsync(cancellationToken);

        var dtos = branches.Select(b => MapToDto(b));

        return Result<IEnumerable<BranchDto>>.Success(dtos);
    }

    public async Task<Result<BranchDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var branch = await _context.Branches
            .AsNoTracking()
            .Include(b => b.Tables.Where(t => t.IsActive))
            .Include(b => b.Employees.Where(e => e.IsActive))
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (branch == null)
            return Result<BranchDto>.Failure("Sucursal no encontrada");

        return Result<BranchDto>.Success(MapToDto(branch));
    }

    public async Task<Result<BranchDto>> CreateAsync(CreateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = _currentUserService.TenantId ?? Guid.Empty;

        var branch = new Branch
        {
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            Phone = request.Phone,
            Email = request.Email,
            Timezone = "America/Guayaquil",
            IsActive = true,
            OpeningHours = request.OpeningHours,
            Notes = request.Notes,
            TenantId = tenantId
        };

        _context.Branches.Add(branch);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<BranchDto>.Success(MapToDto(branch));
    }

    public async Task<Result<BranchDto>> UpdateAsync(Guid id, UpdateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var branch = await _context.Branches
            .Include(b => b.Tables.Where(t => t.IsActive))
            .Include(b => b.Employees.Where(e => e.IsActive))
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (branch == null)
            return Result<BranchDto>.Failure("Sucursal no encontrada");

        branch.Name = request.Name;
        branch.Address = request.Address;
        branch.City = request.City;
        branch.Phone = request.Phone;
        branch.Email = request.Email;
        branch.IsActive = request.IsActive;
        branch.OpeningHours = request.OpeningHours;
        branch.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);

        return Result<BranchDto>.Success(MapToDto(branch));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (branch == null)
            return Result<bool>.Failure("Sucursal no encontrada");

        // Verificar que no tenga tablas activas asignadas
        var hasActiveTables = await _context.Tables
            .AnyAsync(t => t.BranchId == id && t.IsActive, cancellationToken);

        if (hasActiveTables)
            return Result<bool>.Failure("No se puede eliminar la sucursal porque tiene mesas activas asignadas");

        _context.Branches.Remove(branch);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<BranchStatsDto>> GetStatsAsync(Guid id, DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var branch = await _context.Branches
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);

        if (branch == null)
            return Result<BranchStatsDto>.Failure("Sucursal no encontrada");

        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var today = DateTime.UtcNow.Date;
        var weekStart = today.AddDays(-(int)today.DayOfWeek);
        var monthStart = new DateTime(today.Year, today.Month, 1);

        // Obtener ordenes del rango
        var ordersQuery = _context.Orders
            .Where(o => o.BranchId == id
                && o.CreatedAt >= fromDate
                && o.CreatedAt <= toDate
                && o.Status == Domain.Entities.OrderStatus.Completed);

        var totalOrders = await ordersQuery.CountAsync(cancellationToken);
        var totalRevenue = await ordersQuery.SumAsync(o => o.Total, cancellationToken);

        // Ventas de hoy
        var todaySales = await _context.Orders
            .Where(o => o.BranchId == id
                && o.CreatedAt >= today
                && o.Status == Domain.Entities.OrderStatus.Completed)
            .SumAsync(o => o.Total, cancellationToken);

        // Ventas de la semana
        var weekSales = await _context.Orders
            .Where(o => o.BranchId == id
                && o.CreatedAt >= weekStart
                && o.Status == Domain.Entities.OrderStatus.Completed)
            .SumAsync(o => o.Total, cancellationToken);

        // Ventas del mes
        var monthSales = await _context.Orders
            .Where(o => o.BranchId == id
                && o.CreatedAt >= monthStart
                && o.Status == Domain.Entities.OrderStatus.Completed)
            .SumAsync(o => o.Total, cancellationToken);

        // Mesas y empleados activos
        var activeTables = await _context.Tables
            .CountAsync(t => t.BranchId == id && t.IsActive, cancellationToken);

        var activeEmployees = await _context.Employees
            .CountAsync(e => e.BranchId == id && e.IsActive, cancellationToken);

        var stats = new BranchStatsDto(
            TotalOrders: totalOrders,
            TotalRevenue: totalRevenue,
            ActiveTables: activeTables,
            ActiveEmployees: activeEmployees,
            TodaySales: todaySales,
            WeekSales: weekSales,
            MonthSales: monthSales);

        return Result<BranchStatsDto>.Success(stats);
    }

    public async Task<Result<ConsolidatedBranchReportDto>> GetConsolidatedReportAsync(DateTime? from, DateTime? to, CancellationToken cancellationToken = default)
    {
        var fromDate = from ?? DateTime.UtcNow.AddMonths(-1);
        var toDate = to ?? DateTime.UtcNow;

        var branches = await _context.Branches
            .AsNoTracking()
            .Where(b => b.IsActive)
            .ToListAsync(cancellationToken);

        var completedOrders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.BranchId != null
                && o.CreatedAt >= fromDate
                && o.CreatedAt <= toDate
                && o.Status == Domain.Entities.OrderStatus.Completed)
            .GroupBy(o => o.BranchId)
            .Select(g => new
            {
                BranchId = g.Key!.Value,
                Revenue = g.Sum(o => o.Total),
                Orders = g.Count()
            })
            .ToListAsync(cancellationToken);

        var summaries = branches.Select(b =>
        {
            var orderData = completedOrders.FirstOrDefault(o => o.BranchId == b.Id);
            var revenue = orderData?.Revenue ?? 0;
            var orders = orderData?.Orders ?? 0;
            var avgTicket = orders > 0 ? revenue / orders : 0;

            return new BranchSummaryDto(
                BranchId: b.Id,
                BranchName: b.Name,
                Revenue: revenue,
                Orders: orders,
                AvgTicket: avgTicket);
        }).ToList();

        var grandTotal = summaries.Sum(s => s.Revenue);
        var totalOrders = summaries.Sum(s => s.Orders);

        var report = new ConsolidatedBranchReportDto(
            Branches: summaries,
            GrandTotal: grandTotal,
            TotalOrders: totalOrders);

        return Result<ConsolidatedBranchReportDto>.Success(report);
    }

    private static BranchDto MapToDto(Branch branch)
    {
        return new BranchDto(
            Id: branch.Id,
            Name: branch.Name,
            Address: branch.Address,
            City: branch.City,
            Phone: branch.Phone,
            Email: branch.Email,
            Timezone: branch.Timezone,
            IsActive: branch.IsActive,
            OpeningHours: branch.OpeningHours,
            Notes: branch.Notes,
            ActiveTablesCount: branch.Tables?.Count ?? 0,
            ActiveEmployeesCount: branch.Employees?.Count ?? 0,
            CreatedAt: branch.CreatedAt,
            UpdatedAt: branch.UpdatedAt);
    }
}
