using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class DispatchApprovalConfiguration : IEntityTypeConfiguration<DispatchApproval>
{
    public void Configure(EntityTypeBuilder<DispatchApproval> builder)
    {
        builder.ToTable("DispatchApprovals");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.OrderId)
            .IsRequired();

        builder.Property(d => d.Status)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(d => d.ApprovedBy)
            .HasMaxLength(200);

        builder.Property(d => d.ApprovedAt);

        builder.Property(d => d.RejectionReason)
            .HasMaxLength(500);

        builder.Property(d => d.Notes)
            .HasMaxLength(1000);

        builder.Property(d => d.TenantId)
            .IsRequired();

        builder.Property(d => d.CreatedAt)
            .IsRequired();

        builder.Property(d => d.UpdatedAt);

        // Relaciones
        builder.HasOne(d => d.Order)
            .WithMany()
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        // Índices
        builder.HasIndex(d => d.TenantId);
        builder.HasIndex(d => d.OrderId);
        builder.HasIndex(d => new { d.TenantId, d.Status });
    }
}
