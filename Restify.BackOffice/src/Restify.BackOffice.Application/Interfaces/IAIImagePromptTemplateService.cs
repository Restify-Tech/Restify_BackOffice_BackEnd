using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAIImagePromptTemplateService
{
    Task<Result<AIImagePromptTemplateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AIImagePromptTemplateDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<AIImagePromptTemplateDto>> CreateAsync(CreateAIImagePromptTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<AIImagePromptTemplateDto>> UpdateAsync(Guid id, UpdateAIImagePromptTemplateRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
