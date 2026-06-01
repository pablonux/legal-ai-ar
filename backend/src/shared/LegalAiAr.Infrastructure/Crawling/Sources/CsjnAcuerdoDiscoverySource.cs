using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Threading.Channels;
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
/// HTTP-only CSJN crawler source. Discovers rulings by iterating acuerdo dates
/// via GET consultaAcuerdo.html?fecha=DD/MM/YYYY, then paginating with
/// paginarFallos.html?jtStartIndex=N (same JSON format as Selenium flow).
/// Eliminates the Selenium dependency for discovery.
/// </summary>
public partial class CsjnAcuerdoDiscoverySource : ICrawlerSource
{
    private const int SourceId = 1;
    private const int PageSize = 10;
    private const int MaxPageRetries = 3;
    private static readonly int[] PageRetryDelaysSeconds = [5, 15, 30];

    private static readonly string BrowserUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";

    private readonly ICrawlerConfigRepository _configRepository;
    private readonly IOptions<CsjnCrawlerOptions> _options;
    private readonly ILogger<CsjnAcuerdoDiscoverySource> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IExternalDownloadCache _downloadCache;
    private readonly CsjnDiscoveryHttpGate _discoveryHttpGate;

    public int? LastTotalSearchResults { get; private set; }

    public CsjnAcuerdoDiscoverySource(
        ICrawlerConfigRepository configRepository,
        IOptions<CsjnCrawlerOptions> options,
        ILogger<CsjnAcuerdoDiscoverySource> logger,
        IHttpClientFactory httpClientFactory,
        IExternalDownloadCache downloadCache,
        CsjnDiscoveryHttpGate discoveryHttpGate)
    {
        _configRepository = configRepository;
        _options = options;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _downloadCache = downloadCache;
        _discoveryHttpGate = discoveryHttpGate;
    }

    /// <summary>
    /// Not directly supported for acuerdo-based discovery since totals are per-date.
    /// Returns null to skip pre-flight split logic (each acuerdo date is well under 5000).
    /// </summary>
    public Task<int?> GetTotalForRangeAsync(CrawlerMessage message, CancellationToken cancellationToken = default)
        => Task.FromResult<int?>(null);

    public async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverAsync(
        CrawlerMessage message,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (message.SourceId != SourceId)
            throw new ArgumentException($"CsjnAcuerdoDiscoverySource only supports SourceId {SourceId}.", nameof(message));

        var (fechaDesde, fechaHasta) = await ResolveDateRangeAsync(message, cancellationToken);
        var baseUrl = _options.Value.BaseUrl.TrimEnd('/');

        var dates = new List<DateOnly>();
        for (var d = fechaDesde; d <= fechaHasta; d = d.AddDays(1))
            dates.Add(d);

        if (dates.Count == 0)
        {
            LastTotalSearchResults = 0;
            _logger.LogInformation("CSJN Acuerdo discovery: empty date range, nothing to do");
            yield break;
        }

        var opts = _options.Value;
        var dateConcurrency = Math.Clamp(opts.DiscoveryDateConcurrency, 1, 8);

        _logger.LogInformation(
            "CSJN Acuerdo discovery started: {From:dd/MM/yyyy} - {To:dd/MM/yyyy}, {DayCount} day(s), DateConcurrency={DateConcurrency}, DiscoveryHttpMaxConcurrency={HttpCap}",
            fechaDesde, fechaHasta, dates.Count, dateConcurrency, opts.DiscoveryHttpMaxConcurrency);

        var discoverySw = Stopwatch.StartNew();
        var totalDiscovered = 0;

        if (dateConcurrency <= 1 || dates.Count <= 1)
        {
            await foreach (var batch in DiscoverSequentialDatesAsync(baseUrl, dates, cancellationToken))
            {
                totalDiscovered += batch.Count;
                yield return batch;
            }
        }
        else
        {
            await foreach (var batch in DiscoverDateWavesAsync(baseUrl, dates, dateConcurrency, cancellationToken))
            {
                totalDiscovered += batch.Count;
                yield return batch;
            }
        }

        LastTotalSearchResults = totalDiscovered;
        _logger.LogInformation(
            "CSJN Acuerdo discovery completed: {Total} total documents in {ElapsedMs} ms",
            totalDiscovered, discoverySw.ElapsedMilliseconds);
    }

