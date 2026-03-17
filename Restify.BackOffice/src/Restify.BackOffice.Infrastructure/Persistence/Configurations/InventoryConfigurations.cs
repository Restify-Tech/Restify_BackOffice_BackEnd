using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("Suppliers");
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.TaxId).HasMaxLength(50);
        builder.Property(s => s.ContactName).HasMaxLength(200);
        builder.Property(s => s.Email).HasMaxLength(200);
        builder.Property(s => s.Phone).HasMaxLength(50);
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Notes).HasMaxLength(1000);
        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.Name);
    }
}

public class InventoryItemConfiguration : IEntityTypeConfiguration<InventoryItem>
{
    public void Configure(EntityTypeBuilder<InventoryItem> builder)
    {
        builder.ToTable("InventoryItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.CurrentStock).HasColumnType("decimal(18,3)");
        builder.Property(i => i.Unit).IsRequired().HasMaxLength(50);
        builder.Property(i => i.MinimumStock).HasColumnType("decimal(18,3)");
        builder.Property(i => i.MaximumStock).HasColumnType("decimal(18,3)");
        builder.Property(i => i.AverageCost).HasColumnType("decimal(18,4)");
        builder.Property(i => i.LastPurchaseCost).HasColumnType("decimal(18,4)");
        builder.Property(i => i.StorageLocation).HasMaxLength(100);
        builder.Property(i => i.CostMethod).IsRequired().HasConversion<int>();
        builder.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(i => i.IsLowStock);
        builder.Ignore(i => i.IsOutOfStock);
        builder.Ignore(i => i.TotalValue);
        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.ProductId).IsUnique();
    }
}

public class InventoryMovementConfiguration : IEntityTypeConfiguration<InventoryMovement>
{
    public void Configure(EntityTypeBuilder<InventoryMovement> builder)
    {
        builder.ToTable("InventoryMovements");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Type).IsRequired().HasConversion<int>();
        builder.Property(m => m.Quantity).HasColumnType("decimal(18,3)");
        builder.Property(m => m.UnitCost).HasColumnType("decimal(18,4)");
        builder.Property(m => m.TotalCost).HasColumnType("decimal(18,4)");
        builder.Property(m => m.PreviousStock).HasColumnType("decimal(18,3)");
        builder.Property(m => m.NewStock).HasColumnType("decimal(18,3)");
        builder.Property(m => m.ReferenceType).HasMaxLength(50);
        builder.Property(m => m.Description).IsRequired().HasMaxLength(500);
        builder.Property(m => m.Notes).HasMaxLength(1000);
        builder.Property(m => m.RegisteredBy).HasMaxLength(100);
        builder.HasOne(m => m.InventoryItem).WithMany(i => i.Movements).HasForeignKey(m => m.InventoryItemId).OnDelete(DeleteBehavior.Cascade);
        builder.Ignore(m => m.IsInbound);
        builder.Ignore(m => m.IsOutbound);
        builder.HasIndex(m => m.TenantId);
        builder.HasIndex(m => m.InventoryItemId);
        builder.HasIndex(m => m.Type);
        builder.HasIndex(m => m.MovementDate);
    }
}

public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.ToTable("PurchaseOrders");
        builder.HasKey(po => po.Id);
        builder.Property(po => po.OrderNumber).IsRequired().HasMaxLength(50);
        builder.Property(po => po.Status).IsRequired().HasConversion<int>();
        builder.Property(po => po.Subtotal).HasColumnType("decimal(18,2)");
        builder.Property(po => po.Tax).HasColumnType("decimal(18,2)");
        builder.Property(po => po.Discount).HasColumnType("decimal(18,2)");
        builder.Property(po => po.Total).HasColumnType("decimal(18,2)");
        builder.Property(po => po.Notes).HasMaxLength(1000);
        builder.Property(po => po.OrderedBy).HasMaxLength(100);
        builder.Property(po => po.ReceivedBy).HasMaxLength(100);
        builder.Property(po => po.SupplierInvoiceNumber).HasMaxLength(100);
        builder.HasOne(po => po.Supplier).WithMany(s => s.PurchaseOrders).HasForeignKey(po => po.SupplierId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(po => po.TenantId);
        builder.HasIndex(po => po.OrderNumber).IsUnique();
        builder.HasIndex(po => po.Status);
        builder.HasIndex(po => po.OrderDate);
    }
}

public class PurchaseOrderItemConfiguration : IEntityTypeConfiguration<PurchaseOrderItem>
{
    public void Configure(EntityTypeBuilder<PurchaseOrderItem> builder)
    {
        builder.ToTable("PurchaseOrderItems");
        builder.HasKey(i => i.Id);
        builder.Property(i => i.QuantityOrdered).HasColumnType("decimal(18,3)");
        builder.Property(i => i.QuantityReceived).HasColumnType("decimal(18,3)");
        builder.Property(i => i.Unit).IsRequired().HasMaxLength(50);
        builder.Property(i => i.UnitCost).HasColumnType("decimal(18,4)");
        builder.Property(i => i.Subtotal).HasColumnType("decimal(18,2)");
        builder.Property(i => i.Notes).HasMaxLength(500);
        builder.HasOne(i => i.PurchaseOrder).WithMany(po => po.Items).HasForeignKey(i => i.PurchaseOrderId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(i => i.Product).WithMany().HasForeignKey(i => i.ProductId).OnDelete(DeleteBehavior.Restrict);
        builder.Ignore(i => i.QuantityPending);
        builder.Ignore(i => i.IsFullyReceived);
        builder.HasIndex(i => i.TenantId);
        builder.HasIndex(i => i.PurchaseOrderId);
    }
}
