using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class DeductionTypeConfiguration : IEntityTypeConfiguration<DeductionType>
{
    public void Configure(EntityTypeBuilder<DeductionType> builder)
    {
        builder.ToTable("DeductionTypes");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.Property(d => d.CalculationType)
            .IsRequired();

        builder.Property(d => d.DefaultValue)
            .IsRequired()
            .HasPrecision(10, 4);

        builder.Property(d => d.IsRequired)
            .IsRequired();

        builder.Property(d => d.AppliesTo)
            .IsRequired();

        builder.Property(d => d.IsActive)
            .IsRequired();

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);

        // Indices
        builder.HasIndex(d => d.TenantId);
        builder.HasIndex(d => new { d.TenantId, d.Name }).IsUnique();
    }
}
