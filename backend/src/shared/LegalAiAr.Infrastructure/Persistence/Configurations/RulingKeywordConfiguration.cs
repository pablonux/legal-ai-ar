using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingKeywordConfiguration : IEntityTypeConfiguration<RulingKeyword>
{
    public void Configure(EntityTypeBuilder<RulingKeyword> builder)
    {
        builder.ToTable("RulingKeywords");

        builder.HasKey(e => new { e.RulingId, e.KeywordId });

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.RulingKeywords)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Keyword)
            .WithMany(e => e.RulingKeywords)
            .HasForeignKey(e => e.KeywordId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
