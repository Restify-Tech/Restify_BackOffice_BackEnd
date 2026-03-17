using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class JournalEntryLineConfiguration : IEntityTypeConfiguration<JournalEntryLine>
{
    public void Configure(EntityTypeBuilder<JournalEntryLine> builder)
    {
        builder.ToTable("JournalEntryLines");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.JournalEntryId)
            .IsRequired();

        builder.Property(l => l.AccountId)
            .IsRequired();

        builder.Property(l => l.Description)
            .HasMaxLength(500);

        builder.Property(l => l.Debit)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.Credit)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(l => l.TenantId)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        builder.Property(l => l.UpdatedAt);

        // Relaciones
        builder.HasOne(l => l.JournalEntry)
            .WithMany(j => j.Lines)
            .HasForeignKey(l => l.JournalEntryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(l => l.Account)
            .WithMany(a => a.JournalEntryLines)
            .HasForeignKey(l => l.AccountId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(l => l.JournalEntryId);
        builder.HasIndex(l => l.AccountId);
    }
}
