using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingStatuteArticleConfiguration : IEntityTypeConfiguration<RulingStatuteArticle>
{
    public void Configure(EntityTypeBuilder<RulingStatuteArticle> builder)
    {
        builder.ToTable("RulingStatuteArticles");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Article)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Subsection)
            .HasMaxLength(100);

        builder.HasIndex(e => new { e.RulingStatuteId, e.Article });

        builder.HasOne(e => e.RulingStatute)
            .WithMany(e => e.StructuredArticles)
            .HasForeignKey(e => e.RulingStatuteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
