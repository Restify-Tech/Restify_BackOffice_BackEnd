using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

public enum AIImageGenerationStatus
{
    Pending = 1,
    Generating = 2,
    Completed = 3,
    Failed = 4
}

public class AIImageGeneration : TenantEntity
{
    public Guid ProductId { get; set; }
    public Guid? PromptTemplateId { get; set; }
    public string FinalPrompt { get; set; } = string.Empty;
    public string? GeneratedImageUrl { get; set; }
    public AIImageGenerationStatus Status { get; set; } = AIImageGenerationStatus.Pending;
    public string? ProviderUsed { get; set; } // "dall-e-3"
    public string? ErrorMessage { get; set; }
    public decimal? CostUsd { get; set; }

    // Navigation
    public Product Product { get; set; } = null!;
    public AIImagePromptTemplate? PromptTemplate { get; set; }
}
