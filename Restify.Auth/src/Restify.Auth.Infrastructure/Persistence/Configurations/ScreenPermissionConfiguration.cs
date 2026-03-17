using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class ScreenPermissionConfiguration : IEntityTypeConfiguration<ScreenPermission>
{
    public void Configure(EntityTypeBuilder<ScreenPermission> builder)
    {
        builder.ToTable("ScreenPermissions");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.ScreenCode)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(sp => sp.ScreenCode)
            .IsUnique();

        builder.Property(sp => sp.ScreenName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sp => sp.Module)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.Route)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(sp => sp.RequiredPermission)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(sp => sp.DisplayOrder)
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(sp => sp.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
    }
}
