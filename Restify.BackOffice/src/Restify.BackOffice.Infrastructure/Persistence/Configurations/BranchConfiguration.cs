using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches", "backoffice");

        builder.HasKey(b => b.Id);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.Address)
            .HasMaxLength(500);

        builder.Property(b => b.City)
            .HasMaxLength(100);

        builder.Property(b => b.Phone)
            .HasMaxLength(20);

        builder.Property(b => b.Email)
            .HasMaxLength(200);

        builder.Property(b => b.Timezone)
            .HasMaxLength(100);

        builder.Property(b => b.IsActive)
            .IsRequired();

        builder.Property(b => b.OpeningHours)
            .HasColumnType("jsonb");

        builder.Property(b => b.Notes)
            .HasMaxLength(1000);

        builder.Property(b => b.TenantId)
            .IsRequired();

        builder.Property(b => b.CreatedAt)
            .IsRequired();

        builder.Property(b => b.UpdatedAt);

        // Relaciones
        builder.HasMany(b => b.Tables)
            .WithOne(t => t.Branch)
            .HasForeignKey(t => t.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.CashRegisters)
            .WithOne(cr => cr.Branch)
            .HasForeignKey(cr => cr.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.Employees)
            .WithOne(e => e.Branch)
            .HasForeignKey(e => e.BranchId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(b => b.ManagerAssignments)
            .WithOne(ma => ma.Branch)
            .HasForeignKey(ma => ma.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indices
        builder.HasIndex(b => b.TenantId);
        builder.HasIndex(b => new { b.TenantId, b.Name });
        builder.HasIndex(b => new { b.TenantId, b.IsActive });
    }
}
