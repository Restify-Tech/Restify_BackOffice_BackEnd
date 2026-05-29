using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class PlanScreenPermissionConfiguration : IEntityTypeConfiguration<PlanScreenPermission>
{
    public void Configure(EntityTypeBuilder<PlanScreenPermission> builder)
    {
        builder.ToTable("PlanScreenPermissions");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ScreenCode)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(x => new { x.PlanId, x.ScreenCode })
            .IsUnique();
    }
}
