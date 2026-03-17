using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class AIImageGenerationConfiguration : IEntityTypeConfiguration<AIImageGeneration>
{
    public void Configure(EntityTypeBuilder<AIImageGeneration> builder)
    {
        builder.ToTable("AIImageGenerations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FinalPrompt)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.GeneratedImageUrl)
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .HasConversion<int>();

        builder.Property(x => x.ProviderUsed)
            .HasMaxLength(50);

        builder.Property(x => x.ErrorMessage)
            .HasMaxLength(1000);

        builder.Property(x => x.CostUsd)
            .HasPrecision(10, 4);

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.TenantId);
        builder.HasIndex(x => x.ProductId);
        builder.HasIndex(x => new { x.TenantId, x.Status });
    }
}
