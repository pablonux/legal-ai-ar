using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingLinkConfiguration : IEntityTypeConfiguration<RulingLink>
{
    public void Configure(EntityTypeBuilder<RulingLink> builder)
    {
        builder.ToTable("RulingLinks");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Url)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(e => e.Title)
            .HasMaxLength(500);
        builder.Property(e => e.LinkType)
            .HasMaxLength(50);
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.Links)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
