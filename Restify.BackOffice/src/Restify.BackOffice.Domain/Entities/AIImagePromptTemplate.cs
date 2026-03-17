using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

public enum AIImageStyle
{
    Photorealistic = 1,
    Watercolor = 2,
    Minimalist = 3,
    Cartoon = 4,
    Sketch = 5,
    FoodPhotography = 6
}

public class AIImagePromptTemplate : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string PromptTemplate { get; set; } = string.Empty; // e.g. "A professional photo of {product_name}, restaurant style..."
    public AIImageStyle Style { get; set; } = AIImageStyle.FoodPhotography;
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<AIImageGeneration> Generations { get; set; } = new List<AIImageGeneration>();
}
