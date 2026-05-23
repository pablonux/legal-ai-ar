using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class ThesaurusTermConfiguration : IEntityTypeConfiguration<ThesaurusTerm>
{
    public void Configure(EntityTypeBuilder<ThesaurusTerm> builder)
    {
        builder.ToTable("ThesaurusTerms");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Label)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.BranchName)
            .HasMaxLength(200);

        builder.HasIndex(e => e.ExternalId)
            .IsUnique();

        builder.HasIndex(e => e.Label);
        builder.HasIndex(e => e.IsPreferred);
    }
}
