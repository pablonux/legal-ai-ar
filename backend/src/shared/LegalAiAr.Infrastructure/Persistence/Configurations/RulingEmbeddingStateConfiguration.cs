using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingEmbeddingStateConfiguration : IEntityTypeConfiguration<RulingEmbeddingState>
{
    public void Configure(EntityTypeBuilder<RulingEmbeddingState> builder)
    {
        builder.ToTable("RulingEmbeddingStates");

        builder.HasKey(e => e.RulingId);

        builder.Property(e => e.NeedsReembedding)
            .HasDefaultValue(false);

        builder.HasIndex(e => e.NeedsReembedding)
            .HasFilter("[NeedsReembedding] = 1");

        builder.HasOne(e => e.Ruling)
            .WithOne()
            .HasForeignKey<RulingEmbeddingState>(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.EmbeddingConfig)
            .WithMany(e => e.RulingStates)
            .HasForeignKey(e => e.EmbeddingConfigId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
