namespace Restify.BackOffice.Application.DTOs;

public class AIImageGenerationDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid? PromptTemplateId { get; set; }
    public string? PromptTemplateName { get; set; }
    public string FinalPrompt { get; set; } = string.Empty;
    public string? GeneratedImageUrl { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ProviderUsed { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal? CostUsd { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GenerateImageRequest
{
    public Guid ProductId { get; set; }
    public Guid? PromptTemplateId { get; set; }
    public string? CustomPrompt { get; set; }
}
