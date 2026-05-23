using System.ComponentModel.DataAnnotations;

namespace LegalAiAr.Infrastructure.Crawling.Options;

/// <summary>
/// Configuration for CSJN sjconsulta JSON client. Bound from CsjnApi section.
/// </summary>
public class CsjnApiOptions
{
    public const string SectionName = "CsjnApi";

    /// <summary>
    /// Base URL of the CSJN sjconsulta API. Default: https://sjconsulta.csjn.gov.ar/sjconsulta/
    /// </summary>
    public string BaseUrl { get; set; } = "https://sjconsulta.csjn.gov.ar/sjconsulta/";

    /// <summary>
    /// Minimum delay in milliseconds between consecutive API requests. Default: 500.
    /// </summary>
    [Range(0, 60_000, ErrorMessage = "ThrottlingDelayMs must be between 0 and 60000 ms.")]
    public int ThrottlingDelayMs { get; set; } = 500;

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
    /// HTTP request timeout in seconds. Default: 30.
    /// </summary>
    [Range(5, 120, ErrorMessage = "RequestTimeoutSeconds must be between 5 and 120.")]
    public int RequestTimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// When true (default), primed API JSON under <c>_cache/.../api/</c> is read before HTTP even if the
    /// pipeline message has <c>UseCache=false</c>. Write-through after HTTP is unchanged. Set false to
    /// restore legacy behavior (blob read only when <c>UseCache</c> is true).
    /// </summary>
    public bool PreferBlobApiCacheBeforeHttp { get; set; } = true;

    /// <summary>
    /// When true (default), the seven CSJN GETs after <c>abrirAnalisis</c> are issued concurrently, bounded by
    /// <see cref="PostAbrirHttpMaxConcurrencyGlobal"/> and <see cref="ThrottlingDelayMs"/> spacing (see <see cref="CsjnApiRequestGate"/>).
    /// When false, those calls run strictly one after another (legacy behavior).
    /// </summary>
    public bool PostAbrirParallelEnabled { get; set; } = true;

    /// <summary>
    /// Maximum concurrent HTTP requests to CSJN from this worker process (bulkhead). Default 6.
    /// </summary>
    [Range(1, 32, ErrorMessage = "PostAbrirHttpMaxConcurrencyGlobal must be between 1 and 32.")]
    public int PostAbrirHttpMaxConcurrencyGlobal { get; set; } = 6;

    /// <summary>
    /// When false, JSON is only read from blob cache after a miss when outbound HTTP would have been used.
    /// The Fetcher hydrates cache first; the Parser should set this to false so sjconsulta is only called from Fetcher.
    /// Default true preserves legacy behavior (Parser may still call HTTP).
    /// </summary>
    public bool OutboundHttpEnabled { get; set; } = true;
}
