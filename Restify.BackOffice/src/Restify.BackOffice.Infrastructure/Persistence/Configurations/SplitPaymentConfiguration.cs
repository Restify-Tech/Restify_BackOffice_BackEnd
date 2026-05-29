using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class SplitPaymentConfiguration : IEntityTypeConfiguration<SplitPayment>
{
    public void Configure(EntityTypeBuilder<SplitPayment> builder)
    {
        builder.ToTable("split_payments", "backoffice");

        builder.HasKey(sp => sp.Id);

        builder.Property(sp => sp.OrderId)
            .IsRequired();

        builder.Property(sp => sp.TotalAmount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(sp => sp.SplitCount)
            .IsRequired();

        builder.Property(sp => sp.Status)
            .IsRequired();

        builder.Property(sp => sp.CreatedByUserId)
            .HasMaxLength(256);

        builder.Property(sp => sp.TenantId)
            .IsRequired();

        builder.Property(sp => sp.CreatedAt)
            .IsRequired();

        builder.Property(sp => sp.UpdatedAt);

        // Relaciones
        builder.HasOne(sp => sp.Order)
            .WithMany()
            .HasForeignKey(sp => sp.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(sp => sp.Items)
            .WithOne(i => i.SplitPayment)
            .HasForeignKey(i => i.SplitPaymentId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(sp => sp.TenantId);
        builder.HasIndex(sp => sp.OrderId);
        builder.HasIndex(sp => new { sp.TenantId, sp.OrderId });
        builder.HasIndex(sp => new { sp.TenantId, sp.Status });
    }
}

public class SplitPaymentItemConfiguration : IEntityTypeConfiguration<SplitPaymentItem>
{
    public void Configure(EntityTypeBuilder<SplitPaymentItem> builder)
    {
        builder.ToTable("split_payment_items", "backoffice");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.SplitPaymentId)
            .IsRequired();

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.PaymentMethod)
            .IsRequired();

        builder.Property(i => i.PaidBy)
            .HasMaxLength(256);

        builder.Property(i => i.PaidAt);

        builder.Property(i => i.Status)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // Indices
        builder.HasIndex(i => i.SplitPaymentId);
        builder.HasIndex(i => i.Status);
    }
}
