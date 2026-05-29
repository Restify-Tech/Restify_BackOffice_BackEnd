using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("Plans");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.MonthlyPrice)
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.AnnualPrice)
            .HasColumnType("decimal(10,2)");

        builder.Property(x => x.Color)
            .HasMaxLength(10);

        builder.HasMany(x => x.PlanScreenPermissions)
            .WithOne(x => x.Plan)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tenants)
            .WithOne(x => x.Plan)
            .HasForeignKey(x => x.PlanId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