    private async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverSequentialDatesAsync(
        string baseUrl,
        IReadOnlyList<DateOnly> dates,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        foreach (var cursor in dates)
        {
            ct.ThrowIfCancellationRequested();
            await foreach (var batch in EnumerateBatchesForSingleDateAsync(baseUrl, cursor, ct))
                yield return batch;
        }
    }

    /// <summary>
    /// Processes dates in waves of <paramref name="waveSize"/> concurrent producers; yields batches in strict chronological date order.
    /// </summary>
    private async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> DiscoverDateWavesAsync(
        string baseUrl,
        IReadOnlyList<DateOnly> dates,
        int waveSize,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        for (var waveStart = 0; waveStart < dates.Count; waveStart += waveSize)
        {
            ct.ThrowIfCancellationRequested();
            var wave = dates.Skip(waveStart).Take(waveSize).ToList();
            var channels = wave.ToDictionary(
                d => d,
                _ => Channel.CreateUnbounded<IReadOnlyList<DiscoveredDocument>>());

            var producerTasks = wave.Select(date => ProduceDateBatchesToChannelAsync(baseUrl, date, channels[date].Writer, ct))
                .ToArray();

            await Task.WhenAll(producerTasks).ConfigureAwait(false);

            foreach (var date in wave)
            {
                await foreach (var batch in channels[date].Reader.ReadAllAsync(ct))
                    yield return batch;
            }
        }
    }

    private async Task ProduceDateBatchesToChannelAsync(
        string baseUrl,
        DateOnly cursor,
        ChannelWriter<IReadOnlyList<DiscoveredDocument>> writer,
        CancellationToken ct)
    {
        try
        {
            await foreach (var batch in EnumerateBatchesForSingleDateAsync(baseUrl, cursor, ct).ConfigureAwait(false))
                await writer.WriteAsync(batch, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CSJN Acuerdo: failed during discovery for date {Date:dd/MM/yyyy}", cursor);
        }
        finally
        {
            writer.TryComplete();
        }
    }

    private async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> EnumerateBatchesForSingleDateAsync(
        string baseUrl,
        DateOnly cursor,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var dateStr = cursor.ToString("dd/MM/yyyy");
        var dateSw = Stopwatch.StartNew();
        var pageCount = 0;

        var session = await InitAcuerdoSessionAsync(baseUrl, dateStr, ct).ConfigureAwait(false);

        if (session is null)
            yield break;

        var (totalForDate, client) = session.Value;

        _logger.LogInformation(
            "CSJN Acuerdo {Date}: {Total} fallos found",
            dateStr, totalForDate);

        try
        {
            if (totalForDate <= 0)
                yield break;

            var totalFetched = 0;
            await foreach (var batch in PaginateAcuerdoAsync(client, baseUrl, dateStr, cursor, totalForDate, ct).ConfigureAwait(false))
            {
                pageCount++;
                totalFetched += batch.Count;
                yield return batch;
            }

            if (totalFetched < totalForDate)
            {
                _logger.LogWarning(
                    "CSJN Acuerdo {Date}: pagination stopped early — fetched {Fetched} of {Expected} (possible empty page or session issue)",
                    dateStr, totalFetched, totalForDate);
            }
        }
        finally
        {
            client.Dispose();
            _logger.LogInformation(
                "CSJN Acuerdo {Date}: done in {ElapsedMs} ms, pages={Pages}",
                dateStr, dateSw.ElapsedMilliseconds, pageCount);
        }
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
                _logger.LogInformation(
                    "CSJN: PDF from cache documentId={DocumentId}, size={Size}",
                    documentId, cached.Length);
                return new CrawlerDocumentContent(cachedHash, cached);
            }
        }

