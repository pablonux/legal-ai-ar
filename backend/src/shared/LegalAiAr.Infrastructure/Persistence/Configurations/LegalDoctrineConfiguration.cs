using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class LegalDoctrineConfiguration : IEntityTypeConfiguration<LegalDoctrine>
{
    public void Configure(EntityTypeBuilder<LegalDoctrine> builder)
    {
        builder.ToTable("LegalDoctrines");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Statement).IsRequired();
        builder.Property(e => e.Topic).HasMaxLength(200);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Ruling)
            .WithMany(r => r.LegalDoctrines)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.OverruledByRuling)
            .WithMany()
            .HasForeignKey(e => e.OverruledByRulingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.RulingId);
        builder.HasIndex(e => e.OverruledByRulingId).HasFilter("[OverruledByRulingId] IS NOT NULL");
    }
}
