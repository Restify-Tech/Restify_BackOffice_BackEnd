using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderId)
            .IsRequired();

        builder.Property(p => p.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(p => p.Method)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(p => p.GatewayTransactionId)
            .HasMaxLength(500);

        builder.Property(p => p.GatewayResponse)
            .HasMaxLength(2000);

        builder.Property(p => p.PayerName)
            .HasMaxLength(200);

        builder.Property(p => p.PayerIdentification)
            .HasMaxLength(50);

        builder.Property(p => p.ProcessedAt);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(500);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        // Relaciones
        builder.HasOne(p => p.Order)
            .WithMany()
            .HasForeignKey(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Invoice)
            .WithMany()
            .HasForeignKey(p => p.InvoiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Items)
            .WithOne(i => i.Payment)
            .HasForeignKey(i => i.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => p.OrderId);
        builder.HasIndex(p => new { p.TenantId, p.Status });
        builder.HasIndex(p => p.GatewayTransactionId);
    }
}

public class PaymentItemConfiguration : IEntityTypeConfiguration<PaymentItem>
{
    public void Configure(EntityTypeBuilder<PaymentItem> builder)
    {
        builder.ToTable("PaymentItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.PaymentId)
            .IsRequired();

        builder.Property(i => i.OrderItemId)
            .IsRequired();

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // Relaciones
        builder.HasOne(i => i.Payment)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.PaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.OrderItem)
            .WithMany()
            .HasForeignKey(i => i.OrderItemId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(i => i.PaymentId);
        builder.HasIndex(i => i.OrderItemId);
    }
}
