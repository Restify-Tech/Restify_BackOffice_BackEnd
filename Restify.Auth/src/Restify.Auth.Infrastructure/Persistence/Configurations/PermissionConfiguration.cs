using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("Permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Code)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(300);

        builder.Property(p => p.Module)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasMany(p => p.RolePermissions)
            .WithOne(rp => rp.Permission)
            .HasForeignKey(rp => rp.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
