using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ChunkEntityMentionConfiguration : IEntityTypeConfiguration<ChunkEntityMention>
{
    public void Configure(EntityTypeBuilder<ChunkEntityMention> builder)
    {
        builder.ToTable("ChunkEntityMentions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();
        // Statute mentions may use display names when Number is absent (see ExtractChunkMentionsStep).
        // Keep length within SQL Server nonclustered index key limits (~1700 bytes) with EntityType (50).
        builder.Property(e => e.EntityId)
            .HasMaxLength(450)
            .IsRequired();
        builder.Property(e => e.MentionType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.HasIndex(e => new { e.RulingId, e.ChunkIndex });
        builder.HasIndex(e => new { e.EntityType, e.EntityId });

        builder.HasOne(e => e.Ruling)
            .WithMany()
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
