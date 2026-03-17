using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(o => o.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.CustomerName)
            .HasMaxLength(200);

        builder.Property(o => o.CustomerPhone)
            .HasMaxLength(50);

        builder.Property(o => o.TableId);

        builder.Property(o => o.CustomerId);

        builder.Property(o => o.PaymentStatus)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(o => o.ResponsiblePersonName)
            .HasMaxLength(200);

        builder.Property(o => o.ResponsiblePersonIdentification)
            .HasMaxLength(50);

        builder.Property(o => o.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Tax)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Discount)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Total)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(o => o.Notes)
            .HasMaxLength(1000);

        builder.Property(o => o.CompletedAt);
        builder.Property(o => o.CancelledAt);

        builder.Property(o => o.CancelReason)
            .HasMaxLength(500);

        builder.Property(o => o.TakenBy)
            .HasMaxLength(200);

        builder.Property(o => o.TenantId)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.UpdatedAt);

        // Relaciones
        builder.HasOne(o => o.Table)
            .WithMany()
            .HasForeignKey(o => o.TableId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.Items)
            .WithOne(i => i.Order)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(o => o.TenantId);
        builder.HasIndex(o => new { o.TenantId, o.OrderNumber }).IsUnique();
        builder.HasIndex(o => new { o.TenantId, o.Status });
        builder.HasIndex(o => o.TableId);
        builder.HasIndex(o => o.CustomerId);
        builder.HasIndex(o => o.CreatedAt);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.OrderId)
            .IsRequired();

        builder.Property(i => i.ProductId)
            .IsRequired();

        builder.Property(i => i.Quantity)
            .IsRequired();

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Subtotal)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(i => i.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        builder.Property(i => i.TenantId)
            .IsRequired();

        builder.Property(i => i.CreatedAt)
            .IsRequired();

        builder.Property(i => i.UpdatedAt);

        // Relaciones
        builder.HasOne(i => i.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Modifiers)
            .WithOne(m => m.OrderItem)
            .HasForeignKey(m => m.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.OrderId);
        builder.HasIndex(i => i.ProductId);
    }
}

public class OrderItemModifierConfiguration : IEntityTypeConfiguration<OrderItemModifier>
{
    public void Configure(EntityTypeBuilder<OrderItemModifier> builder)
    {
        builder.ToTable("OrderItemModifiers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.OrderItemId)
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
        builder.HasOne(m => m.OrderItem)
            .WithMany(i => i.Modifiers)
            .HasForeignKey(m => m.OrderItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(m => m.OrderItemId);
    }
}
