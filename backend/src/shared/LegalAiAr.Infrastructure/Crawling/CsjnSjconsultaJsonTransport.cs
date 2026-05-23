using System.Text;
using System.Text.Json;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Blob;
using LegalAiAr.Infrastructure.Crawling.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Infrastructure.Crawling;

/// <summary>
/// HTTP + blob cache for CSJN sjconsulta JSON endpoints. Used by Fetcher (always outbound) and Parser (optional outbound).
/// </summary>
public sealed class CsjnSjconsultaJsonTransport
{
    private const int SourceId = 1;

    private static readonly int[] TransientRetryDelaysSeconds = [5, 15, 30];

    private readonly HttpClient _httpClient;
    private readonly IOptions<CsjnApiOptions> _options;
    private readonly ILogger<CsjnSjconsultaJsonTransport> _logger;
    private readonly IExternalDownloadCache _downloadCache;
    private readonly CsjnApiRequestGate _requestGate;

    public CsjnSjconsultaJsonTransport(
        HttpClient httpClient,
        IOptions<CsjnApiOptions> options,
        ILogger<CsjnSjconsultaJsonTransport> logger,
        IExternalDownloadCache downloadCache,
        CsjnApiRequestGate requestGate)
    {
        _httpClient = httpClient;
        _options = options;
        _logger = logger;
        _downloadCache = downloadCache;
        _requestGate = requestGate;
    }

    /// <summary>
    /// Fetches all sjconsulta JSON for a ruling and writes through to blob cache (same keys as Parser).
    /// Always uses outbound HTTP on cache miss.
    /// </summary>
    public async Task PrimeSjconsultaCachesAsync(string documentId, string analysisId, bool useCache, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(analysisId))
            throw new ArgumentException("AnalysisId is required.", nameof(analysisId));

        var baseUrl = _options.Value.BaseUrl.TrimEnd('/');

        var abrir = await GetJsonWithRetryAsync(
                $"{baseUrl}/fallos/abrirAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
                documentId,
                CachePathHelper.ApiCacheKey(SourceId, "abrirAnalisis", analysisId),
                useCache,
                allowOutboundHttp: true,
                ct)
            .ConfigureAwait(false);

        if (abrir.ValueKind == JsonValueKind.Object && !abrir.EnumerateObject().Any())
        {
            throw new InvalidOperationException(
                $"CSJN abrirAnalisis returned empty JSON for analysisId={analysisId}, documentId={documentId}.");
        }

