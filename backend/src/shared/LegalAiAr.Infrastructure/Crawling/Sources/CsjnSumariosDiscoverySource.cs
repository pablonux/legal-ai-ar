using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling.Sources;

/// <summary>
/// HTTP-only CSJN crawler source for sumarios (1863-2026).
/// Uses POST consultaSumarios/buscar.html + GET paginarSumarios.html for discovery.
/// Each sumario yields idFallo (maps to AnalysisId) and the DocumentId is resolved
/// via getAllDocumentos. Covers historical rulings not available in the fallos completos DB.
/// </summary>
public partial class CsjnSumariosDiscoverySource : ICrawlerSource
{
    private const int SourceId = 1;
    private const int MaxResultsPerSearch = 5000;
    private const int MaxPageRetries = 3;
    private static readonly int[] PageRetryDelaysSeconds = [5, 15, 30];

    private static readonly string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";

    private readonly ICrawlerConfigRepository _configRepository;
    private readonly IOptions<CsjnCrawlerOptions> _options;
    private readonly ILogger<CsjnSumariosDiscoverySource> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalDownloadCache _downloadCache;

    public int? LastTotalSearchResults { get; private set; }

    public CsjnSumariosDiscoverySource(
        ICrawlerConfigRepository configRepository,
        IOptions<CsjnCrawlerOptions> options,
        ILogger<CsjnSumariosDiscoverySource> logger,
        IHttpClientFactory httpClientFactory,
        IExternalDownloadCache downloadCache)
    {
        _configRepository = configRepository;
        _options = options;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _downloadCache = downloadCache;
    }

    public async Task<int?> GetTotalForRangeAsync(CrawlerMessage message, CancellationToken cancellationToken = default)
    {
        if (!message.DateFrom.HasValue || !message.DateTo.HasValue)
            return null;

        var baseUrl = _options.Value.BaseUrl.TrimEnd('/');
        var fechaDesde = message.DateFrom.Value.ToString("dd/MM/yyyy");
        var fechaHasta = message.DateTo.Value.ToString("dd/MM/yyyy");

        using var session = await InitSumariosSessionAsync(baseUrl, cancellationToken);
        if (session is null) return null;

        var total = await SubmitSearchAsync(session, baseUrl, "", "", fechaDesde, fechaHasta, cancellationToken);
        return total;
    }

    public async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        CrawlerMessage message,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (message.SourceId != SourceId)
            throw new ArgumentException($"CsjnSumariosDiscoverySource only supports SourceId {SourceId}.", nameof(message));

        var (fechaDesde, fechaHasta) = await ResolveDateRangeAsync(message, cancellationToken);
        var baseUrl = _options.Value.BaseUrl.TrimEnd('/');

        _logger.LogInformation(
            "CSJN Sumarios discovery started: {From:dd/MM/yyyy} - {To:dd/MM/yyyy}",
            fechaDesde, fechaHasta);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        using var session = await InitSumariosSessionAsync(baseUrl, cancellationToken);
        if (session is null)
        {
            _logger.LogError("CSJN Sumarios: failed to initialize session");
            yield break;
        }

        var total = await SubmitSearchAsync(
            session, baseUrl, "", "",
            fechaDesde.ToString("dd/MM/yyyy"),
            fechaHasta.ToString("dd/MM/yyyy"),
            cancellationToken);

        _logger.LogInformation("CSJN Sumarios: {Total} results for range", total);
        LastTotalSearchResults = total;

        if (total == 0)
            yield break;

        var startIndex = 0;
        var fetched = 0;

        while (fetched < total)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var items = await FetchSumariosPageAsync(session, baseUrl, startIndex, cancellationToken);
            if (items == null || items.Count == 0)
                break;

            var batch = new List<DiscoveredDocument>();
            foreach (var item in items)
            {
                if (item.AnalysisId is not null && item.DocumentId is not null)
                    batch.Add(new DiscoveredDocument(
                        item.DocumentId, item.AnalysisId,
                        CaseNumber: string.IsNullOrEmpty(item.Expediente) ? null : item.Expediente));
            }

