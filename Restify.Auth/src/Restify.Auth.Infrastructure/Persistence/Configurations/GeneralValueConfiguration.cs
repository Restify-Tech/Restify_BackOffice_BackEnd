using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.Auth.Domain.Entities;

namespace Restify.Auth.Infrastructure.Persistence.Configurations;

public class GeneralValueConfiguration : IEntityTypeConfiguration<GeneralValue>
{
    public void Configure(EntityTypeBuilder<GeneralValue> builder)
    {
        builder.ToTable("general_values");
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Key).HasMaxLength(150).IsRequired();
        builder.Property(v => v.DisplayName).HasMaxLength(200).IsRequired();
        builder.Property(v => v.Value).HasMaxLength(2000);
        builder.Property(v => v.ValueType).HasConversion<string>().HasMaxLength(20);
        builder.Property(v => v.Category).HasMaxLength(100).IsRequired();
        builder.Property(v => v.Description).HasMaxLength(500);

        // Auto-referencia padre-hijo
        builder.HasOne(v => v.Parent)
            .WithMany(v => v.Children)
            .HasForeignKey(v => v.ParentId)
            .OnDelete(DeleteBehavior.SetNull);

        // Key unico global (no es multi-tenant: es configuracion del sistema)
        builder.HasIndex(v => v.Key).IsUnique();
        builder.HasIndex(v => v.Category);
        builder.HasIndex(v => v.ParentId);
    }
}
