using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class GlobalModifierConfiguration : IEntityTypeConfiguration<GlobalModifier>
{
    public void Configure(EntityTypeBuilder<GlobalModifier> builder)
    {
        builder.ToTable("GlobalModifiers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.Description)
            .HasMaxLength(500);

        builder.Property(m => m.DefaultPriceAdjustment)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(m => m.IsActive)
            .IsRequired();

        builder.Property(m => m.DisplayOrder)
            .IsRequired();

        builder.Property(m => m.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(m => m.TenantId)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        // Relaciones
        builder.HasMany(m => m.Products)
            .WithOne(gmp => gmp.GlobalModifier)
            .HasForeignKey(gmp => gmp.GlobalModifierId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(m => m.TenantId);
        builder.HasIndex(m => new { m.TenantId, m.Type });
        builder.HasIndex(m => new { m.TenantId, m.IsActive });
    }
}

public class GlobalModifierProductConfiguration : IEntityTypeConfiguration<GlobalModifierProduct>
{
    public void Configure(EntityTypeBuilder<GlobalModifierProduct> builder)
    {
        builder.ToTable("GlobalModifierProducts");

        builder.HasKey(gmp => gmp.Id);

        builder.Property(gmp => gmp.CustomPriceAdjustment)
            .HasPrecision(18, 2);

        builder.Property(gmp => gmp.IsRequired)
            .IsRequired();

        builder.Property(gmp => gmp.IsActive)
            .IsRequired();

        builder.Property(gmp => gmp.TenantId)
            .IsRequired();

        builder.Property(gmp => gmp.GlobalModifierId)
            .IsRequired();

        builder.Property(gmp => gmp.ProductId)
            .IsRequired();

        builder.Property(gmp => gmp.CreatedAt)
            .IsRequired();

        builder.Property(gmp => gmp.UpdatedAt);

        // Relaciones
        builder.HasOne(gmp => gmp.GlobalModifier)
            .WithMany(gm => gm.Products)
            .HasForeignKey(gmp => gmp.GlobalModifierId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(gmp => gmp.Product)
            .WithMany(p => p.GlobalModifiers)
            .HasForeignKey(gmp => gmp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(gmp => gmp.TenantId);
        builder.HasIndex(gmp => gmp.GlobalModifierId);
        builder.HasIndex(gmp => gmp.ProductId);
        builder.HasIndex(gmp => new { gmp.GlobalModifierId, gmp.ProductId }).IsUnique();
    }
}
