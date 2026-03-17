using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAIImagePromptTemplateRepository
{
    Task<AIImagePromptTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<AIImagePromptTemplate>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<AIImagePromptTemplate?> GetDefaultAsync(CancellationToken cancellationToken = default);
    Task<AIImagePromptTemplate> CreateAsync(AIImagePromptTemplate template, CancellationToken cancellationToken = default);
    Task<AIImagePromptTemplate> UpdateAsync(AIImagePromptTemplate template, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
