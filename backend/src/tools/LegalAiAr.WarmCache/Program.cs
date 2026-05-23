// Warm Download Cache: reads all rulings from SQL, copies PDFs from kb blobs into
// _cache/, and downloads CSJN API JSON responses into _cache/, so subsequent pipeline
// runs with UseCache=true skip all external HTTP downloads.
// Usage: dotnet run [--dry-run] [--source <id>] [--take <n>] [--parallelism <n>] [--skip-pdf] [--skip-api] [--throttle-ms <ms>]
// Env: AzureSql__ConnectionString, AzureBlob__ConnectionString, AzureBlob__ContainerName

using System.Net;
using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Data.SqlClient;

LoadEnvFile();

var knownFlags = new HashSet<string> { "--dry-run", "--source", "--parallelism", "--skip-pdf", "--skip-api", "--throttle-ms", "--take", "--report" };
var unknownFlags = args.Where(a => a.StartsWith("--") && !knownFlags.Contains(a)).ToList();
if (unknownFlags.Count > 0)
{
    Console.WriteLine($"Unknown flag(s): {string.Join(", ", unknownFlags)}");
    ShowUsage();
    return 1;
}

var dryRun = args.Contains("--dry-run");
var skipPdf = args.Contains("--skip-pdf");
var skipApi = args.Contains("--skip-api");
var sourceFilter = GetArgValue(args, "--source");
var parallelism = int.TryParse(GetArgValue(args, "--parallelism"), out var p) ? p : 8;
var throttleMs = int.TryParse(GetArgValue(args, "--throttle-ms"), out var t) ? t : 500;
var take = int.TryParse(GetArgValue(args, "--take"), out var tk) ? tk : (int?)null;

if (dryRun)
    Console.WriteLine("[DRY-RUN] No blobs will be written, no API calls will be made.\n");

// --- Config ---
var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var blobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
var containerName = Environment.GetEnvironmentVariable("AzureBlob__ContainerName") ?? "rulings-pdfs";

if (string.IsNullOrWhiteSpace(blobConn) || IsPlaceholder(blobConn))
{
    Console.WriteLine("ERROR: AzureBlob__ConnectionString is not configured.");
    return 1;
}

// --- Report mode ---
if (args.Contains("--report"))
{
    await ReportCacheCountsAsync(blobConn, containerName);
    return 0;
}

if (string.IsNullOrWhiteSpace(sqlConn) || IsPlaceholder(sqlConn))
{
    Console.WriteLine("ERROR: AzureSql__ConnectionString is not configured.");
    return 1;
}

// --- Read rulings ---
Console.WriteLine("Reading rulings from database...");
var rulings = await ReadRulingsAsync(sqlConn, sourceFilter);
if (take.HasValue)
{
    rulings = rulings.Take(take.Value).ToList();
    Console.WriteLine($"Found {rulings.Count} ruling(s) with BlobPath (limited to first {take.Value}).");
}
else
{
    Console.WriteLine($"Found {rulings.Count} ruling(s) with BlobPath.");
}

if (rulings.Count == 0)
{
    Console.WriteLine("Nothing to do.");
    return 0;
}

var blobServiceClient = new BlobServiceClient(blobConn);
var container = blobServiceClient.GetBlobContainerClient(containerName);
if (!await container.ExistsAsync())
{
    Console.WriteLine($"ERROR: Container '{containerName}' does not exist.");
    return 1;
}

// =====================
// Phase 1: PDF cache
// =====================
var pdfStats = new WarmStats();
if (!skipPdf)
{
    Console.WriteLine($"\n=== Phase 1: PDF cache (parallelism={parallelism}) ===");
    pdfStats = await WarmPdfCacheAsync(container, rulings, parallelism, dryRun);
    Console.WriteLine($"\n--- PDF Summary ---");
    Console.WriteLine($"  Already cached: {pdfStats.AlreadyCached}");
    Console.WriteLine($"  Copied:         {pdfStats.Copied}");
    Console.WriteLine($"  Source missing: {pdfStats.SourceMissing}");
    Console.WriteLine($"  Errors:         {pdfStats.Errors}");
}
else
{
    Console.WriteLine("\n=== Phase 1: PDF cache SKIPPED (--skip-pdf) ===");
}

