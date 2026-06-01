using System.ComponentModel.DataAnnotations;

namespace LegalAiAr.Infrastructure.Crawling.Options;

/// <summary>
/// Configuration for CSJN crawler. Bound from CsjnCrawler section (e.g. CsjnCrawler__ThrottlingDelayMs).
/// </summary>
public class CsjnCrawlerOptions
{
    public const string SectionName = "CsjnCrawler";

    /// <summary>
    /// Base URL of the CSJN sjconsulta portal. Default: https://sjconsulta.csjn.gov.ar/sjconsulta/
    /// </summary>
    public string BaseUrl { get; set; } = "https://sjconsulta.csjn.gov.ar/sjconsulta/";

    /// <summary>
    /// Minimum delay in milliseconds between consecutive requests (discovery, API, PDF). Default: 2000.
    /// </summary>
    [Range(0, 60_000, ErrorMessage = "ThrottlingDelayMs must be between 0 and 60000 ms.")]
    public int ThrottlingDelayMs { get; set; } = 2000;

    /// <summary>
    /// Multiplier for exponential backoff on 429/rate-limit. Default: 2.0.
    /// </summary>
    [Range(1.0, 10.0, ErrorMessage = "ThrottlingBackoffMultiplier must be between 1.0 and 10.0.")]
    public double ThrottlingBackoffMultiplier { get; set; } = 2.0;

    /// <summary>
    /// Maximum retries per request when rate limited. Default: 3.
    /// </summary>
    [Range(0, 10, ErrorMessage = "ThrottlingMaxRetries must be between 0 and 10.")]
    public int ThrottlingMaxRetries { get; set; } = 3;

    /// <summary>
    /// Maximum acuerdo dates processed in parallel during discovery (each date keeps its own session and sequential pagination). Default 1 (sequential dates).
    /// </summary>
    [Range(1, 8, ErrorMessage = "DiscoveryDateConcurrency must be between 1 and 8.")]
    public int DiscoveryDateConcurrency { get; set; } = 1;

    /// <summary>
    /// Maximum concurrent CSJN discovery HTTP requests across all in-flight dates (0 = no global bulkhead).
    /// </summary>
    [Range(0, 32, ErrorMessage = "DiscoveryHttpMaxConcurrency must be between 0 and 32.")]
    public int DiscoveryHttpMaxConcurrency { get; set; } = 0;
}
