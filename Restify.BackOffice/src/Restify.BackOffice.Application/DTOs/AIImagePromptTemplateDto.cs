namespace Restify.BackOffice.Application.DTOs;

public class AIImagePromptTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CreateAIImagePromptTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty;
    public int Style { get; set; } = 6; // FoodPhotography
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

public class UpdateAIImagePromptTemplateRequest
{
    public string Name { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty;
    public int Style { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
}
