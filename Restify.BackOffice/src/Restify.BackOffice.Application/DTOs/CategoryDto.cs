namespace Restify.BackOffice.Application.DTOs;

/// <summary>
/// DTO de categoría
/// </summary>
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public List<CategoryDto>? SubCategories { get; set; }
}

/// <summary>
/// Request para crear categoría
/// </summary>
public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; }
}

/// <summary>
/// Request para actualizar categoría
/// </summary>
public class UpdateCategoryRequest : CreateCategoryRequest
{
    public Guid Id { get; set; }
}