        await FetchPostAbrirEndpointsAsync(baseUrl, documentId, analysisId, useCache, allowOutboundHttp: true, ct)
            .ConfigureAwait(false);
    }

    /// <summary>
    /// Fetches the seven CSJN endpoints after <c>abrirAnalisis</c>, either sequentially or in parallel.
    /// </summary>
    public async Task<(JsonElement documentos, JsonElement sumarios, JsonElement citas, JsonElement citantes, JsonElement dictamenes, JsonElement sintesisJson, JsonElement enlacesJson)> FetchPostAbrirEndpointsAsync(
        string baseUrl,
        string documentId,
        string analysisId,
        bool useCache,
        bool allowOutboundHttp,
        CancellationToken cancellationToken)
    {
        if (!_options.Value.PostAbrirParallelEnabled)
        {
            var documentos = await GetJsonWithRetryAsync(
                    $"{baseUrl}/documentos/getAllDocumentos.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getAllDocumentos", analysisId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            var sumarios = await GetJsonWithRetryAsync(
                    $"{baseUrl}/sumarios/getSumariosAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getSumariosAnalisis", analysisId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            var citas = await GetJsonWithRetryAsync(
                    $"{baseUrl}/documentos/getCitas.html?idDocumento={Uri.EscapeDataString(documentId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getCitas", documentId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            var citantes = await GetJsonWithRetryAsync(
                    $"{baseUrl}/documentos/getCitantes.html?idDocumento={Uri.EscapeDataString(documentId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getCitantes", documentId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            var dictamenes = await GetJsonWithRetryAsync(
                    $"{baseUrl}/fallos/getDictamenesAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getDictamenesAnalisis", analysisId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            var sintesisJson = await GetJsonWithRetryAsync(
                    $"{baseUrl}/fallos/getSintesisAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getSintesisAnalisis", analysisId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            var enlacesJson = await GetJsonWithRetryAsync(
                    $"{baseUrl}/fallos/getEnlacesAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
                    documentId,
                    CachePathHelper.ApiCacheKey(SourceId, "getEnlacesAnalisis", analysisId),
                    useCache,
                    allowOutboundHttp,
                    cancellationToken)
                .ConfigureAwait(false);

            return (documentos, sumarios, citas, citantes, dictamenes, sintesisJson, enlacesJson);
        }

        var documentosTask = GetJsonWithRetryAsync(
            $"{baseUrl}/documentos/getAllDocumentos.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getAllDocumentos", analysisId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        var sumariosTask = GetJsonWithRetryAsync(
            $"{baseUrl}/sumarios/getSumariosAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getSumariosAnalisis", analysisId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        var citasTask = GetJsonWithRetryAsync(
            $"{baseUrl}/documentos/getCitas.html?idDocumento={Uri.EscapeDataString(documentId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getCitas", documentId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        var citantesTask = GetJsonWithRetryAsync(
            $"{baseUrl}/documentos/getCitantes.html?idDocumento={Uri.EscapeDataString(documentId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getCitantes", documentId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        var dictamenesTask = GetJsonWithRetryAsync(
            $"{baseUrl}/fallos/getDictamenesAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getDictamenesAnalisis", analysisId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        var sintesisTask = GetJsonWithRetryAsync(
            $"{baseUrl}/fallos/getSintesisAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getSintesisAnalisis", analysisId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        var enlacesTask = GetJsonWithRetryAsync(
            $"{baseUrl}/fallos/getEnlacesAnalisis.html?idAnalisis={Uri.EscapeDataString(analysisId)}",
            documentId,
            CachePathHelper.ApiCacheKey(SourceId, "getEnlacesAnalisis", analysisId),
            useCache,
            allowOutboundHttp,
            cancellationToken);

        await Task.WhenAll(documentosTask, sumariosTask, citasTask, citantesTask, dictamenesTask, sintesisTask, enlacesTask)
            .ConfigureAwait(false);

        return (
            await documentosTask.ConfigureAwait(false),
            await sumariosTask.ConfigureAwait(false),
            await citasTask.ConfigureAwait(false),
            await citantesTask.ConfigureAwait(false),
            await dictamenesTask.ConfigureAwait(false),
            await sintesisTask.ConfigureAwait(false),
            await enlacesTask.ConfigureAwait(false));
    }

    public async Task<JsonElement> GetJsonWithRetryAsync(
        string url,
        string contextDocumentId,
        string? cacheKey,
        bool useCache,
        bool allowOutboundHttp,
        CancellationToken ct)
    {
        var key = cacheKey ?? string.Empty;
        var tryReadBlob = !string.IsNullOrEmpty(key) &&
                          (useCache || _options.Value.PreferBlobApiCacheBeforeHttp || !allowOutboundHttp);
        if (tryReadBlob)
        {
            var cached = await _downloadCache.GetAsync(key, ct).ConfigureAwait(false);
            if (cached != null)
            {
                var json = Encoding.UTF8.GetString(cached);
                _logger.LogDebug("Cache HIT for {Url} (key={CacheKey})", url, cacheKey);
                if (string.IsNullOrWhiteSpace(json))
                {
                    using var empty = JsonDocument.Parse("{}");
                    return empty.RootElement.Clone();
                }

                using var cachedDoc = JsonDocument.Parse(json);
                return cachedDoc.RootElement.Clone();
            }

            _logger.LogDebug("Cache MISS for {Url}, fetching from source", url);
        }

        if (!allowOutboundHttp)
        {
            throw new InvalidOperationException(
                $"CSJN API cache miss for URL while OutboundHttpEnabled=false (key={cacheKey}, documentId={contextDocumentId}). " +
                "Ensure the Fetcher primed sjconsulta JSON or enable OutboundHttpEnabled for this worker.");
        }

        var opts = _options.Value;
        var attempt = 0;
        var transientAttempt = 0;

        while (true)
        {
            await using var _ = await _requestGate.AcquireAsync(ct).ConfigureAwait(false);
            var response = await _httpClient.GetAsync(url, ct).ConfigureAwait(false);

            var statusCode = (int)response.StatusCode;

            if (statusCode == 429)
            {
                attempt++;
                if (attempt > opts.ThrottlingMaxRetries)
                {
                    response.EnsureSuccessStatusCode();
                }

                var delayMs = opts.ThrottlingDelayMs * Math.Pow(opts.ThrottlingBackoffMultiplier, attempt);
                await Task.Delay((int)delayMs, ct).ConfigureAwait(false);
                continue;
            }

            if (statusCode is 503 or 502 or 504)
            {
                transientAttempt++;
                if (transientAttempt > TransientRetryDelaysSeconds.Length)
                {
                    _logger.LogError("HTTP {StatusCode} for {Url} after {Attempts} retries, giving up",
                        statusCode, url, transientAttempt);
                    response.EnsureSuccessStatusCode();
                }

                var delaySec = TransientRetryDelaysSeconds[transientAttempt - 1];
                _logger.LogWarning("HTTP {StatusCode} for {Url}, retrying in {Delay}s (attempt {Attempt}/{Max})",
                    statusCode, url, delaySec, transientAttempt, TransientRetryDelaysSeconds.Length);
                await Task.Delay(TimeSpan.FromSeconds(delaySec), ct).ConfigureAwait(false);
                continue;
            }

            if (statusCode == 404)
            {
                _logger.LogDebug("HTTP 404 for {Url} — treating as empty response", url);
                if (!string.IsNullOrEmpty(cacheKey))
                {
                    await _downloadCache.SetAsync(cacheKey, "{}"u8.ToArray(), ct).ConfigureAwait(false);
                }

                using var notFound = JsonDocument.Parse("{}");
                return notFound.RootElement.Clone();
            }

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);

            // Always write-through when we have a key so Parser (OutboundHttpEnabled=false) can blob-read
            // empty or whitespace bodies as "{}" — previously empty 200s skipped SetAsync and caused cache miss.
            if (!string.IsNullOrEmpty(cacheKey))
            {
                var toStore = string.IsNullOrWhiteSpace(content) ? "{}" : content;
                await _downloadCache.SetAsync(cacheKey, Encoding.UTF8.GetBytes(toStore), ct).ConfigureAwait(false);
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                using var empty = JsonDocument.Parse("{}");
                return empty.RootElement.Clone();
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                return doc.RootElement.Clone();
            }
            catch (JsonException ex)
            {
                throw new CsjnSchemaViolationException(
                    $"Invalid JSON response from {url}: {ex.Message}",
                    ex,
                    SourceId,
                    contextDocumentId);
            }
        }
    }
}
