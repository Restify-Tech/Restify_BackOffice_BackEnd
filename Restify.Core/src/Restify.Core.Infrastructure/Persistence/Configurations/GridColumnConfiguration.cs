using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Configurations;

public class GridColumnConfiguration : IEntityTypeConfiguration<GridColumn>
{
    public void Configure(EntityTypeBuilder<GridColumn> builder)
    {
        builder.ToTable("GridColumns");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FieldName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.HeaderText)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.FormLabel)
            .HasMaxLength(200);

        builder.Property(x => x.HeaderTooltip)
            .HasMaxLength(500);

        builder.Property(x => x.FormGroup)
            .HasMaxLength(100);

        builder.Property(x => x.Width)
            .HasMaxLength(50);

        builder.Property(x => x.MinWidth)
            .HasMaxLength(50);

        builder.Property(x => x.MaxWidth)
            .HasMaxLength(50);

        builder.Property(x => x.FrozenPosition)
            .HasMaxLength(20);

        builder.Property(x => x.DisplayFormat)
            .HasMaxLength(100);

        builder.Property(x => x.DefaultValue)
            .HasMaxLength(500);

        builder.Property(x => x.Placeholder)
            .HasMaxLength(200);

        builder.Property(x => x.HelpText)
            .HasMaxLength(500);

        builder.Property(x => x.Prefix)
            .HasMaxLength(20);

        builder.Property(x => x.Suffix)
            .HasMaxLength(20);

        builder.Property(x => x.CssClass)
            .HasMaxLength(200);

        builder.Property(x => x.CellTemplate)
            .HasMaxLength(2000);

        builder.Property(x => x.EditorTemplate)
            .HasMaxLength(2000);

        builder.Property(x => x.SelectOptions)
            .HasColumnType("jsonb");

        builder.Property(x => x.VisibilityCondition)
            .HasColumnType("jsonb");

        builder.Property(x => x.EnabledCondition)
            .HasColumnType("jsonb");

        builder.Property(x => x.ExtraConfig)
            .HasColumnType("jsonb");

        // Índice para ordenación
        builder.HasIndex(x => new { x.GridConfigurationId, x.GridOrder });

        // Relación con validaciones
        builder.HasMany(x => x.Validations)
            .WithOne(x => x.GridColumn)
            .HasForeignKey(x => x.GridColumnId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relación con lookup
        builder.HasOne(x => x.Lookup)
            .WithOne(x => x.GridColumn)
            .HasForeignKey<GridColumnLookup>(x => x.GridColumnId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
