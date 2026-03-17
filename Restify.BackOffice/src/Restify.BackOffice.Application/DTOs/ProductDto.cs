namespace Restify.BackOffice.Application.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public List<ProductModifierDto> Modifiers { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ProductModifierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PriceAdjustment { get; set; }
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; }
}

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public int DisplayOrder { get; set; }
    public Guid CategoryId { get; set; }
    public List<CreateProductModifierRequest>? Modifiers { get; set; }
}

public class CreateProductModifierRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal PriceAdjustment { get; set; }
    public bool IsRequired { get; set; } = false;
    public bool IsActive { get; set; } = true;
}

public class UpdateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public string? Sku { get; set; }
    public bool IsActive { get; set; }
    public bool IsAvailable { get; set; }
    public int DisplayOrder { get; set; }
    public Guid CategoryId { get; set; }
    public List<CreateProductModifierRequest>? Modifiers { get; set; }
}
