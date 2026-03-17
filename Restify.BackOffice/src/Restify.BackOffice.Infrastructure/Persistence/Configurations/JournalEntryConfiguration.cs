using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class JournalEntryConfiguration : IEntityTypeConfiguration<JournalEntry>
{
    public void Configure(EntityTypeBuilder<JournalEntry> builder)
    {
        builder.ToTable("JournalEntries");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.EntryNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(j => j.PeriodId)
            .IsRequired();

        builder.Property(j => j.Date)
            .IsRequired();

        builder.Property(j => j.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(j => j.Reference)
            .HasMaxLength(200);

        builder.Property(j => j.EntryType)
            .IsRequired();

        builder.Property(j => j.SourceId);

        builder.Property(j => j.Status)
            .IsRequired();

        builder.Property(j => j.PostedBy)
            .HasMaxLength(200);

        builder.Property(j => j.PostedAt);

        builder.Property(j => j.ReversedBy)
            .HasMaxLength(200);

        builder.Property(j => j.ReversedAt);

        builder.Property(j => j.TenantId)
            .IsRequired();

        builder.Property(j => j.CreatedAt)
            .IsRequired();

        builder.Property(j => j.UpdatedAt);

        // Relaciones
        builder.HasOne(j => j.Period)
            .WithMany(p => p.JournalEntries)
            .HasForeignKey(j => j.PeriodId)
            .OnDelete(DeleteBehavior.Restrict);

        // Indices
        builder.HasIndex(j => j.TenantId);
        builder.HasIndex(j => new { j.TenantId, j.EntryNumber }).IsUnique();
        builder.HasIndex(j => j.PeriodId);
        builder.HasIndex(j => new { j.TenantId, j.Status });
    }
}
