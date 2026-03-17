using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Configurations;

public class GeneralValueConfiguration : IEntityTypeConfiguration<GeneralValue>
{
    public void Configure(EntityTypeBuilder<GeneralValue> builder)
    {
        builder.ToTable("GeneralValues");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ShortDescription)
            .HasMaxLength(200);

        builder.Property(x => x.NumericValue)
            .HasPrecision(18, 4);

        builder.Property(x => x.Reference1)
            .HasMaxLength(500);

        builder.Property(x => x.Reference2)
            .HasMaxLength(500);

        builder.Property(x => x.Reference3)
            .HasMaxLength(500);

        builder.Property(x => x.Reference4)
            .HasMaxLength(500);

        builder.Property(x => x.Reference5)
            .HasMaxLength(500);

        builder.Property(x => x.Icon)
            .HasMaxLength(100);

        builder.Property(x => x.BackgroundColor)
            .HasMaxLength(20);

        builder.Property(x => x.TextColor)
            .HasMaxLength(20);

        builder.Property(x => x.ExtraConfig)
            .HasColumnType("jsonb");

        // Índice único por TenantId + GeneralTableId + Code
        builder.HasIndex(x => new { x.TenantId, x.GeneralTableId, x.Code })
            .IsUnique();

        // Índice para búsquedas por tabla
        builder.HasIndex(x => x.GeneralTableId);
    }
}
