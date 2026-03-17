using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Configurations;

public class GeneralTableConfiguration : IEntityTypeConfiguration<GeneralTable>
{
    public void Configure(EntityTypeBuilder<GeneralTable> builder)
    {
        builder.ToTable("GeneralTables");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.ApplicationCode)
            .HasMaxLength(50);

        builder.Property(x => x.Icon)
            .HasMaxLength(100);

        builder.Property(x => x.ExtraConfig)
            .HasColumnType("jsonb");

        // Índice único por TenantId + Code
        builder.HasIndex(x => new { x.TenantId, x.Code })
            .IsUnique();

        // Relación recursiva (padre-hijo)
        builder.HasOne(x => x.Parent)
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relación con valores
        builder.HasMany(x => x.Values)
            .WithOne(x => x.GeneralTable)
            .HasForeignKey(x => x.GeneralTableId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