// =====================
// Phase 2: API cache
// =====================
var apiStats = new WarmStats();
if (!skipApi)
{
    var csjnRulings = rulings.Where(r => r.SourceId == 1 && !string.IsNullOrEmpty(r.AnalysisId)).ToList();
    Console.WriteLine($"\n=== Phase 2: API cache ({csjnRulings.Count} CSJN rulings with AnalysisId, throttle={throttleMs}ms) ===");

    if (csjnRulings.Count > 0)
    {
        apiStats = await WarmApiCacheAsync(container, csjnRulings, throttleMs, dryRun);
        Console.WriteLine($"\n--- API Summary ---");
        Console.WriteLine($"  Already cached: {apiStats.AlreadyCached}");
        Console.WriteLine($"  Downloaded:     {apiStats.Copied}");
        Console.WriteLine($"  Errors:         {apiStats.Errors}");
    }
    else
    {
        Console.WriteLine("  No CSJN rulings with AnalysisId found.");
    }
}
else
{
    Console.WriteLine("\n=== Phase 2: API cache SKIPPED (--skip-api) ===");
}

// =====================
// Final summary
// =====================
Console.WriteLine($"\n=== Final Summary ===");
Console.WriteLine($"Total rulings:     {rulings.Count}");
Console.WriteLine($"PDF cached/copied: {pdfStats.AlreadyCached}/{pdfStats.Copied}  (missing={pdfStats.SourceMissing}, errors={pdfStats.Errors})");
Console.WriteLine($"API cached/fetched:{apiStats.AlreadyCached}/{apiStats.Copied}  (errors={apiStats.Errors})");

var totalErrors = pdfStats.Errors + apiStats.Errors;
return totalErrors > 0 ? 1 : 0;

// ============================================================
// Functions
// ============================================================

static void LoadEnvFile()
{
    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir != null)
    {
        var envPath = Path.Combine(dir.FullName, ".env");
        if (File.Exists(envPath))
        {
            var count = 0;
            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
                var eq = trimmed.IndexOf('=');
                if (eq <= 0) continue;

                var name = trimmed[..eq].Trim();
                var value = trimmed[(eq + 1)..].Trim().Trim('"', '\'');

                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(name)))
                {
                    Environment.SetEnvironmentVariable(name, value);
                    count++;
                }
            }
            Console.WriteLine($"Loaded {count} variable(s) from {envPath}");
            return;
        }
        dir = dir.Parent;
    }
}

static async Task ReportCacheCountsAsync(string blobConnectionString, string containerName)
{
    var blobService = new BlobServiceClient(blobConnectionString);
    var container = blobService.GetBlobContainerClient(containerName);

    if (!await container.ExistsAsync())
    {
        Console.WriteLine($"Container '{containerName}' does not exist.");
        return;
    }

    var prefixes = new[]
    {
        ("PDF",                "_cache/csjn/pdf/"),
        ("abrirAnalisis",      "_cache/csjn/api/abrirAnalisis/"),
        ("getAllDocumentos",    "_cache/csjn/api/getAllDocumentos/"),
        ("getSumariosAnalisis", "_cache/csjn/api/getSumariosAnalisis/"),
        ("getCitas",           "_cache/csjn/api/getCitas/"),
        ("getCitantes",        "_cache/csjn/api/getCitantes/"),
    };

    Console.WriteLine($"Cache report for container '{containerName}':\n");
    var results = await Task.WhenAll(prefixes.Select(async pair =>
    {
        var (label, prefix) = pair;
        var count = 0;
        await foreach (var _ in container.GetBlobsAsync(prefix: prefix))
            count++;
        return (label, count);
    }));

    var total = 0;
    foreach (var (label, count) in results)
    {
        total += count;
        Console.WriteLine($"  {label,-25} {count,6} blobs");
    }
    Console.WriteLine($"  {"TOTAL",-25} {total,6} blobs");
}

