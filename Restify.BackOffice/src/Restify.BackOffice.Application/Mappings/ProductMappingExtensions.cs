using Restify.BackOffice.Application.DTOs;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Application.Mappings;

public static class ProductMappingExtensions
{
    public static ProductDto ToDto(this Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            ImageUrl = product.ImageUrl,
            Price = product.Price,
            Sku = product.Sku,
            IsActive = product.IsActive,
            IsAvailable = product.IsAvailable,
            DisplayOrder = product.DisplayOrder,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            Modifiers = product.Modifiers.Select(m => m.ToDto()).ToList(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    public static ProductModifierDto ToDto(this ProductModifier modifier)
    {
        return new ProductModifierDto
        {
            Id = modifier.Id,
            Name = modifier.Name,
            Description = modifier.Description,
            PriceAdjustment = modifier.PriceAdjustment,
            IsRequired = modifier.IsRequired,
            IsActive = modifier.IsActive
        };
    }

    public static Product ToEntity(this CreateProductRequest request, Guid tenantId)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Name = request.Name,
            Description = request.Description,
            ImageUrl = request.ImageUrl,
            Price = request.Price,
            Sku = request.Sku,
            IsActive = request.IsActive,
            IsAvailable = request.IsAvailable,
            DisplayOrder = request.DisplayOrder,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        if (request.Modifiers != null && request.Modifiers.Any())
        {
            product.Modifiers = request.Modifiers.Select(m => new ProductModifier
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                ProductId = product.Id,
                Name = m.Name,
                Description = m.Description,
                PriceAdjustment = m.PriceAdjustment,
                IsRequired = m.IsRequired,
                IsActive = m.IsActive,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        return product;
    }

    public static void UpdateFromRequest(this Product product, UpdateProductRequest request)
    {
        product.Name = request.Name;
        product.Description = request.Description;
        product.ImageUrl = request.ImageUrl;
        product.Price = request.Price;
        product.Sku = request.Sku;
        product.IsActive = request.IsActive;
        product.IsAvailable = request.IsAvailable;
        product.DisplayOrder = request.DisplayOrder;
        product.CategoryId = request.CategoryId;
        product.UpdatedAt = DateTime.UtcNow;

        // Actualizar modificadores: eliminar los existentes y agregar los nuevos
        product.Modifiers.Clear();
        
        if (request.Modifiers != null && request.Modifiers.Any())
        {
            foreach (var modRequest in request.Modifiers)
            {
                product.Modifiers.Add(new ProductModifier
                {
                    Id = Guid.NewGuid(),
                    TenantId = product.TenantId,
                    ProductId = product.Id,
                    Name = modRequest.Name,
                    Description = modRequest.Description,
                    PriceAdjustment = modRequest.PriceAdjustment,
                    IsRequired = modRequest.IsRequired,
                    IsActive = modRequest.IsActive,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }
    }
}
