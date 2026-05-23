namespace LegalAiAr.Api.Models;

/// <summary>
/// Request body for PATCH /api/admin/crawlers/{sourceId}.
/// </summary>
/// <param name="IsEnabled">Enable or disable the source for crawling.</param>
public record UpdateCrawlerConfigRequest(bool IsEnabled);
