using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.Price)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Sku)
            .HasMaxLength(50);

        builder.Property(p => p.IsActive)
            .IsRequired();

        builder.Property(p => p.IsAvailable)
            .IsRequired();

        builder.Property(p => p.DisplayOrder)
            .IsRequired();

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.CategoryId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        // Relaciones
        builder.HasOne(p => p.Category)
            .WithMany()
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Modifiers)
            .WithOne(m => m.Product)
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.CategoryId);
        builder.HasIndex(p => new { p.TenantId, p.IsActive });
        builder.HasIndex(p => new { p.TenantId, p.Sku }).IsUnique();
    }
}

public class ProductModifierConfiguration : IEntityTypeConfiguration<ProductModifier>
{
    public void Configure(EntityTypeBuilder<ProductModifier> builder)
    {
        builder.ToTable("ProductModifiers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder.Property(m => m.PriceAdjustment)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(m => m.IsRequired)
            .IsRequired();

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.TenantId)
            .IsRequired();

        builder.Property(m => m.ProductId)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        // Índices
        builder.HasIndex(m => m.TenantId);
        builder.HasIndex(m => m.ProductId);
    }
}
