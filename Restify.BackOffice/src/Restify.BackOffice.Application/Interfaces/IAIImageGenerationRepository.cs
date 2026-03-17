using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAIImageGenerationRepository
{
    Task<AIImageGeneration?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AIImageGeneration>> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AIImageGeneration>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AIImageGeneration> CreateAsync(AIImageGeneration generation, CancellationToken cancellationToken = default);
    Task<AIImageGeneration> UpdateAsync(AIImageGeneration generation, CancellationToken cancellationToken = default);
}
