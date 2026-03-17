using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class AIImagePromptTemplateMappingExtensions
{
    public static AIImagePromptTemplateDto ToDto(this AIImagePromptTemplate entity)
    {
        return new AIImagePromptTemplateDto
        {
            Id = entity.Id,
            Name = entity.Name,
            PromptTemplate = entity.PromptTemplate,
            Style = entity.Style.ToString(),
            IsDefault = entity.IsDefault,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static AIImagePromptTemplate ToEntity(this CreateAIImagePromptTemplateRequest request, Guid tenantId)
    {
        return new AIImagePromptTemplate
        {
            TenantId = tenantId,
            Name = request.Name,
            PromptTemplate = request.PromptTemplate,
            Style = (AIImageStyle)request.Style,
            IsDefault = request.IsDefault,
            IsActive = request.IsActive
        };
    }

    public static void UpdateFrom(this AIImagePromptTemplate entity, UpdateAIImagePromptTemplateRequest request)
    {
        entity.Name = request.Name;
        entity.PromptTemplate = request.PromptTemplate;
        entity.Style = (AIImageStyle)request.Style;
        entity.IsDefault = request.IsDefault;
        entity.IsActive = request.IsActive;
    }
}
