using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class KeywordConfiguration : IEntityTypeConfiguration<Keyword>
{
    public void Configure(EntityTypeBuilder<Keyword> builder)
    {
        builder.ToTable("Keywords");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Description)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne(e => e.ThesaurusTerm)
            .WithMany()
            .HasForeignKey(e => e.ThesaurusTermId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.ThesaurusTermId);

        builder.HasMany(e => e.RulingKeywords)
            .WithOne(e => e.Keyword)
            .HasForeignKey(e => e.KeywordId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
