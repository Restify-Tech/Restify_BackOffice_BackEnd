using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class CashClosingConfiguration : IEntityTypeConfiguration<CashClosing>
{
    public void Configure(EntityTypeBuilder<CashClosing> builder)
    {
        builder.ToTable("cash_closings", "backoffice");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CashRegisterSessionId)
            .IsRequired();

        builder.Property(c => c.ClosedBy)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.ClosedByName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(c => c.SupervisedBy)
            .HasMaxLength(256);

        builder.Property(c => c.SupervisedByName)
            .HasMaxLength(256);

        builder.Property(c => c.DenominationsJson)
            .HasColumnName("denominations_json")
            .HasColumnType("text");

        builder.Property(c => c.TotalCounted)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.TotalExpected)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.Difference)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(c => c.DifferenceReason)
            .HasMaxLength(1000);

        builder.Property(c => c.BankDepositAmount)
            .HasPrecision(18, 2);

        builder.Property(c => c.DepositVoucherUrl)
            .HasMaxLength(500);

        builder.Property(c => c.BankName)
            .HasMaxLength(200);

        builder.Property(c => c.BankReference)
            .HasMaxLength(200);

        builder.Property(c => c.ReportZUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Status)
            .IsRequired();

        builder.Property(c => c.ApprovedBy)
            .HasMaxLength(256);

        builder.Property(c => c.ApprovedAt);

        builder.Property(c => c.RejectionReason)
            .HasMaxLength(1000);

        builder.Property(c => c.ClosingDate)
            .IsRequired();

        builder.Property(c => c.TenantId)
            .IsRequired();

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        // Relaciones
        builder.HasOne(c => c.CashRegisterSession)
            .WithMany(s => s.Closings)
            .HasForeignKey(c => c.CashRegisterSessionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(c => c.TenantId);
        builder.HasIndex(c => c.CashRegisterSessionId);
        builder.HasIndex(c => new { c.TenantId, c.Status });
        builder.HasIndex(c => new { c.TenantId, c.ClosingDate });
    }
}
