using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class IngestionJobDetailConfiguration : IEntityTypeConfiguration<IngestionJobDetail>
{
    public void Configure(EntityTypeBuilder<IngestionJobDetail> builder)
    {
        builder.ToTable("IngestionJobDetails");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EntityType)
            .HasMaxLength(50)
            .IsRequired();
        builder.Property(e => e.Timestamp)
            .HasDefaultValueSql("GETUTCDATE()");

        builder.HasIndex(e => new { e.IngestionJobId, e.EntityType })
            .IsUnique();

        builder.HasOne(e => e.IngestionJob)
            .WithMany()
            .HasForeignKey(e => e.IngestionJobId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
