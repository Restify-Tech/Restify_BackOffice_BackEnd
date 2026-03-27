using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class ShiftAssignmentConfiguration : IEntityTypeConfiguration<ShiftAssignment>
{
    public void Configure(EntityTypeBuilder<ShiftAssignment> builder)
    {
        builder.ToTable("shift_assignments", "backoffice");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EmployeeId)
            .IsRequired();

        builder.Property(s => s.ShiftTemplateId);

        builder.Property(s => s.Date)
            .IsRequired();

        builder.Property(s => s.ScheduledStart);

        builder.Property(s => s.ScheduledEnd);

        builder.Property(s => s.ActualClockIn);

        builder.Property(s => s.ActualClockOut);

        builder.Property(s => s.ClockInMethod)
            .IsRequired();

        builder.Property(s => s.ClockInLocation)
            .HasMaxLength(100);

        builder.Property(s => s.Status)
            .IsRequired();

        builder.Property(s => s.Notes)
            .HasMaxLength(1000);

        builder.Property(s => s.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(s => s.HoursWorked)
            .HasPrecision(8, 2);

        builder.Property(s => s.TenantId)
            .IsRequired();

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt);

        // Relaciones
        builder.HasOne(s => s.Employee)
            .WithMany()
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.ShiftTemplate)
            .WithMany(t => t.Assignments)
            .HasForeignKey(s => s.ShiftTemplateId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.SetNull);

        // Indices
        builder.HasIndex(s => s.TenantId);
        builder.HasIndex(s => s.EmployeeId);
        builder.HasIndex(s => new { s.TenantId, s.Date });
        builder.HasIndex(s => new { s.TenantId, s.EmployeeId, s.Date });
        builder.HasIndex(s => new { s.TenantId, s.Status });
    }
}
