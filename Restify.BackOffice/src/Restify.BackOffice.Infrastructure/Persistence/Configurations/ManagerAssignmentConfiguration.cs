using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class ManagerAssignmentConfiguration : IEntityTypeConfiguration<ManagerAssignment>
{
    public void Configure(EntityTypeBuilder<ManagerAssignment> builder)
    {
        builder.ToTable("manager_assignments", "backoffice");

        builder.HasKey(ma => ma.Id);

        builder.Property(ma => ma.BranchId)
            .IsRequired();

        builder.Property(ma => ma.UserId)
            .IsRequired();

        builder.Property(ma => ma.UserName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ma => ma.UserEmail)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(ma => ma.CanApproveCashClosing)
            .IsRequired();

        builder.Property(ma => ma.CanVoidOrders)
            .IsRequired();

        builder.Property(ma => ma.MaxDiscountPercent)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(ma => ma.IsActive)
            .IsRequired();

        builder.Property(ma => ma.ValidFrom);
        builder.Property(ma => ma.ValidTo);

        builder.Property(ma => ma.TenantId)
            .IsRequired();

        builder.Property(ma => ma.CreatedAt)
            .IsRequired();

        builder.Property(ma => ma.UpdatedAt);

        // Relacion con Branch
        builder.HasOne(ma => ma.Branch)
            .WithMany(b => b.ManagerAssignments)
            .HasForeignKey(ma => ma.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(ma => ma.TenantId);
        builder.HasIndex(ma => ma.BranchId);
        builder.HasIndex(ma => ma.UserId);
        builder.HasIndex(ma => new { ma.BranchId, ma.UserId });
        builder.HasIndex(ma => new { ma.TenantId, ma.IsActive });
    }
}
