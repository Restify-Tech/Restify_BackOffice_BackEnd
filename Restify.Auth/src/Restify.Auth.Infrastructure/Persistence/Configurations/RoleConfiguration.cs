using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.NormalizedName)
            .IsRequired()
            .HasMaxLength(50);

        // Nombre normalizado único por tenant
        builder.HasIndex(r => new { r.TenantId, r.NormalizedName })
            .IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(200);

        builder.Property(r => r.IsSystem)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(r => r.Tenant)
            .WithMany(t => t.Roles)
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(r => r.RolePermissions)
            .WithOne(rp => rp.Role)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
