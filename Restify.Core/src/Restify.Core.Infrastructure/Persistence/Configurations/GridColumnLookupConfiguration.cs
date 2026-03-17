using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Configurations;

public class GridColumnLookupConfiguration : IEntityTypeConfiguration<GridColumnLookup>
{
    public void Configure(EntityTypeBuilder<GridColumnLookup> builder)
    {
        builder.ToTable("GridColumnLookups");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.TargetEntity)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.ApiEndpoint)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.ValueField)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("id");

        builder.Property(x => x.DisplayField)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("name");

        builder.Property(x => x.AdditionalDisplayFields)
            .HasMaxLength(500);

        builder.Property(x => x.DisplayFormat)
            .HasMaxLength(200);

        builder.Property(x => x.SearchField)
            .HasMaxLength(100);

        builder.Property(x => x.StaticFilter)
            .HasColumnType("jsonb");

        builder.Property(x => x.OrderBy)
            .HasMaxLength(100);

        builder.Property(x => x.ParentField)
            .HasMaxLength(100);

        builder.Property(x => x.ParentFilterField)
            .HasMaxLength(100);

        // Índice único por columna
        builder.HasIndex(x => x.GridColumnId)
            .IsUnique();
    }
}
