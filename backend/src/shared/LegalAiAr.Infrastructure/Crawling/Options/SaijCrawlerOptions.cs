using System.ComponentModel.DataAnnotations;

namespace LegalAiAr.Infrastructure.Crawling.Options;

public class SaijCrawlerOptions
{
    public const string SectionName = "SaijCrawler";

    public string BaseUrl { get; set; } = "https://api.saij.gob.ar/api/";

    [Range(0, 60_000)]
    public int ThrottlingDelayMs { get; set; } = 1500;

    [Range(1, 100)]
    public int PageSize { get; set; } = 25;
}
