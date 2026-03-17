using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class PayrollPeriodConfiguration : IEntityTypeConfiguration<PayrollPeriod>
{
    public void Configure(EntityTypeBuilder<PayrollPeriod> builder)
    {
        builder.ToTable("PayrollPeriods");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Year)
            .IsRequired();

        builder.Property(p => p.Month)
            .IsRequired();

        builder.Property(p => p.PeriodType)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.Property(p => p.EndDate)
            .IsRequired();

        builder.Property(p => p.PaymentDate)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.TotalGross)
            .HasPrecision(18, 2);

        builder.Property(p => p.TotalDeductions)
            .HasPrecision(18, 2);

        builder.Property(p => p.TotalNet)
            .HasPrecision(18, 2);

        builder.Property(p => p.ApprovedBy)
            .HasMaxLength(200);

        builder.Property(p => p.ApprovedAt);

        builder.Property(p => p.PaidBy)
            .HasMaxLength(200);

        builder.Property(p => p.PaidAt);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        // Indices
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => new { p.TenantId, p.Year, p.Month, p.PeriodType }).IsUnique();
    }
}