            if (batch.Count > 0)
            {
                fetched += batch.Count;
                yield return batch;
            }

            startIndex += items.Count;

            if (items.Count == 0)
                break;
        }

        if (fetched < total)
        {
            _logger.LogWarning(
                "CSJN Sumarios: pagination ended early — fetched {Fetched} of {Expected} (empty page or parse issue)",
                fetched, total);
        }

        _logger.LogInformation(
            "CSJN Sumarios discovery completed: {Fetched} documents in {ElapsedMs} ms",
            fetched, sw.ElapsedMilliseconds);
    }

    public async Task<CrawlerDocumentContent> GetContentHashAsync(
        string documentId,
        string? analysisId,
        bool useCache = false,
        int? httpRequestTimeoutSeconds = null,
        CancellationToken cancellationToken = default)
    {
        var cacheKey = CachePathHelper.PdfCacheKey(SourceId, documentId);

        if (useCache)
        {
            var cached = await _downloadCache.GetAsync(cacheKey, cancellationToken);
            if (cached != null)
            {
                var cachedHash = ComputeSha256Hex(cached);
                return new CrawlerDocumentContent(cachedHash, cached);
            }
        }

        var opts = _options.Value;
        var baseUrl = opts.BaseUrl.TrimEnd('/');
        var pdfUrl = $"{baseUrl}/documentos/verDocumentoById.html?idDocumento={Uri.EscapeDataString(documentId)}";

        _logger.LogInformation("CSJN Sumarios: downloading PDF documentId={DocumentId}", documentId);

        HttpClient? ownedClient = null;
        HttpClient httpClient;
        if (httpRequestTimeoutSeconds is > 0)
        {
            var seconds = Math.Clamp(httpRequestTimeoutSeconds.Value, 60, 900);
            ownedClient = CsjnPdfHttp.CreateClientForPdfDownload(opts.BaseUrl, seconds);
            httpClient = ownedClient;
            _logger.LogInformation(
                "CSJN Sumarios: per-document PDF HTTP timeout={Timeout}s documentId={DocumentId}",
                seconds, documentId);
        }
        else
        {
            httpClient = _httpClientFactory.CreateClient(nameof(CsjnSumariosDiscoverySource));
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

    private async Task<HttpClient?> InitSumariosSessionAsync(string baseUrl, CancellationToken ct)
    {
        var url = $"{baseUrl}/consultaSumarios/consulta.html";

        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.All,
            UseCookies = true
        };
        var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(30) };
        client.DefaultRequestHeaders.Add("User-Agent", BrowserUserAgent);

        try
        {
            var response = await client.GetAsync(url, ct);
            await ThrottleAsync(ct);
            response.EnsureSuccessStatusCode();

            _logger.LogDebug("CSJN Sumarios: session initialized");
            return client;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CSJN Sumarios: failed to init session");
            client.Dispose();
            return null;
        }
    }

    private async Task<int> SubmitSearchAsync(
        HttpClient session, string baseUrl,
        string fullText, string autos,
        string fechaDesde, string fechaHasta,
        CancellationToken ct)
    {
        var url = $"{baseUrl}/consultaSumarios/buscar.html";

        var formData = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["filter.fullText"] = fullText,
            ["filter.autos"] = autos,
            ["filter.fechaExacta"] = "",
            ["filter.fechaDesde"] = fechaDesde,
            ["filter.fechaHasta"] = fechaHasta
        });

        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = formData
        };
        request.Headers.Referrer = new Uri($"{baseUrl}/consultaSumarios/consulta.html");

        var response = await session.SendAsync(request, ct);
        await ThrottleAsync(ct);
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync(ct);
        return ParseTotalFromHtml(html);
    }

    private async Task<IReadOnlyList<SumarioRecord>?> FetchSumariosPageAsync(
        HttpClient session, string baseUrl, int startIndex, CancellationToken ct)
    {
        var url = $"{baseUrl}/consultaSumarios/paginarSumarios.html?startIndex={startIndex}";

        for (var attempt = 0; attempt <= MaxPageRetries; attempt++)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Add("X-Requested-With", "XMLHttpRequest");

                var response = await session.SendAsync(request, ct);
                await ThrottleAsync(ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);
                return ParseSumariosResponse(json);
            }
            catch (HttpRequestException ex) when (attempt < MaxPageRetries)
            {
                var delay = PageRetryDelaysSeconds[Math.Min(attempt, PageRetryDelaysSeconds.Length - 1)];
                _logger.LogWarning(ex,
                    "CSJN Sumarios: page at startIndex={Start} failed (attempt {Attempt}), retrying in {Delay}s",
                    startIndex, attempt + 1, delay);
                await Task.Delay(TimeSpan.FromSeconds(delay), ct);
            }
        }

        return null;
    }

    private static IReadOnlyList<SumarioRecord> ParseSumariosResponse(string json)
    {
        var results = new List<SumarioRecord>();
        if (string.IsNullOrWhiteSpace(json))
            return results;

        try
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.ValueKind != JsonValueKind.Array)
                return results;

            foreach (var item in root.EnumerateArray())
            {
                var idFallo = GetStringProp(item, "idFallo");
                var id = GetStringProp(item, "id");
                var autos = GetStringProp(item, "autos") ?? GetStringProp(item, "caratulaWeb");
                var fecha = GetStringProp(item, "fechaString");
                var tomo = GetStringProp(item, "tomo");
                var pagina = GetStringProp(item, "pagina");
                var expediente = GetStringProp(item, "numeroExpediente");

                if (idFallo is null && id is null)
                    continue;

                results.Add(new SumarioRecord(
                    AnalysisId: idFallo ?? id,
                    DocumentId: idFallo ?? id,
                    Autos: autos,
                    Fecha: fecha,
                    Tomo: tomo,
                    Pagina: pagina,
                    Expediente: expediente));
            }
        }
        catch (JsonException)
        {
            // Malformed response
        }

        return results;
    }

    private static string? GetStringProp(JsonElement el, string name)
    {
        if (el.TryGetProperty(name, out var prop))
        {
            if (prop.ValueKind == JsonValueKind.String)
                return prop.GetString();
            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetRawText();
        }
        return null;
    }

    private async Task<(DateOnly fechaDesde, DateOnly fechaHasta)> ResolveDateRangeAsync(
        CrawlerMessage message, CancellationToken ct)
    {
        var isIncremental = string.Equals(message.Type, "incremental", StringComparison.OrdinalIgnoreCase);
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (isIncremental)
        {
            DateOnly fechaDesde;
            if (message.Since.HasValue)
            {
                fechaDesde = message.Since.Value;
            }
            else
            {
                var config = await _configRepository.GetBySourceIdAsync(SourceId, ct);
                fechaDesde = config?.LastCrawledAt != null
                    ? DateOnly.FromDateTime(config.LastCrawledAt.Value)
                    : today.AddDays(-30);
            }
            return (fechaDesde, today);
        }

        if (!message.DateFrom.HasValue || !message.DateTo.HasValue)
            throw new InvalidOperationException("CrawlerMessage by-range requires DateFrom and DateTo.");

        return (message.DateFrom.Value, message.DateTo.Value);
    }

    private static int ParseTotalFromHtml(string html)
    {
        var match = TotalResultsRegex().Match(html);
        return match.Success && int.TryParse(match.Groups[1].Value, out var total)
            ? total
            : 0;
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

    private sealed record SumarioRecord(
        string? AnalysisId,
        string? DocumentId,
        string? Autos,
        string? Fecha,
        string? Tomo,
        string? Pagina,
        string? Expediente);
}
