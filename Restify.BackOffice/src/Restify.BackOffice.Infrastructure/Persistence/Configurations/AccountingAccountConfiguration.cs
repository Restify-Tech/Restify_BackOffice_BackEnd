using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class AccountingAccountConfiguration : IEntityTypeConfiguration<AccountingAccount>
{
    public void Configure(EntityTypeBuilder<AccountingAccount> builder)
    {
        builder.ToTable("AccountingAccounts");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        builder.Property(a => a.AccountType)
            .IsRequired();

        builder.Property(a => a.Level)
            .IsRequired();

        builder.Property(a => a.AcceptsEntries)
            .IsRequired();

        builder.Property(a => a.IsActive)
            .IsRequired();

        builder.Property(a => a.TenantId)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt);

        // Relacion auto-referencial
        builder.HasOne(a => a.Parent)
            .WithMany(a => a.Children)
            .HasForeignKey(a => a.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(a => a.TenantId);
        builder.HasIndex(a => new { a.TenantId, a.Code }).IsUnique();
    }
}
