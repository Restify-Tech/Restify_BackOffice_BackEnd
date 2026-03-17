using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Configurations;

public class GridConfigurationConfiguration : IEntityTypeConfiguration<GridConfiguration>
{
    public void Configure(EntityTypeBuilder<GridConfiguration> builder)
    {
        builder.ToTable("GridConfigurations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.DisplayName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.DisplayNamePlural)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.Description)
            .HasMaxLength(500);

        builder.Property(x => x.Icon)
            .HasMaxLength(50);

        builder.Property(x => x.ApiEndpoint)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.DefaultSortColumn)
            .HasMaxLength(100);

        builder.Property(x => x.FormMode)
            .HasMaxLength(20)
            .HasDefaultValue("modal");

        builder.Property(x => x.RowActionsPosition)
            .HasMaxLength(20)
            .HasDefaultValue("end");

        builder.Property(x => x.PageSizeOptions)
            .HasMaxLength(100);

        builder.Property(x => x.ExportFormats)
            .HasMaxLength(200);

        builder.Property(x => x.ExtraConfig)
            .HasColumnType("jsonb");

        // Índice único por TenantId + EntityName
        builder.HasIndex(x => new { x.TenantId, x.EntityName })
            .IsUnique();

        // Relación con columnas
        builder.HasMany(x => x.Columns)
            .WithOne(x => x.GridConfiguration)
            .HasForeignKey(x => x.GridConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
