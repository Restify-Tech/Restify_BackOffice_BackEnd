using Microsoft.EntityFrameworkCore;
using Restify.Auth.Domain.Entities;
using Restify.Auth.Domain.Interfaces;

namespace Restify.Auth.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de planes de suscripción
/// </summary>
public class PlanRepository : IPlanRepository
{
    private readonly AppDbContext _context;

    public PlanRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Plan>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Plans
            .AsNoTracking()
            .Include(p => p.PlanScreenPermissions)
            .Include(p => p.Tenants)
            .OrderBy(p => p.DisplayOrder)
            .ToListAsync(ct);
    }

    public async Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Plans
            .AsNoTracking()
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<Plan?> GetWithScreensAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Plans
            .AsNoTracking()
            .Include(p => p.PlanScreenPermissions)
            .Include(p => p.Tenants)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task AddAsync(Plan plan, CancellationToken ct = default)
    {
        await _context.Plans.AddAsync(plan, ct);
    }

    public void Update(Plan plan)
    {
        _context.Plans.Update(plan);
    }

    public void Delete(Plan plan)
    {
        _context.Plans.Remove(plan);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Plans.AnyAsync(p => p.Id == id, ct);
    }
}
