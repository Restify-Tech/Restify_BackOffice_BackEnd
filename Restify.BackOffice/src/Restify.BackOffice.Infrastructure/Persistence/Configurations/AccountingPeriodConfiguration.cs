using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class AccountingPeriodConfiguration : IEntityTypeConfiguration<AccountingPeriod>
{
    public void Configure(EntityTypeBuilder<AccountingPeriod> builder)
    {
        builder.ToTable("AccountingPeriods");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Year)
            .IsRequired();

        builder.Property(p => p.Month)
            .IsRequired();

        builder.Property(p => p.StartDate)
            .IsRequired();

        builder.Property(p => p.EndDate)
            .IsRequired();

        builder.Property(p => p.Status)
            .IsRequired();

        builder.Property(p => p.ClosedBy)
            .HasMaxLength(200);

        builder.Property(p => p.ClosedAt);

        builder.Property(p => p.TenantId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt);

        // Indices
        builder.HasIndex(p => p.TenantId);
        builder.HasIndex(p => new { p.TenantId, p.Year, p.Month }).IsUnique();
    }
}
