using Restify.BackOffice.Application.DTOs;
using Restify.Core.Application.DTOs.Common;

namespace Restify.BackOffice.Application.Interfaces;

public interface IAIImageService
{
    Task<Result<AIImageGenerationDto>> GenerateImageAsync(GenerateImageRequest request, CancellationToken cancellationToken = default);
    Task<Result<AIImageGenerationDto>> GetGenerationByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<AIImageGenerationDto>>> GetGenerationsByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);
}
