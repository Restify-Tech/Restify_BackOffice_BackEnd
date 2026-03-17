using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class CreditNoteConfiguration : IEntityTypeConfiguration<CreditNote>
{
    public void Configure(EntityTypeBuilder<CreditNote> builder)
    {
        builder.ToTable("CreditNotes");

        builder.HasKey(cn => cn.Id);

        builder.Property(cn => cn.CreditNoteNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(cn => cn.Reason)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(cn => cn.CustomerName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(cn => cn.CustomerIdNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(cn => cn.CustomerIdType)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(cn => cn.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(cn => cn.TaxRate)
            .IsRequired()
            .HasPrecision(5, 4);

        builder.Property(cn => cn.Tax)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(cn => cn.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(cn => cn.TenantId)
            .IsRequired();

        builder.Property(cn => cn.CreatedAt)
            .IsRequired();

        // Relaciones
        builder.HasOne(cn => cn.Invoice)
            .WithMany()
            .HasForeignKey(cn => cn.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(cn => cn.Items)
            .WithOne(i => i.CreditNote)
            .HasForeignKey(i => i.CreditNoteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(cn => new { cn.TenantId, cn.CreditNoteNumber }).IsUnique();
        builder.HasIndex(cn => cn.InvoiceId);
        builder.HasIndex(cn => cn.TenantId);
    }
}

public class CreditNoteItemConfiguration : IEntityTypeConfiguration<CreditNoteItem>
{
    public void Configure(EntityTypeBuilder<CreditNoteItem> builder)
    {
        builder.ToTable("CreditNoteItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.TaxRate)
            .IsRequired()
            .HasPrecision(5, 4);

        builder.Property(i => i.TaxAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.TenantId)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        // Relaciones
        builder.HasOne(i => i.CreditNote)
            .WithMany(cn => cn.Items)
            .HasForeignKey(i => i.CreditNoteId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(i => i.CreditNoteId);
        builder.HasIndex(i => i.TenantId);
    }
}
