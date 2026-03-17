using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class PayrollEntryConfiguration : IEntityTypeConfiguration<PayrollEntry>
{
    public void Configure(EntityTypeBuilder<PayrollEntry> builder)
    {
        builder.ToTable("PayrollEntries");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.PayrollPeriodId)
            .IsRequired();

        builder.Property(e => e.EmployeeId)
            .IsRequired();

        builder.Property(e => e.BaseSalary)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.WorkedDays)
            .IsRequired();

        builder.Property(e => e.OvertimeHours)
            .HasPrecision(10, 2);

        builder.Property(e => e.GrossPay)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.SocialSecurityEmployee)
            .HasPrecision(18, 2);

        builder.Property(e => e.SocialSecurityEmployer)
            .HasPrecision(18, 2);

        builder.Property(e => e.IncomeTax)
            .HasPrecision(18, 2);

        builder.Property(e => e.OtherDeductions)
            .HasPrecision(18, 2);

        builder.Property(e => e.OtherBenefits)
            .HasPrecision(18, 2);

        builder.Property(e => e.TotalDeductions)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.NetPay)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(e => e.TenantId)
            .IsRequired();

        builder.Property(e => e.CreatedAt)
            .IsRequired();

        builder.Property(e => e.UpdatedAt);

        // Relaciones
        builder.HasOne(e => e.PayrollPeriod)
            .WithMany(p => p.Entries)
            .HasForeignKey(e => e.PayrollPeriodId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Employee)
            .WithMany(emp => emp.PayrollEntries)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(e => e.PayrollPeriodId);
        builder.HasIndex(e => e.EmployeeId);
        builder.HasIndex(e => new { e.PayrollPeriodId, e.EmployeeId }).IsUnique();
    }
}
