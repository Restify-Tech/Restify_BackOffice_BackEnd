using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(t => t.Slug)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(t => t.Slug)
            .IsUnique();

        builder.Property(t => t.Ruc)
            .HasMaxLength(20);

        builder.Property(t => t.BusinessName)
            .HasMaxLength(300);

        builder.Property(t => t.Address)
            .HasMaxLength(500);

        builder.Property(t => t.Phone)
            .HasMaxLength(20);

        builder.Property(t => t.Email)
            .HasMaxLength(200);

        builder.Property(t => t.LogoUrl)
            .HasMaxLength(500);

        builder.Property(t => t.PrimaryColor)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("#1976D2");

        builder.Property(t => t.SecondaryColor)
            .IsRequired()
            .HasMaxLength(10)
            .HasDefaultValue("#FF9800");

        builder.Property(t => t.Currency)
            .IsRequired()
            .HasMaxLength(3)
            .HasDefaultValue("USD");

        builder.Property(t => t.TaxPercentage)
            .HasPrecision(5, 2)
            .HasDefaultValue(15.00m);

        builder.Property(t => t.TimeZone)
            .IsRequired()
            .HasMaxLength(50)
            .HasDefaultValue("America/Guayaquil");

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        // Relaciones
        builder.HasMany(t => t.Users)
            .WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Roles)
            .WithOne(r => r.Tenant)
            .HasForeignKey(r => r.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
