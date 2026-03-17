using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class DeliveryCooperativeConfiguration : IEntityTypeConfiguration<DeliveryCooperative>
{
    public void Configure(EntityTypeBuilder<DeliveryCooperative> builder)
    {
        builder.ToTable("DeliveryCooperatives");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Ruc)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.ContactName)
            .HasMaxLength(200);

        builder.Property(x => x.Phone)
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .HasMaxLength(200);

        builder.Property(x => x.Address)
            .HasMaxLength(500);

        builder.Property(x => x.CommissionPercentage)
            .HasPrecision(5, 2);

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // Relaciones
        builder.HasMany(x => x.Drivers)
            .WithOne(x => x.Cooperative)
            .HasForeignKey(x => x.CooperativeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.Ruc }).IsUnique();
    }
}
