using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling.Sources;

/// <summary>
/// Crawls SAIJ jurisprudencia API for rulings from Cámaras Federales, Nacionales and other courts.
/// SourceId = 3. Content is JSON (same pattern as SaijLegislationCrawlerSource).
/// </summary>
public class SaijRulingCrawlerSource : ICrawlerSource
{
    public const int SourceId = 3;

    private readonly IOptions<SaijCrawlerOptions> _options;
    private readonly ILogger<SaijRulingCrawlerSource> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public int? LastTotalSearchResults { get; private set; }

    public SaijRulingCrawlerSource(
        IOptions<SaijCrawlerOptions> options,
        ILogger<SaijRulingCrawlerSource> logger,
        IHttpClientFactory httpClientFactory)
    {
        _options = options;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
    }

    public Task<int?> GetTotalForRangeAsync(CrawlerMessage message, CancellationToken cancellationToken = default)
        => Task.FromResult<int?>(null);

    public async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        CrawlerMessage message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(nameof(SaijRulingCrawlerSource));
        var pageSize = _options.Value.PageSize;
        var delay = _options.Value.ThrottlingDelayMs;
        var tribunal = message.DocumentType != "ruling" ? message.DocumentType : null;

        var offset = 0;
        var totalDiscovered = 0;
        var maxDocs = message.MaxDocuments ?? int.MaxValue;

        _logger.LogInformation("SAIJ Rulings: starting discovery (tribunal={Tribunal}, since={Since})",
            tribunal ?? "all", message.Since);

        while (totalDiscovered < maxDocs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var url = BuildSearchUrl(tribunal, offset, pageSize, message.Since);
            _logger.LogDebug("SAIJ Rulings: fetching {Url}", url);

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<SaijSearchResult>(json, JsonOptions);

            if (result?.Results is null || result.Results.Count == 0)
            {
                _logger.LogInformation("SAIJ Rulings: no more results at offset {Offset}", offset);
                break;
            }

            if (offset == 0)
            {
                LastTotalSearchResults = result.Total;
                _logger.LogInformation("SAIJ Rulings: {Total} total documents reported", result.Total);
            }

            var batch = new List<DiscoveredDocument>();
            foreach (var doc in result.Results)
            {
                if (totalDiscovered >= maxDocs) break;
                if (string.IsNullOrEmpty(doc.Id)) continue;

                batch.Add(new DiscoveredDocument(
                    DocumentId: doc.Id,
                    AnalysisId: null,
                    AcuerdoDate: null,
                    CaseNumber: doc.Caratula));
                totalDiscovered++;
            }

            if (batch.Count > 0)
                yield return batch;

            offset += pageSize;

            if (result.Results.Count < pageSize)
                break;

            if (delay > 0)
                await Task.Delay(delay, cancellationToken);
        }

        _logger.LogInformation("SAIJ Rulings: discovery complete, {Count} documents found", totalDiscovered);
    }

    public async Task<CrawlerDocumentContent> GetContentHashAsync(
        string documentId,
        string? analysisId,
        bool useCache = false,
        int? httpRequestTimeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        HttpClient? owned = null;
        HttpClient client;
        if (httpRequestTimeoutSeconds is > 0)
        {
            var secs = Math.Clamp(httpRequestTimeoutSeconds.Value, 60, 900);
            var opts = _options.Value;
            owned = new HttpClient
            {
                BaseAddress = new Uri(opts.BaseUrl.TrimEnd('/') + "/"),
                Timeout = TimeSpan.FromSeconds(secs),
            };
            owned.DefaultRequestHeaders.TryAddWithoutValidation(
                "User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
            client = owned;
        }
        else
        {
            client = _httpClientFactory.CreateClient(nameof(SaijRulingCrawlerSource));
        }

        try
        {
            var url = $"documentos/{documentId}";

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var jsonBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
            var hash = Convert.ToHexStringLower(SHA256.HashData(jsonBytes));

            return new CrawlerDocumentContent(hash, jsonBytes);
        }
        finally
        {
            owned?.Dispose();
        }
    }

    private static string BuildSearchUrl(string? tribunal, int offset, int limit, DateOnly? since)
    {
        var sb = new StringBuilder("documentos?tipo=jurisprudencia");
        sb.Append($"&limit={limit}&offset={offset}");
        sb.Append("&orden=fecha-resolucion:desc");

        if (!string.IsNullOrEmpty(tribunal))
            sb.Append($"&tribunal={Uri.EscapeDataString(tribunal)}");

        if (since.HasValue)
            sb.Append($"&fecha-desde={since.Value:yyyy-MM-dd}");

        return sb.ToString();
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    private sealed class SaijSearchResult
    {
        public int Total { get; set; }
        public List<SaijDocRef> Results { get; set; } = [];
    }

    private sealed class SaijDocRef
    {
        public string Id { get; set; } = "";
        public string? Caratula { get; set; }
        public string? Tribunal { get; set; }
        public string? FechaResolucion { get; set; }
    }
}