static void ShowUsage()
{
    Console.WriteLine("WarmCache: populates the _cache/ download cache from existing kb PDFs and CSJN API.");
    Console.WriteLine("Usage: dotnet run [--dry-run] [--source <id>] [--parallelism <n>] [--skip-pdf] [--skip-api] [--throttle-ms <ms>]");
    Console.WriteLine("  --dry-run           Report what would be cached without writing or fetching");
    Console.WriteLine("  --report            Show blob counts per cache folder and exit");
    Console.WriteLine("  --source <id>       Filter by SourceId (1=csjn, 2=saij, 3=pjn, 4=scba)");
    Console.WriteLine("  --parallelism <n>   Max concurrent blob operations for PDF phase (default: 8)");
    Console.WriteLine("  --skip-pdf          Skip Phase 1 (PDF blob copy)");
    Console.WriteLine("  --skip-api          Skip Phase 2 (CSJN API download)");
    Console.WriteLine("  --take <n>          Process only the first N rulings");
    Console.WriteLine("  --throttle-ms <ms>  Delay between API requests in ms (default: 500)");
    Console.WriteLine("\nEnv: AzureSql__ConnectionString, AzureBlob__ConnectionString, AzureBlob__ContainerName");
}

static string? GetArgValue(string[] args, string flag)
{
    var idx = Array.IndexOf(args, flag);
    return idx >= 0 && idx + 1 < args.Length ? args[idx + 1] : null;
}

static bool IsPlaceholder(string? v) =>
    string.IsNullOrWhiteSpace(v) ||
    v is "DB_SECRET" or "STORAGE_KEY" or "SEARCH_KEY" or "OPENAI_KEY";

static string ResolveSourceName(int sourceId) => sourceId switch
{
    1 => "csjn",
    2 => "saij",
    3 => "pjn",
    4 => "scba",
    _ => "unknown"
};

static string PdfCacheKey(int sourceId, string documentId)
    => $"_cache/{ResolveSourceName(sourceId)}/pdf/{documentId}.pdf";

static string ApiCacheKey(int sourceId, string endpoint, string id)
    => $"_cache/{ResolveSourceName(sourceId)}/api/{endpoint}/{id}.json";

// --- SQL ---

static async Task<List<RulingInfo>> ReadRulingsAsync(string connectionString, string? sourceFilter)
{
    var rulings = new List<RulingInfo>();
    await using var conn = new SqlConnection(connectionString);
    await conn.OpenAsync();

    var sql = "SELECT [SourceId], [ExternalId], [AnalysisId], [BlobPath] FROM [Rulings] WHERE [BlobPath] IS NOT NULL";
    if (int.TryParse(sourceFilter, out var sid))
        sql += $" AND [SourceId] = {sid}";
    sql += " ORDER BY [SourceId], [ExternalId]";

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = sql;
    await using var reader = await cmd.ExecuteReaderAsync();

    while (await reader.ReadAsync())
    {
        rulings.Add(new RulingInfo(
            reader.GetInt32(0),
            reader.GetString(1),
            reader.IsDBNull(2) ? null : reader.GetString(2),
            reader.GetString(3)));
    }
    return rulings;
}

// --- Phase 1: PDF ---

static async Task<WarmStats> WarmPdfCacheAsync(
    BlobContainerClient container,
    List<RulingInfo> rulings,
    int parallelism,
    bool dryRun)
{
    var stats = new WarmStats();
    var semaphore = new SemaphoreSlim(parallelism);
    var lockObj = new object();
    var processed = 0;

    var tasks = rulings.Select(async ruling =>
    {
        await semaphore.WaitAsync();
        try
        {
            var cacheKey = PdfCacheKey(ruling.SourceId, ruling.ExternalId);
            var cacheBlob = container.GetBlobClient(cacheKey);

            if (await cacheBlob.ExistsAsync())
            {
                Interlocked.Increment(ref stats.AlreadyCached);
            }
            else
            {
                var sourceBlob = container.GetBlobClient(ruling.BlobPath);
                if (!await sourceBlob.ExistsAsync())
                {
                    Interlocked.Increment(ref stats.SourceMissing);
                    lock (lockObj)
                        Console.WriteLine($"  PDF MISSING: {ruling.BlobPath}");
                    return;
                }

                if (!dryRun)
                {
                    var download = await sourceBlob.DownloadContentAsync();
                    var bytes = download.Value.Content.ToArray();

                    using var stream = new MemoryStream(bytes);
                    await cacheBlob.UploadAsync(
                        stream,
                        new BlobHttpHeaders { ContentType = "application/octet-stream" });
                }
                Interlocked.Increment(ref stats.Copied);
            }
        }
        catch (Exception ex)
        {
            Interlocked.Increment(ref stats.Errors);
            lock (lockObj)
                Console.WriteLine($"  PDF ERROR [{ruling.SourceId}/{ruling.ExternalId}]: {ex.Message}");
        }
        finally
        {
            semaphore.Release();
            var current = Interlocked.Increment(ref processed);
            if (current % 25 == 0 || current == rulings.Count)
            {
                var pct = (int)(current * 100.0 / rulings.Count);
                lock (lockObj)
                    Console.WriteLine($"  PDF: {current} de {rulings.Count} ({pct}%) " +
                        $"— cached={stats.AlreadyCached}, copied={stats.Copied}, " +
                        $"missing={stats.SourceMissing}, errors={stats.Errors}");
            }
        }
    });

    await Task.WhenAll(tasks);
    return stats;
}

