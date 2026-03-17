using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class CategoryMappingExtensions
{
    public static CategoryDto ToDto(this Category entity, bool includeSubCategories = false)
    {
        return new CategoryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Icon = entity.Icon,
            ImageUrl = entity.ImageUrl,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            ParentCategoryId = entity.ParentCategoryId,
            ParentCategoryName = entity.ParentCategory?.Name,
            SubCategories = includeSubCategories && entity.SubCategories?.Count > 0
                ? entity.SubCategories.Select(s => s.ToDto(true)).ToList()
                : null
        };
    }

    public static Category ToEntity(this CreateCategoryRequest request)
    {
        return new Category
        {
            Name = request.Name,
            Description = request.Description,
            Icon = request.Icon,
            ImageUrl = request.ImageUrl,
            DisplayOrder = request.DisplayOrder,
            IsActive = request.IsActive,
            ParentCategoryId = request.ParentCategoryId
        };
    }

    public static void UpdateFrom(this Category entity, UpdateCategoryRequest request)
    {
        entity.Name = request.Name;
        entity.Description = request.Description;
        entity.Icon = request.Icon;
        entity.ImageUrl = request.ImageUrl;
        entity.DisplayOrder = request.DisplayOrder;
        entity.IsActive = request.IsActive;
        entity.ParentCategoryId = request.ParentCategoryId;
    }
}
