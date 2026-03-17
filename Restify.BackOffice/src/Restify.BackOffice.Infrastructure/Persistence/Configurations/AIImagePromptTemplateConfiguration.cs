using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Restify.BackOffice.Domain.Entities;

namespace Restify.BackOffice.Infrastructure.Persistence.Configurations;

public class AIImagePromptTemplateConfiguration : IEntityTypeConfiguration<AIImagePromptTemplate>
{
    public void Configure(EntityTypeBuilder<AIImagePromptTemplate> builder)
    {
        builder.ToTable("AIImagePromptTemplates");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.PromptTemplate)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Style)
            .HasConversion<int>();

        builder.HasIndex(x => new { x.TenantId, x.Name })
            .IsUnique();

        builder.HasMany(x => x.Generations)
            .WithOne(x => x.PromptTemplate)
            .HasForeignKey(x => x.PromptTemplateId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