        var opts = _options.Value;
        var baseUrl = opts.BaseUrl.TrimEnd('/');
        var pdfUrl = $"{baseUrl}/documentos/verDocumentoById.html?idDocumento={Uri.EscapeDataString(documentId)}";

        _logger.LogInformation("CSJN: Downloading PDF documentId={DocumentId}", documentId);

        HttpClient? ownedClient = null;
        HttpClient httpClient;
        if (httpRequestTimeoutSeconds is > 0)
        {
            var seconds = Math.Clamp(httpRequestTimeoutSeconds.Value, 60, 900);
            ownedClient = CsjnPdfHttp.CreateClientForPdfDownload(opts.BaseUrl, seconds);
            httpClient = ownedClient;
            _logger.LogInformation(
                "CSJN: Using per-document PDF HTTP timeout={Timeout}s documentId={DocumentId}",
                seconds, documentId);
        }
        else
        {
            httpClient = _httpClientFactory.CreateClient(nameof(CsjnAcuerdoDiscoverySource));
        }

        try
        {
            var attempt = 0;

            while (true)
            {
                var response = await httpClient.GetAsync(pdfUrl, cancellationToken).ConfigureAwait(false);
                await ThrottleAsync(cancellationToken).ConfigureAwait(false);

                var statusCode = (int)response.StatusCode;

                if (statusCode is 429 or 503)
                {
                    attempt++;
                    if (attempt > opts.ThrottlingMaxRetries)
                        response.EnsureSuccessStatusCode();

                    var delayMs = opts.ThrottlingDelayMs * Math.Pow(opts.ThrottlingBackoffMultiplier, attempt);
                    _logger.LogWarning(
                        "CSJN: HTTP {Status} downloading PDF {DocumentId}, retry {Attempt}/{Max} in {Delay}ms",
                        statusCode, documentId, attempt, opts.ThrottlingMaxRetries, (int)delayMs);
                    await Task.Delay((int)delayMs, cancellationToken).ConfigureAwait(false);
                    continue;
                }

                response.EnsureSuccessStatusCode();
                var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
                await _downloadCache.SetAsync(cacheKey, bytes, cancellationToken).ConfigureAwait(false);

                var hash = ComputeSha256Hex(bytes);
                _logger.LogInformation(
                    "CSJN: PDF downloaded documentId={DocumentId}, size={Size}",
                    documentId, bytes.Length);
                return new CrawlerDocumentContent(hash, bytes);
            }
        }
        finally
        {
            ownedClient?.Dispose();
        }
    }

    /// <summary>
    /// Fetches the consultaAcuerdo page for a given date, establishing a session and extracting the total.
    /// Returns null if the date has no results or the request fails.
    /// </summary>
    private async Task<(int Total, HttpClient Client)?> InitAcuerdoSessionAsync(
        string baseUrl, string dateStr, CancellationToken ct)
    {
        var url = $"{baseUrl}/fallos/consultaAcuerdo.html?fecha={Uri.EscapeDataString(dateStr)}";

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
            using var response = await SendDiscoveryGetAsync(client, url, ct).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("CSJN Acuerdo: HTTP {Status} for date {Date}", (int)response.StatusCode, dateStr);
                client.Dispose();
                return null;
            }

            var html = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            var total = ParseTotalFromHtml(html);

            if (total == 0)
            {
                _logger.LogDebug("CSJN Acuerdo: no results for date {Date}", dateStr);
                client.Dispose();
                return null;
            }

            client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            client.DefaultRequestHeaders.Add("Accept", "application/json, text/javascript, */*; q=0.01");
            client.DefaultRequestHeaders.Referrer = new Uri($"{baseUrl}/fallos/consultaAcuerdo.html");

            return (total, client);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "CSJN Acuerdo: failed to init session for date {Date}", dateStr);
            client.Dispose();
            return null;
        }
    }

    private async Task<HttpResponseMessage> SendDiscoveryGetAsync(HttpClient client, string url, CancellationToken ct)
    {
        await using var _ = await _discoveryHttpGate.AcquireAsync(ct).ConfigureAwait(false);
        var response = await client.GetAsync(url, ct).ConfigureAwait(false);
        await ThrottleAsync(ct).ConfigureAwait(false);
        return response;
    }

    private async IAsyncEnumerable<IReadOnlyList<DiscoveredDocument>> PaginateAcuerdoAsync(
        HttpClient sessionClient,
        string baseUrl,
        string dateStr,
        DateOnly acuerdoDate,
        int expectedTotal,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var page = 0;
        var totalFetched = 0;

        while (totalFetched < expectedTotal)
        {
            ct.ThrowIfCancellationRequested();

            var records = await FetchPageAsync(sessionClient, baseUrl, page, dateStr, ct).ConfigureAwait(false);

            if (records == null || records.Count == 0)
            {
                _logger.LogDebug("CSJN Acuerdo {Date}: page {Page} empty, stopping", dateStr, page + 1);
                break;
            }

            _logger.LogDebug("CSJN Acuerdo {Date}: page {Page} returned {Count} records", dateStr, page + 1, records.Count);

            var batch = records
                .Select(r => new DiscoveredDocument(
                    r.DocumentId, r.AnalysisId, acuerdoDate,
                    CaseNumber: string.IsNullOrEmpty(r.CaseNumber) ? null : r.CaseNumber))
                .ToList();

            totalFetched += batch.Count;
            yield return batch;

            if (records.Count < PageSize)
                break;

            page++;
        }
    }

    private async Task<IReadOnlyList<DiscoveredRecord>?> FetchPageAsync(
        HttpClient client, string baseUrl, int pageIndex, string dateContext, CancellationToken ct)
    {
        var url = $"{baseUrl}/fallos/paginarFallos.html?jtStartIndex={pageIndex * PageSize}";

        for (var attempt = 0; attempt <= MaxPageRetries; attempt++)
        {
            try
            {
                using var response = await SendDiscoveryGetAsync(client, url, ct).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
                return CsjnPaginationParser.ParseRecordsWithMetadata(json, pageIndex);
            }
            catch (HttpRequestException ex) when (attempt < MaxPageRetries)
            {
                var delay = PageRetryDelaysSeconds[Math.Min(attempt, PageRetryDelaysSeconds.Length - 1)];
                _logger.LogWarning(ex,
                    "CSJN Acuerdo {Date}: page {Page} failed (attempt {Attempt}/{Max}), retrying in {Delay}s",
                    dateContext, pageIndex + 1, attempt + 1, MaxPageRetries + 1, delay);
                await Task.Delay(TimeSpan.FromSeconds(delay), ct).ConfigureAwait(false);
            }
        }

        _logger.LogError("CSJN Acuerdo {Date}: page {Page} failed after all retries", dateContext, pageIndex + 1);
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
                var config = await _configRepository.GetBySourceIdAsync(SourceId, ct).ConfigureAwait(false);
                fechaDesde = config?.LastCrawledAt != null
                    ? DateOnly.FromDateTime(config.LastCrawledAt.Value)
                    : today.AddDays(-7);
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
        await Task.Delay(_options.Value.ThrottlingDelayMs, ct).ConfigureAwait(false);
    }

    private static string ComputeSha256Hex(byte[] data)
    {
        var hash = SHA256.HashData(data);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    [GeneratedRegex(@"totalResultados\s*=\s*""(\d+)""")]
    private static partial Regex TotalResultsRegex();
}
