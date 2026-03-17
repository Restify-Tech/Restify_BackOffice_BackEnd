using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.OrderId)
            .IsRequired();

        builder.Property(i => i.CustomerName)
            .HasMaxLength(200);

        builder.Property(i => i.CustomerIdNumber)
            .HasMaxLength(50);

        builder.Property(i => i.CustomerIdType)
            .HasConversion<int?>();

        builder.Property(i => i.CustomerEmail)
            .HasMaxLength(200);

        builder.Property(i => i.CustomerPhone)
            .HasMaxLength(50);

        builder.Property(i => i.CustomerAddress)
            .HasMaxLength(500);

        builder.Property(i => i.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.TaxRate)
            .IsRequired()
            .HasPrecision(5, 4);

        builder.Property(i => i.Tax)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.DiscountPercentage)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(i => i.DiscountAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.PaymentMethod)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.CancelReason)
            .HasMaxLength(500);

        builder.Property(i => i.IssuedBy)
            .HasMaxLength(200);

        builder.Property(i => i.PaidAt);
        builder.Property(i => i.CancelledAt);

        builder.Property(i => i.ElectronicAuthorizationCode)
            .HasMaxLength(100);

        builder.Property(i => i.ElectronicAccessKey)
            .HasMaxLength(200);

        builder.Property(i => i.TenantId)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // Relaciones
        builder.HasOne(i => i.Order)
            .WithMany()
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Items)
            .WithOne(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => new { i.TenantId, i.InvoiceNumber }).IsUnique();
        builder.HasIndex(i => new { i.TenantId, i.OrderId }).IsUnique();
        builder.HasIndex(i => new { i.TenantId, i.Status });
        builder.HasIndex(i => new { i.TenantId, i.PaymentMethod });
        builder.HasIndex(i => i.CreatedAt);
        builder.HasIndex(i => i.PaidAt);
    }
}

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(it => it.Id);

        builder.Property(it => it.InvoiceId)
            .IsRequired();

        builder.Property(it => it.ProductId)
            .IsRequired();

        builder.Property(it => it.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(it => it.Quantity)
            .IsRequired();

        builder.Property(it => it.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(it => it.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(it => it.TenantId)
            .IsRequired();

        builder.Property(it => it.CreatedAt)
            .IsRequired();

        builder.Property(it => it.UpdatedAt);

        // Relaciones
        builder.HasOne(it => it.Invoice)
            .WithMany(i => i.Items)
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(it => it.Modifiers)
            .WithOne(m => m.InvoiceItem)
            .HasForeignKey(m => m.InvoiceItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(it => it.TenantId);
        builder.HasIndex(it => it.InvoiceId);
        builder.HasIndex(it => it.ProductId);
    }
}

public class InvoiceItemModifierConfiguration : IEntityTypeConfiguration<InvoiceItemModifier>
{
    public void Configure(EntityTypeBuilder<InvoiceItemModifier> builder)
    {
        builder.ToTable("InvoiceItemModifiers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.InvoiceItemId)
            .IsRequired();

        builder.Property(m => m.ModifierName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(m => m.PriceAdjustment)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        // Relaciones
        builder.HasOne(m => m.InvoiceItem)
            .WithMany(it => it.Modifiers)
            .HasForeignKey(m => m.InvoiceItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(m => m.InvoiceItemId);
    }
}
