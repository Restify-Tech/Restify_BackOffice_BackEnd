using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class WithholdingVoucherConfiguration : IEntityTypeConfiguration<WithholdingVoucher>
{
    public void Configure(EntityTypeBuilder<WithholdingVoucher> builder)
    {
        builder.ToTable("WithholdingVouchers");

        builder.HasKey(wv => wv.Id);

        builder.Property(wv => wv.VoucherNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(wv => wv.SupplierName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(wv => wv.SupplierRuc)
            .IsRequired()
            .HasMaxLength(13);

        builder.Property(wv => wv.SupplierIdType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(wv => wv.SupportDocType)
            .IsRequired()
            .HasMaxLength(2);

        builder.Property(wv => wv.SupportDocNumber)
            .IsRequired()
            .HasMaxLength(49);

        builder.Property(wv => wv.TotalWithheld)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(wv => wv.TenantId)
            .IsRequired();

        builder.Property(wv => wv.CreatedAt)
            .IsRequired();

        // Relaciones
        builder.HasOne(wv => wv.PurchaseOrder)
            .WithMany()
            .HasForeignKey(wv => wv.PurchaseOrderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(wv => wv.Details)
            .WithOne(d => d.WithholdingVoucher)
            .HasForeignKey(d => d.WithholdingVoucherId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(wv => new { wv.TenantId, wv.VoucherNumber }).IsUnique();
        builder.HasIndex(wv => wv.TenantId);
    }
}

public class WithholdingDetailConfiguration : IEntityTypeConfiguration<WithholdingDetail>
{
    public void Configure(EntityTypeBuilder<WithholdingDetail> builder)
    {
        builder.ToTable("WithholdingDetails");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.TaxCode)
            .IsRequired()
            .HasMaxLength(5);

        builder.Property(d => d.RetentionCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(d => d.TaxBase)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.RetentionPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(d => d.RetentionAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        // Relaciones
        builder.HasOne(d => d.WithholdingVoucher)
            .WithMany(wv => wv.Details)
            .HasForeignKey(d => d.WithholdingVoucherId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(d => d.WithholdingVoucherId);
        builder.HasIndex(d => d.TenantId);
    }
}
