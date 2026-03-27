using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class ShiftTemplateConfiguration : IEntityTypeConfiguration<ShiftTemplate>
{
    public void Configure(EntityTypeBuilder<ShiftTemplate> builder)
    {
        builder.ToTable("shift_templates", "backoffice");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.StartTime)
            .IsRequired();

        builder.Property(s => s.EndTime)
            .IsRequired();

        builder.Property(s => s.DaysOfWeek)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(s => s.BranchId);

        builder.Property(s => s.IsActive)
            .IsRequired();

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Indices
        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => new { s.TenantId, s.IsActive });
        builder.HasIndex(s => new { s.TenantId, s.BranchId });
    }
}
