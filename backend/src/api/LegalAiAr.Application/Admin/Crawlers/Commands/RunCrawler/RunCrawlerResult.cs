namespace LegalAiAr.Application.Admin.Crawlers.Commands.RunCrawler;

/// <summary>
/// Result of triggering a manual crawl.
/// </summary>
/// <param name="Success">Whether the crawl was triggered successfully.</param>
/// <param name="Message">Human-readable message for the response.</param>
public record RunCrawlerResult(bool Success, string Message);
