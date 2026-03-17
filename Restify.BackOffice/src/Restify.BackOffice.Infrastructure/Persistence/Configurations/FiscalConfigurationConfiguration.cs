using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class FiscalConfigurationConfiguration : IEntityTypeConfiguration<FiscalConfiguration>
{
    public void Configure(EntityTypeBuilder<FiscalConfiguration> builder)
    {
        builder.ToTable("FiscalConfigurations");

        builder.HasKey(fc => fc.Id);

        builder.Property(fc => fc.Environment)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(fc => fc.Ruc)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(fc => fc.BusinessName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(fc => fc.TradeName)
            .HasMaxLength(300);

        builder.Property(fc => fc.MainAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(fc => fc.EstablishmentAddress)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(fc => fc.ContribuyenteEspecial)
            .HasMaxLength(20);

        builder.Property(fc => fc.Establishment)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(fc => fc.EmissionPoint)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(fc => fc.CertificatePassword)
            .HasMaxLength(500);

        builder.Property(fc => fc.TenantId)
            .IsRequired();

        builder.Property(fc => fc.CreatedAt)
            .IsRequired();

        // Indice unico: 1 configuracion por tenant
        builder.HasIndex(fc => fc.TenantId).IsUnique();
    }
}
