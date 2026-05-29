using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Domain.Interfaces;

/// <summary>
/// Repositorio para Plans de suscripción
/// </summary>
public interface IPlanRepository
{
    Task<IEnumerable<Plan>> GetAllAsync(CancellationToken ct = default);
    Task<Plan?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Plan?> GetWithScreensAsync(Guid id, CancellationToken ct = default);
    Task AddAsync(Plan plan, CancellationToken ct = default);
    void Update(Plan plan);
    void Delete(Plan plan);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
}