// --- Phase 2: API ---

static async Task<WarmStats> WarmApiCacheAsync(
    BlobContainerClient container,
    List<RulingInfo> rulings,
    int throttleMs,
    bool dryRun)
{
    const string csjnBaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta";
    const int sourceId = 1;
    const int checkParallelism = 50;

    var endpoints = new ApiEndpointDef[]
    {
        new("abrirAnalisis",        $"{csjnBaseUrl}/fallos/abrirAnalisis.html",        "idAnalisis",  IdKind.AnalysisId),
        new("getAllDocumentos",      $"{csjnBaseUrl}/documentos/getAllDocumentos.html",  "idAnalisis",  IdKind.AnalysisId),
        new("getSumariosAnalisis",   $"{csjnBaseUrl}/sumarios/getSumariosAnalisis.html", "idAnalisis",  IdKind.AnalysisId),
        new("getCitas",             $"{csjnBaseUrl}/documentos/getCitas.html",          "idDocumento", IdKind.DocumentId),
        new("getCitantes",          $"{csjnBaseUrl}/documentos/getCitantes.html",       "idDocumento", IdKind.DocumentId),
    };

    var stats = new WarmStats();
    var deduplicatedCalls = BuildDeduplicatedApiCalls(rulings, endpoints, sourceId);
    Console.WriteLine($"  {deduplicatedCalls.Count} unique API cache entries to check ({rulings.Count} rulings x {endpoints.Length} endpoints, deduplicated by AnalysisId overlap).");

    // Step 1: parallel ExistsAsync scan
    Console.WriteLine($"  Scanning cache (parallelism={checkParallelism})...");
    var missing = new System.Collections.Concurrent.ConcurrentBag<ApiCall>();
    var checkedCount = 0;
    var semaphore = new SemaphoreSlim(checkParallelism);
    var endpointMissing = new System.Collections.Concurrent.ConcurrentDictionary<string, int>();
    foreach (var ep in endpoints) endpointMissing[ep.Name] = 0;

    var checkTasks = deduplicatedCalls.Select(async call =>
    {
        await semaphore.WaitAsync();
        try
        {
            var cacheBlob = container.GetBlobClient(call.CacheKey);
            if (await cacheBlob.ExistsAsync())
            {
                Interlocked.Increment(ref stats.AlreadyCached);
            }
            else
            {
                missing.Add(call);
                endpointMissing.AddOrUpdate(call.EndpointName, 1, (_, v) => v + 1);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  CHECK ERROR [{call.EndpointName}/{call.IdValue}]: {ex.Message}");
            missing.Add(call);
        }
        finally
        {
            semaphore.Release();
            var c = Interlocked.Increment(ref checkedCount);
            if (c % 500 == 0 || c == deduplicatedCalls.Count)
            {
                var pct = (int)(c * 100.0 / deduplicatedCalls.Count);
                Console.WriteLine($"  Scan: {c}/{deduplicatedCalls.Count} ({pct}%) — cached={stats.AlreadyCached}, missing={missing.Count}");
            }
        }
    });
    await Task.WhenAll(checkTasks);

    Console.WriteLine($"\n  Scan complete: {stats.AlreadyCached} cached, {missing.Count} missing.");
    foreach (var kv in endpointMissing.OrderBy(k => k.Key))
        Console.WriteLine($"    {kv.Key,-25} {kv.Value,6} missing");

    if (missing.IsEmpty)
    {
        Console.WriteLine("  All API responses are cached. Nothing to download.");
        return stats;
    }

    if (dryRun)
    {
        stats.Copied = missing.Count;
        Console.WriteLine($"  [DRY-RUN] Would download {missing.Count} API responses.");
        return stats;
    }

    // Step 2: sequential fetch for missing entries (throttled)
    Console.WriteLine($"\n  Downloading {missing.Count} missing API responses (throttle={throttleMs}ms)...");
    using var http = new HttpClient();
    http.Timeout = TimeSpan.FromSeconds(30);
    http.DefaultRequestHeaders.Add("User-Agent", "LegalAiAr-WarmCache/1.0");

    var downloaded = 0;
    var downloadErrors = 0;
    var missingList = missing.ToList();
    foreach (var call in missingList)
    {
        try
        {
            var url = $"{call.BaseUrl}?{call.QueryParam}={Uri.EscapeDataString(call.IdValue)}";

            var response = await FetchWithRetryAsync(http, url, throttleMs);
            var content = await response.Content.ReadAsStringAsync();

            var payload = string.IsNullOrWhiteSpace(content) ? "\"\"" : content;
            var bytes = Encoding.UTF8.GetBytes(payload);
            using var stream = new MemoryStream(bytes);
            var cacheBlob = container.GetBlobClient(call.CacheKey);
            await cacheBlob.UploadAsync(
                stream,
                new BlobHttpHeaders { ContentType = "application/json" });

            downloaded++;
            await Task.Delay(throttleMs);
        }
        catch (Exception ex)
        {
            downloadErrors++;
            Console.WriteLine($"  API ERROR [{call.EndpointName}/{call.IdValue}]: {ex.Message}");
        }

        var total = downloaded + downloadErrors;
        if (total % 25 == 0 || total == missingList.Count)
        {
            var pct = (int)(total * 100.0 / missingList.Count);
            Console.WriteLine($"  Download: {total}/{missingList.Count} ({pct}%) — ok={downloaded}, errors={downloadErrors}");
        }
    }

    stats.Copied = downloaded;
    stats.Errors = downloadErrors;
    return stats;
}

static List<ApiCall> BuildDeduplicatedApiCalls(
    List<RulingInfo> rulings,
    ApiEndpointDef[] endpoints,
    int sourceId)
{
    var seen = new HashSet<string>();
    var calls = new List<ApiCall>();

    foreach (var ruling in rulings)
    {
        foreach (var ep in endpoints)
        {
            var idValue = ep.Kind == IdKind.AnalysisId ? ruling.AnalysisId! : ruling.ExternalId;
            var cacheKey = ApiCacheKey(sourceId, ep.Name, idValue);

            if (seen.Add(cacheKey))
            {
                calls.Add(new ApiCall(ep.Name, ep.Url, ep.QueryParam, idValue, cacheKey));
            }
        }
    }
    return calls;
}

static async Task<HttpResponseMessage> FetchWithRetryAsync(HttpClient http, string url, int throttleMs)
{
    var maxRetries = 5;
    var backoffBase = 2.0;

    for (var attempt = 0; attempt <= maxRetries; attempt++)
    {
        var response = await http.GetAsync(url);

        if (response.StatusCode == HttpStatusCode.TooManyRequests)
        {
            if (attempt == maxRetries) response.EnsureSuccessStatusCode();
            var delay = (int)(throttleMs * Math.Pow(backoffBase, attempt + 1));
            Console.WriteLine($"  429 throttled, waiting {delay}ms...");
            await Task.Delay(delay);
            continue;
        }

        if (response.StatusCode is HttpStatusCode.BadGateway or HttpStatusCode.ServiceUnavailable or HttpStatusCode.GatewayTimeout)
        {
            if (attempt == maxRetries) response.EnsureSuccessStatusCode();
            var delays = new[] { 5, 15, 30, 60, 120 };
            var delaySec = delays[Math.Min(attempt, delays.Length - 1)];
            Console.WriteLine($"  {(int)response.StatusCode} transient error, retrying in {delaySec}s...");
            await Task.Delay(TimeSpan.FromSeconds(delaySec));
            continue;
        }

        response.EnsureSuccessStatusCode();
        return response;
    }

    throw new InvalidOperationException($"Exhausted retries for {url}");
}

// --- Types ---

file record RulingInfo(int SourceId, string ExternalId, string? AnalysisId, string BlobPath);

file record ApiEndpointDef(string Name, string Url, string QueryParam, IdKind Kind);

file record ApiCall(string EndpointName, string BaseUrl, string QueryParam, string IdValue, string CacheKey);

file enum IdKind { AnalysisId, DocumentId }

file class WarmStats
{
    public int AlreadyCached;
    public int Copied;
    public int SourceMissing;
    public int Errors;
}
