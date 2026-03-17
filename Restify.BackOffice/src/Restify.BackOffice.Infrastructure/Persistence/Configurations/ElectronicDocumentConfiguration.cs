using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class ElectronicDocumentConfiguration : IEntityTypeConfiguration<ElectronicDocument>
{
    public void Configure(EntityTypeBuilder<ElectronicDocument> builder)
    {
        builder.ToTable("ElectronicDocuments");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.DocumentType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.AccessKey)
            .IsRequired()
            .HasMaxLength(49);

        builder.Property(e => e.Establishment)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(e => e.EmissionPoint)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(e => e.Sequential)
            .IsRequired();

        builder.Property(e => e.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.Environment)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(e => e.AuthorizationCode)
            .HasMaxLength(49);

        builder.Property(e => e.RidePdfUrl)
            .HasMaxLength(500);

        builder.Property(e => e.SriResponse)
            .HasColumnType("jsonb");

        builder.Property(e => e.SriErrors)
            .HasColumnType("jsonb");

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        // Relaciones
        builder.HasOne(e => e.Invoice)
            .WithMany()
            .HasForeignKey(e => e.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.CreditNote)
            .WithMany()
            .HasForeignKey(e => e.CreditNoteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.WithholdingVoucher)
            .WithMany()
            .HasForeignKey(e => e.WithholdingVoucherId)
            .OnDelete(DeleteBehavior.SetNull);

        // Indices
        builder.HasIndex(e => e.AccessKey).IsUnique();
        builder.HasIndex(e => new { e.TenantId, e.Status });
        builder.HasIndex(e => new { e.TenantId, e.DocumentType, e.Sequential }).IsUnique();
        builder.HasIndex(e => e.InvoiceId);
        builder.HasIndex(e => e.CreditNoteId);
        builder.HasIndex(e => e.WithholdingVoucherId);
    }
}
