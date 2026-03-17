using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Icon)
            .HasMaxLength(50);

        builder.Property(x => x.ImageUrl)
            .HasMaxLength(500);

        // Índice único por TenantId + Name
        builder.HasIndex(x => new { x.TenantId, x.Name })
            .IsUnique();

        // Índice para orden
        builder.HasIndex(x => new { x.TenantId, x.ParentCategoryId, x.DisplayOrder });

        // Relación recursiva para subcategorías
        builder.HasOne(x => x.ParentCategory)
            .WithMany(x => x.SubCategories)
            .HasForeignKey(x => x.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
