using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class RulingSynthesisConfiguration : IEntityTypeConfiguration<RulingSynthesis>
{
    public void Configure(EntityTypeBuilder<RulingSynthesis> builder)
    {
        builder.ToTable("RulingSyntheses");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Text)
            .IsRequired();
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasOne(e => e.Ruling)
            .WithMany(e => e.Syntheses)
            .HasForeignKey(e => e.RulingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
