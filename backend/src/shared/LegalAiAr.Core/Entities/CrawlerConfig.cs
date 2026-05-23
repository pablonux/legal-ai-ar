namespace LegalAiAr.Core.Entities;

/// <summary>
/// Per-source crawler configuration. CronExpression is Fase 2.
/// </summary>
public class CrawlerConfig
{
    public int Id { get; set; }
    public int SourceId { get; set; }
    public bool IsEnabled { get; set; }
    public string? CronExpression { get; set; }
    public DateTime? LastCrawledAt { get; set; }
    public string? LastCrawledStatus { get; set; } // "success" | "partial" | "failed"
    public int? LastDocumentCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Source Source { get; set; } = null!;
}
