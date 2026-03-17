using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class DeliveryConfiguration : IEntityTypeConfiguration<Delivery>
{
    public void Configure(EntityTypeBuilder<Delivery> builder)
    {
        builder.ToTable("Deliveries");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.OrderId)
            .IsRequired();

        builder.Property(x => x.DriverId);

        builder.Property(x => x.DeliveryAddress)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.DeliveryNotes)
            .HasMaxLength(500);

        builder.Property(x => x.DeliveryFee)
            .HasPrecision(18, 2);

        builder.Property(x => x.DeliveryProofUrl)
            .HasMaxLength(500);

        builder.Property(x => x.CustomerFeedback)
            .HasMaxLength(500);

        builder.Property(x => x.DriverLatitude)
            .HasPrecision(10, 7);

        builder.Property(x => x.DriverLongitude)
            .HasPrecision(10, 7);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // Relaciones
        builder.HasOne(x => x.Order)
            .WithMany()
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Driver)
            .WithMany(d => d.Deliveries)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.Status });
        builder.HasIndex(x => x.OrderId);
        builder.HasIndex(x => x.DriverId);
    }
}
