using Restify.Auth.Application.DTOs.Common;
using Restify.Auth.Application.DTOs.Plans;

namespace Restify.Auth.Application.Interfaces;

/// <summary>
/// Servicio de gestion de planes de suscripcion
/// </summary>
public interface IPlanService
{
    Task<Result<IEnumerable<PlanDto>>> GetAllAsync(CancellationToken ct = default);
    Task<Result<PlanDto>> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Result<PlanDto>> CreateAsync(CreatePlanRequest request, CancellationToken ct = default);
    Task<Result<PlanDto>> UpdateAsync(Guid id, UpdatePlanRequest request, CancellationToken ct = default);
    Task<Result> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<Result> AssignPlanToTenantAsync(Guid tenantId, Guid planId, CancellationToken ct = default);
    Task<Result> ToggleTenantStatusAsync(Guid tenantId, CancellationToken ct = default);
}
