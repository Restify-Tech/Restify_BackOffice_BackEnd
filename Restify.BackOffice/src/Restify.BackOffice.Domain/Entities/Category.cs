using Restify.Core.Domain.Entities;

namespace Restify.BackOffice.Domain.Entities;

/// <summary>
/// Categoría de productos del menú
/// </summary>
public class Category : TenantEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string? ImageUrl { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ParentCategoryId { get; set; }

    // Navegación
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
}
