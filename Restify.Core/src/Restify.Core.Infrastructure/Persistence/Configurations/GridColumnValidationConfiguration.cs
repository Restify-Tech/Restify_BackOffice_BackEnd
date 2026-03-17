using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Core.Domain.Entities;

namespace Restify.Core.Infrastructure.Persistence.Configurations;

public class GridColumnValidationConfiguration : IEntityTypeConfiguration<GridColumnValidation>
{
    public void Configure(EntityTypeBuilder<GridColumnValidation> builder)
    {
        builder.ToTable("GridColumnValidations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ValidationValue)
            .HasMaxLength(500);

        builder.Property(x => x.ValidationValue2)
            .HasMaxLength(500);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(500);

        // Índice para ordenación
        builder.HasIndex(x => new { x.GridColumnId, x.Order });
    }
}
