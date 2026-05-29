using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Plans;
using Restify.Auth.Application.Interfaces;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Enums;
using Restify.Auth.Infrastructure.Persistence;

namespace Restify.Auth.Infrastructure.Services;

/// <summary>
/// Servicio de gestión de planes de suscripción
/// </summary>
public class PlanService : IPlanService
{
    private readonly AppDbContext _context;
    private readonly ILogger<PlanService> _logger;

    public PlanService(AppDbContext context, ILogger<PlanService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Result<IEnumerable<PlanDto>>> GetAllAsync(CancellationToken ct = default)
    {
        var plans = await _context.Plans
            .AsNoTracking()
            .Include(p => p.PlanScreenPermissions)
            .Include(p => p.Tenants)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(ct);

        var dtos = plans.Select(MapToDto);
        return Result<IEnumerable<PlanDto>>.Success(dtos);
    }

    public async Task<Result<PlanDto>> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await _context.Plans
            .AsNoTracking()
            .Include(p => p.PlanScreenPermissions)
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (plan == null)
            return Result<PlanDto>.Failure("Plan no encontrado");

        return Result<PlanDto>.Success(MapToDto(plan));
    }

    public async Task<Result<PlanDto>> CreateAsync(CreatePlanRequest request, CancellationToken ct = default)
    {
        // Si es default, quitar el default de los demás
        if (request.IsDefault)
        {
            var currentDefaults = await _context.Plans
                .Where(p => p.IsDefault)
                .ToListAsync(ct);

            foreach (var d in currentDefaults)
                d.IsDefault = false;
        }

        var plan = new Plan
        {
            Name = request.Name,
            Description = request.Description,
            MonthlyPrice = request.MonthlyPrice,
            AnnualPrice = request.AnnualPrice,
            MaxUsers = request.MaxUsers,
            MaxBranches = request.MaxBranches,
            Color = request.Color,
            DisplayOrder = request.DisplayOrder,
            IsDefault = request.IsDefault,
            IsActive = true
        };

        // Agregar pantallas incluidas
        foreach (var code in request.IncludedScreenCodes.Distinct())
        {
            plan.PlanScreenPermissions.Add(new PlanScreenPermission
            {
                ScreenCode = code,
                IsIncluded = true
            });
        }

        await _context.Plans.AddAsync(plan, ct);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Plan creado: {PlanId} - {PlanName}", plan.Id, plan.Name);

        // Cargar con relaciones para el DTO
        var created = await _context.Plans
            .AsNoTracking()
            .Include(p => p.PlanScreenPermissions)
            .Include(p => p.Tenants)
            .FirstAsync(p => p.Id == plan.Id, ct);

        return Result<PlanDto>.Success(MapToDto(created));
    }

    public async Task<Result<PlanDto>> UpdateAsync(Guid id, UpdatePlanRequest request, CancellationToken ct = default)
    {
        var plan = await _context.Plans
            .Include(p => p.PlanScreenPermissions)
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (plan == null)
            return Result<PlanDto>.Failure("Plan no encontrado");

        // Si es default, quitar el default de los demás
        if (request.IsDefault && !plan.IsDefault)
        {
            var currentDefaults = await _context.Plans
                .Where(p => p.IsDefault && p.Id != id)
                .ToListAsync(ct);

            foreach (var d in currentDefaults)
                d.IsDefault = false;
        }

        plan.Name = request.Name;
        plan.Description = request.Description;
        plan.MonthlyPrice = request.MonthlyPrice;
        plan.AnnualPrice = request.AnnualPrice;
        plan.MaxUsers = request.MaxUsers;
        plan.MaxBranches = request.MaxBranches;
        plan.Color = request.Color;
        plan.DisplayOrder = request.DisplayOrder;
        plan.IsDefault = request.IsDefault;
        plan.IsActive = request.IsActive;

        // Actualizar pantallas incluidas: borrar y recrear
        _context.PlanScreenPermissions.RemoveRange(plan.PlanScreenPermissions);

        foreach (var code in request.IncludedScreenCodes.Distinct())
        {
            plan.PlanScreenPermissions.Add(new PlanScreenPermission
            {
                ScreenCode = code,
                IsIncluded = true
            });
        }

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Plan actualizado: {PlanId} - {PlanName}", plan.Id, plan.Name);

        return Result<PlanDto>.Success(MapToDto(plan));
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var plan = await _context.Plans
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

        if (plan == null)
            return Result.Failure("Plan no encontrado");

        if (plan.Tenants.Any())
            return Result.Failure($"No se puede eliminar el plan porque tiene {plan.Tenants.Count} tenant(s) asignado(s)");

        _context.Plans.Remove(plan);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Plan eliminado: {PlanId} - {PlanName}", id, plan.Name);

        return Result.Success();
    }

    public async Task<Result> AssignPlanToTenantAsync(Guid tenantId, Guid planId, CancellationToken ct = default)
    {
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId, ct);

        if (tenant == null)
            return Result.Failure("Tenant no encontrado");

        var planExists = await _context.Plans.AnyAsync(p => p.Id == planId && p.IsActive, ct);
        if (!planExists)
            return Result.Failure("Plan no encontrado o inactivo");

        tenant.PlanId = planId;
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Plan {PlanId} asignado al tenant {TenantId}", planId, tenantId);

        return Result.Success();
    }

    public async Task<Result> ToggleTenantStatusAsync(Guid tenantId, CancellationToken ct = default)
    {
        var tenant = await _context.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId, ct);

        if (tenant == null)
            return Result.Failure("Tenant no encontrado");

        tenant.Status = tenant.Status == TenantStatus.Active
            ? TenantStatus.Inactive
            : TenantStatus.Active;

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation("Estado del tenant {TenantId} cambiado a {Status}", tenantId, tenant.Status);

        return Result.Success();
    }

    private static PlanDto MapToDto(Plan plan)
    {
        return new PlanDto(
            plan.Id,
            plan.Name,
            plan.Description,
            plan.MonthlyPrice,
            plan.AnnualPrice,
            plan.MaxUsers,
            plan.MaxBranches,
            plan.IsActive,
            plan.DisplayOrder,
            plan.Color,
            plan.IsDefault,
            plan.Tenants.Count,
            plan.PlanScreenPermissions
                .Where(sp => sp.IsIncluded)
                .Select(sp => sp.ScreenCode)
                .ToList(),
            plan.CreatedAt
        );
    }
}
