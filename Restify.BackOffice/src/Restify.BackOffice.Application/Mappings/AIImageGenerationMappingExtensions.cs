using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class AIImageGenerationMappingExtensions
{
    public static AIImageGenerationDto ToDto(this AIImageGeneration entity)
    {
        return new AIImageGenerationDto
        {
            Id = entity.Id,
            ProductId = entity.ProductId,
            ProductName = entity.Product?.Name ?? string.Empty,
            PromptTemplateId = entity.PromptTemplateId,
            PromptTemplateName = entity.PromptTemplate?.Name,
            FinalPrompt = entity.FinalPrompt,
            GeneratedImageUrl = entity.GeneratedImageUrl,
            Status = entity.Status.ToString(),
            ProviderUsed = entity.ProviderUsed,
            ErrorMessage = entity.ErrorMessage,
            CostUsd = entity.CostUsd,
            CreatedAt = entity.CreatedAt
        };
    }
}
