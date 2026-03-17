using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class DeliveryDriverConfiguration : IEntityTypeConfiguration<DeliveryDriver>
{
    public void Configure(EntityTypeBuilder<DeliveryDriver> builder)
    {
        builder.ToTable("DeliveryDrivers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.IdentificationNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Phone)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.VehiclePlate)
            .HasMaxLength(20);

        builder.Property(x => x.VehicleDescription)
            .HasMaxLength(200);

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(500);

        builder.Property(x => x.PasswordHash)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.RefreshToken)
            .HasMaxLength(500);

        builder.Property(x => x.Rating)
            .HasPrecision(3, 2);

        builder.Property(x => x.DriverType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.IsActive)
            .IsRequired();

        builder.Property(x => x.TenantId)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.UpdatedAt);

        // Relaciones
        builder.HasMany(x => x.Deliveries)
            .WithOne(x => x.Driver)
            .HasForeignKey(x => x.DriverId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Cooperative)
            .WithMany(x => x.Drivers)
            .HasForeignKey(x => x.CooperativeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => new { x.TenantId, x.Email }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.IdentificationNumber }).IsUnique();
        builder.HasIndex(x => x.CooperativeId);
    }
}
