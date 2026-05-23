using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class EmbeddingConfigConfiguration : IEntityTypeConfiguration<EmbeddingConfig>
{
    public void Configure(EntityTypeBuilder<EmbeddingConfig> builder)
    {
        builder.ToTable("EmbeddingConfigs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Version)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.EmbeddingModel)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.ContextualizationMethod)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.ChunkingStrategy)
            .HasMaxLength(100)
            .IsRequired();
        builder.Property(e => e.IsActive)
            .HasDefaultValue(false);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => e.Version)
            .IsUnique();
        builder.HasIndex(e => e.IsActive)
            .HasFilter("[IsActive] = 1");
    }
}
