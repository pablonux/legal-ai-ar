using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling.Sources;

/// <summary>
/// HTTP-only CSJN crawler source for Fallos Destacados curated collection (~1,811 documents).
/// POST fallosDestacados/buscar.html → session → paginate all records via paginarFallos.html.
/// Reuses the same PDF download + cache pipeline as CsjnAcuerdoDiscoverySource.
/// </summary>
public partial class CsjnFallosDestacadosSource : ICrawlerSource
{
    private const int SourceId = 1;
    private const int PageSize = 10;
    private const int MaxPageRetries = 3;

    private static readonly string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";

    private readonly IOptions<CsjnCrawlerOptions> _options;
    private readonly ILogger<CsjnFallosDestacadosSource> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalDownloadCache _downloadCache;

    public int? LastTotalSearchResults { get; private set; }

    public CsjnFallosDestacadosSource(
        IOptions<CsjnCrawlerOptions> options,
        ILogger<CsjnFallosDestacadosSource> logger,
        IHttpClientFactory httpClientFactory,
        IExternalDownloadCache downloadCache)
    {
        _options = options;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _downloadCache = downloadCache;
    }

    public Task<int?> GetTotalForRangeAsync(CrawlerMessage message, CancellationToken cancellationToken = default)
        => Task.FromResult<int?>(null);

    public async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        CrawlerMessage message,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var baseUrl = _options.Value.BaseUrl.TrimEnd('/');

        _logger.LogInformation("CSJN Fallos Destacados: establishing session...");

        var (total, sessionClient) = await InitSessionAsync(baseUrl, cancellationToken);

        _logger.LogInformation("CSJN Fallos Destacados: {Total} documents found, starting pagination", total);

        LastTotalSearchResults = total;
        var totalFetched = 0;
        var pageIndex = 0;

