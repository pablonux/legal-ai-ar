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
/// Crawls SAIJ legislation API (http://api.saij.gob.ar) for national laws, decrees, resolutions, etc.
/// Paginates via offset/limit, yields batches of DiscoveredDocument.
/// Content is JSON (not PDF) — GetContentHashAsync returns the JSON serialized as UTF-8 bytes.
/// </summary>
public class SaijLegislationCrawlerSource : ICrawlerSource
{
    private const int SourceId = 2;

    private static readonly string[] NormTypes = ["ley", "decreto", "resolucion", "decision_administrativa", "acordada"];

    private readonly IOptions<SaijCrawlerOptions> _options;
    private readonly ILogger<SaijLegislationCrawlerSource> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public int? LastTotalSearchResults { get; private set; }

    public SaijLegislationCrawlerSource(
        IOptions<SaijCrawlerOptions> options,
        ILogger<SaijLegislationCrawlerSource> logger,
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
        var client = _httpClientFactory.CreateClient(nameof(SaijLegislationCrawlerSource));
        var pageSize = _options.Value.PageSize;
        var delay = _options.Value.ThrottlingDelayMs;
        var normType = message.DocumentType != "ruling" ? message.DocumentType : null;

        var offset = 0;
        var totalDiscovered = 0;
        var maxDocs = message.MaxDocuments ?? int.MaxValue;

        _logger.LogInformation("SAIJ Legislation: starting discovery (normType={NormType}, since={Since})",
            normType ?? "all", message.Since);

        while (totalDiscovered < maxDocs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var url = BuildSearchUrl(normType, offset, pageSize, message.Since);
            _logger.LogDebug("SAIJ Legislation: fetching {Url}", url);

            var response = await client.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<SaijSearchResult>(json, JsonOptions);

            if (result?.Results is null || result.Results.Count == 0)
            {
                _logger.LogInformation("SAIJ Legislation: no more results at offset {Offset}", offset);
                break;
            }

            if (offset == 0)
            {
                LastTotalSearchResults = result.Total;
                _logger.LogInformation("SAIJ Legislation: {Total} total documents reported", result.Total);
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
                    CaseNumber: doc.NroNorma));
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

        _logger.LogInformation("SAIJ Legislation: discovery complete, {Count} documents found", totalDiscovered);
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
            client = _httpClientFactory.CreateClient(nameof(SaijLegislationCrawlerSource));
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

    private string BuildSearchUrl(string? normType, int offset, int limit, DateOnly? since)
    {
        var sb = new StringBuilder("documentos?tipo=legislacion");
        sb.Append($"&limit={limit}&offset={offset}");
        sb.Append("&orden=fecha-publicacion:desc");

        if (!string.IsNullOrEmpty(normType))
            sb.Append($"&subtipo={Uri.EscapeDataString(normType)}");

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
        public List<SaijDocumentRef> Results { get; set; } = [];
    }

    private sealed class SaijDocumentRef
    {
        public string Id { get; set; } = "";
        public string? NroNorma { get; set; }
        public string? TipoNorma { get; set; }
        public string? Titulo { get; set; }
    }
}
