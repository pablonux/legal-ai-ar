using LegalAiAr.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LegalAiAr.Infrastructure.Persistence.Configurations;

internal class CrawlerConfigConfiguration : IEntityTypeConfiguration<CrawlerConfig>
{
    public void Configure(EntityTypeBuilder<CrawlerConfig> builder)
    {
        builder.ToTable("CrawlerConfigs");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.CronExpression)
            .HasMaxLength(100);
        builder.Property(e => e.LastCrawledStatus)
            .HasMaxLength(20);

        builder.HasIndex(e => e.SourceId)
            .IsUnique();

        var now = new DateTime(2026, 3, 10, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            new CrawlerConfig { Id = 1, SourceId = 1, IsEnabled = true, CronExpression = null, LastCrawledAt = null, LastCrawledStatus = null, LastDocumentCount = null, CreatedAt = now, UpdatedAt = now },
            new CrawlerConfig { Id = 2, SourceId = 2, IsEnabled = false, CronExpression = null, LastCrawledAt = null, LastCrawledStatus = null, LastDocumentCount = null, CreatedAt = now, UpdatedAt = now },
            new CrawlerConfig { Id = 3, SourceId = 3, IsEnabled = false, CronExpression = null, LastCrawledAt = null, LastCrawledStatus = null, LastDocumentCount = null, CreatedAt = now, UpdatedAt = now },
            new CrawlerConfig { Id = 4, SourceId = 4, IsEnabled = false, CronExpression = null, LastCrawledAt = null, LastCrawledStatus = null, LastDocumentCount = null, CreatedAt = now, UpdatedAt = now },
            new CrawlerConfig { Id = 5, SourceId = 6, IsEnabled = true, CronExpression = null, LastCrawledAt = null, LastCrawledStatus = null, LastDocumentCount = null, CreatedAt = now, UpdatedAt = now });
    }
}
