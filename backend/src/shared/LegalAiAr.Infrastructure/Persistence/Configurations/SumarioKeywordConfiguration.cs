using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class SumarioKeywordConfiguration : IEntityTypeConfiguration<SumarioKeyword>
{
    public void Configure(EntityTypeBuilder<SumarioKeyword> builder)
    {
        builder.ToTable("SumarioKeywords");

        builder.HasKey(e => new { e.SumarioId, e.KeywordId });

        builder.HasOne(e => e.Sumario)
            .WithMany(e => e.SumarioKeywords)
            .HasForeignKey(e => e.SumarioId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(e => e.Keyword)
            .WithMany(e => e.SumarioKeywords)
            .HasForeignKey(e => e.KeywordId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