        try
        {
            while (totalFetched < total)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await FetchPageAsync(sessionClient, baseUrl, pageIndex, cancellationToken);
                if (batch is null || batch.Count == 0)
                    break;

                totalFetched += batch.Count;
                yield return batch;

                if (batch.Count < PageSize)
                    break;

                pageIndex++;
            }
        }
        finally
        {
            sessionClient.Dispose();
        }

        _logger.LogInformation("CSJN Fallos Destacados: discovery completed. {Total} documents yielded", totalFetched);
    }

    public async Task<CrawlerDocumentContent> GetContentHashAsync(
        string documentId, string? analysisId, bool useCache = false,
        int? httpRequestTimeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CachePathHelper.PdfCacheKey(SourceId, documentId);

        if (useCache)
        {
            var cached = await _downloadCache.GetAsync(cacheKey, cancellationToken);
            if (cached != null)
            {
                _logger.LogInformation("CSJN FD: PDF from cache documentId={DocumentId}, size={Size}", documentId, cached.Length);
                return new CrawlerDocumentContent(ComputeSha256Hex(cached), cached);
            }
        }

        var opts = _options.Value;
        var pdfUrl = $"{opts.BaseUrl.TrimEnd('/')}/documentos/verDocumentoById.html?idDocumento={Uri.EscapeDataString(documentId)}";

        _logger.LogInformation("CSJN FD: downloading PDF documentId={DocumentId}", documentId);

        HttpClient? ownedClient = null;
        HttpClient httpClient;
        if (httpRequestTimeoutSeconds is > 0)
        {
            var seconds = Math.Clamp(httpRequestTimeoutSeconds.Value, 60, 900);
            ownedClient = CsjnPdfHttp.CreateClientForPdfDownload(opts.BaseUrl, seconds);
            httpClient = ownedClient;
            _logger.LogInformation(
                "CSJN FD: per-document PDF HTTP timeout={Timeout}s documentId={DocumentId}",
                seconds, documentId);
        }
        else
        {
            httpClient = _httpClientFactory.CreateClient(nameof(CsjnFallosDestacadosSource));
        }

        try
        {
            var attempt = 0;

            while (true)
            {
                var response = await httpClient.GetAsync(pdfUrl, cancellationToken);
                await ThrottleAsync(cancellationToken);

                var statusCode = (int)response.StatusCode;
                if (statusCode is 429 or 503)
                {
                    attempt++;
                    if (attempt > opts.ThrottlingMaxRetries)
                        response.EnsureSuccessStatusCode();

                    var delayMs = opts.ThrottlingDelayMs * Math.Pow(opts.ThrottlingBackoffMultiplier, attempt);
                    _logger.LogWarning("CSJN FD: HTTP {Status} for {DocumentId}, retry {Attempt}/{Max} in {Delay}ms",
                        statusCode, documentId, attempt, opts.ThrottlingMaxRetries, (int)delayMs);
                    await Task.Delay((int)delayMs, cancellationToken);
                    continue;
                }

                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);
                await _downloadCache.SetAsync(cacheKey, bytes, cancellationToken);
                return new CrawlerDocumentContent(ComputeSha256Hex(bytes), bytes);
            }
        }
        finally
        {
            ownedClient?.Dispose();
        }
    }

    private async Task<(int Total, HttpClient Client)> InitSessionAsync(string baseUrl, CancellationToken ct)
    {
        var buscarUrl = $"{baseUrl}/fallosDestacados/buscar.html";
        var formBody = "materia=&cabecilla=&fechaDesde=&fechaHasta=&g-recaptcha-response=dummy";

        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.All,
            UseCookies = true
        };
        var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(120) };
        client.DefaultRequestHeaders.Add("User-Agent", BrowserUserAgent);

        var content = new StringContent(formBody, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");
        var response = await client.PostAsync(buscarUrl, content, ct);
        await ThrottleAsync(ct);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(ct);
        var match = TotalResultsRegex().Match(html);
        if (!match.Success || !int.TryParse(match.Groups[1].Value, out var total))
            throw new InvalidOperationException("Could not extract totalResultados from fallosDestacados session.");

        client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
        client.DefaultRequestHeaders.Referrer = new Uri($"{baseUrl}/fallosDestacados/buscar.html");

        return (total, client);
    }

    private async Task<IReadOnlyList<DiscoveredDocument>?> FetchPageAsync(
        HttpClient sessionClient, string baseUrl, int pageIndex, CancellationToken ct)
    {
        var indice = pageIndex * PageSize;
        var url = $"{baseUrl}/fallosDestacados/paginarFallos.html?indice={indice}&ordenRelevancia=false";

        for (var attempt = 0; attempt <= MaxPageRetries; attempt++)
        {
            try
            {
                var response = await sessionClient.GetAsync(url, ct);
                await ThrottleAsync(ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                return ParseFallosDestacadosPage(json);
            }
            catch (HttpRequestException ex) when (attempt < MaxPageRetries)
            {
                var delay = (attempt + 1) * 5;
                _logger.LogWarning(ex, "CSJN FD: page {Page} failed (attempt {Attempt}/{Max}), retrying in {Delay}s",
                    pageIndex + 1, attempt + 1, MaxPageRetries + 1, delay);
                await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            }
        }

        _logger.LogError("CSJN FD: page {Page} failed after all retries", pageIndex + 1);
        return null;
    }

    private static IReadOnlyList<DiscoveredDocument> ParseFallosDestacadosPage(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        if (!root.TryGetProperty("Result", out var result) || result.GetString() != "OK")
            return [];

        if (!root.TryGetProperty("Records", out var records))
            return [];

        var batch = new List<DiscoveredDocument>();
        foreach (var record in records.EnumerateArray())
        {
            var analysisId = record.TryGetProperty("idAnalisis", out var idProp)
                ? idProp.ToString() : null;
            var documentId = record.TryGetProperty("codigo", out var codProp)
                ? codProp.ToString() : null;
            var fecha = record.TryGetProperty("fecha", out var fechaProp)
                ? fechaProp.GetString() : null;
            var expediente = record.TryGetProperty("identificadorExpediente", out var expProp)
                ? expProp.GetString() : null;

            if (string.IsNullOrEmpty(documentId) || string.IsNullOrEmpty(analysisId))
                continue;

            DateOnly? rulingDate = null;
            if (!string.IsNullOrEmpty(fecha) && DateTime.TryParse(fecha, out var dt))
                rulingDate = DateOnly.FromDateTime(dt);

            batch.Add(new DiscoveredDocument(
                documentId, analysisId, rulingDate,
                CaseNumber: expediente));
        }
        return batch;
    }

    private async Task ThrottleAsync(CancellationToken ct)
    {
        await Task.Delay(_options.Value.ThrottlingDelayMs, ct);
    }

    private static string ComputeSha256Hex(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    [GeneratedRegex(@"totalResultados\s*=\s*""(\d+)""")]
    private static partial Regex TotalResultsRegex();
}
