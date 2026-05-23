using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class CommunityMembershipConfiguration : IEntityTypeConfiguration<CommunityMembership>
{
    public void Configure(EntityTypeBuilder<CommunityMembership> builder)
    {
        builder.ToTable("CommunityMemberships");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.EntityId)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(e => new { e.CommunityId, e.EntityType, e.EntityId })
            .IsUnique();
        builder.HasIndex(e => new { e.EntityType, e.EntityId });

        builder.HasOne(e => e.Community)
            .WithMany(e => e.Memberships)
            .HasForeignKey(e => e.CommunityId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
