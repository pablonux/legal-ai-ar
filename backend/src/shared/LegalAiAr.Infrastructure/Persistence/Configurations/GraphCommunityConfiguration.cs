using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class GraphCommunityConfiguration : IEntityTypeConfiguration<GraphCommunity>
{
    public void Configure(EntityTypeBuilder<GraphCommunity> builder)
    {
        builder.ToTable("GraphCommunities");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(500)
            .IsRequired();
        builder.Property(e => e.Summary)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => e.Level);

        builder.HasOne(e => e.ParentCommunity)
            .WithMany(e => e.ChildCommunities)
            .HasForeignKey(e => e.ParentCommunityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(e => e.EmbeddingConfig)
            .WithMany(e => e.Communities)
            .HasForeignKey(e => e.EmbeddingConfigId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
