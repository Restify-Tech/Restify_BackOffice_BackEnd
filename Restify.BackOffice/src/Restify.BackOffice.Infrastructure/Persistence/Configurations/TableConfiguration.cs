using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("Tables");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Number)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(t => t.Name)
            .HasMaxLength(200);

        builder.Property(t => t.Capacity)
            .IsRequired();

        builder.Property(t => t.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(t => t.IsActive)
            .IsRequired();

        builder.Property(t => t.PositionX);
        builder.Property(t => t.PositionY);

        builder.Property(t => t.Shape)
            .HasConversion<int>();

        builder.Property(t => t.Zone)
            .HasMaxLength(100);

        builder.Property(t => t.Notes)
            .HasMaxLength(500);

        builder.Property(t => t.CurrentOrderId);

        builder.Property(t => t.CurrentCustomerName)
            .HasMaxLength(200);

        builder.Property(t => t.OccupiedSince);

        builder.Property(t => t.TenantId)
            .IsRequired();

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt);

        // Relacion con sucursal
        builder.HasOne(t => t.Branch)
            .WithMany(b => b.Tables)
            .HasForeignKey(t => t.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Índices
        builder.HasIndex(t => t.TenantId);
        builder.HasIndex(t => new { t.TenantId, t.Number }).IsUnique();
        builder.HasIndex(t => new { t.TenantId, t.Status });
        builder.HasIndex(t => new { t.TenantId, t.Zone });
        builder.HasIndex(t => t.BranchId);
    }
}
