// BulkRequeue: publishes pipeline messages to the queue for all rulings in the DB.
// Supports parser, enrichment and indexer stages.
// For --stage parser: sends ParserMessage (triggers CSJN API + enrichment + indexer).
// For --stage indexer: sends IndexerMessage with ForceReindex (re-embed + re-index only).
//
// Usage: dotnet run [--limit <N>] [--batch-size <N>] [--dry-run] [--stage <stage>] [--use-cache] [--max-retries <N>]
// Env:   AzureSql__ConnectionString, AzureBlob__ConnectionString

using System.Text.Json;
using Azure.Storage.Queues;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using Microsoft.Data.SqlClient;
using SharpToken;

LoadEnvFile();

var pipelineQueuePrefix = Environment.GetEnvironmentVariable("Pipeline__QueuePrefix") ?? "pipeline";
var queueNames = new PipelineQueueNames(pipelineQueuePrefix);

var knownFlags = new HashSet<string> { "--dry-run", "--limit", "--batch-size", "--stage", "--use-cache", "--max-retries", "--replay-dlq", "--only-pending", "--clear-queues", "--from-csv", "--job-id" };
var unknownFlags = args.Where(a => a.StartsWith("--") && !knownFlags.Contains(a)).ToList();
if (unknownFlags.Count > 0)
{
    Console.WriteLine($"Unknown flag(s): {string.Join(", ", unknownFlags)}");
    ShowUsage();
    return 1;
}

var dryRun = args.Contains("--dry-run");
var useCache = args.Contains("--use-cache");
var onlyPending = args.Contains("--only-pending");

if (args.Contains("--clear-queues"))
{
    return await ClearAllQueuesAsync();
}

if (args.Contains("--replay-dlq"))
{
    return await ReplayDlqAsync();
}

var csvPath = GetArgValue(args, "--from-csv");
if (csvPath != null)
{
    return await RequeueFromCsvAsync(csvPath);
}
var limit = int.TryParse(GetArgValue(args, "--limit"), out var lim) ? lim : 0;
var jobIdArg = GetArgValue(args, "--job-id");
var jobIdFilter = Guid.TryParse(jobIdArg, out var parsedJobId) ? parsedJobId : (Guid?)null;
var batchSize = int.TryParse(GetArgValue(args, "--batch-size"), out var bs) ? bs : 200;
var maxRetries = int.TryParse(GetArgValue(args, "--max-retries"), out var mr) ? mr : 10;
var stage = GetArgValue(args, "--stage") ?? "indexer";

Console.WriteLine("BulkRequeue — Publish pipeline messages for all rulings");
Console.WriteLine($"  Stage:      {stage}");
Console.WriteLine($"  Dry run:    {dryRun}");
Console.WriteLine($"  Use cache:  {useCache}");
Console.WriteLine($"  Limit:      {(limit > 0 ? limit.ToString() : "all")}");
Console.WriteLine($"  Job filter: {(jobIdFilter.HasValue ? jobIdFilter.Value.ToString() : "(none)")}");
Console.WriteLine($"  Batch size: {batchSize}");
Console.WriteLine($"  Max retries:{maxRetries}");
Console.WriteLine();

var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var storageConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
    ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");

if (string.IsNullOrWhiteSpace(sqlConn) || sqlConn.Contains("PLACEHOLDER"))
{
    Console.WriteLine("ERROR: SQL connection string is not configured.");
    return 1;
}
if (string.IsNullOrWhiteSpace(storageConn))
{
    Console.WriteLine("ERROR: AzureBlob__ConnectionString is required for queue publishing.");
    return 1;
}

if (stage == "persister")
{
    return await RequeuePersisterAsync(limit, jobIdFilter);
}

var queueName = stage switch
{
    "parser" => queueNames.Parser,
    "enrichment" => queueNames.Enricher,
    "indexer" => queueNames.Indexer,
    _ => throw new ArgumentException($"Unknown stage: {stage}")
};

var queueClient = new QueueClient(storageConn, queueName);
await queueClient.CreateIfNotExistsAsync();

var jsonOptions = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    WriteIndented = false
};

var encoding = GptEncoding.GetEncoding("cl100k_base");

Console.WriteLine("Loading rulings from DB...");
var rulings = await LoadRulingsAsync(sqlConn, limit, onlyPending);
Console.WriteLine($"Loaded {rulings.Count} rulings.");

if (rulings.Count == 0)
{
    Console.WriteLine("Nothing to process.");
    return 0;
}

Dictionary<Guid, List<JudgeData>> judgeMap = [];
Dictionary<Guid, List<KeywordData>> keywordMap = [];
Dictionary<Guid, List<StatuteData>> statuteMap = [];
Dictionary<Guid, List<CitationData>> citationMap = [];

if (stage != "parser")
{
    Console.WriteLine("Loading related data (persons, keywords, statutes, citations)...");
    await using var conn = new SqlConnection(sqlConn);
    await conn.OpenAsync();

    var allIds = rulings.Select(r => r.Id).ToList();
    judgeMap = await LoadPersonsAsync(conn, allIds);
    keywordMap = await LoadKeywordsAsync(conn, allIds);
    statuteMap = await LoadStatutesAsync(conn, allIds);
    citationMap = await LoadCitationsAsync(conn, allIds);
    Console.WriteLine($"  Persons:   {judgeMap.Values.Sum(v => v.Count)} entries");
    Console.WriteLine($"  Keywords:  {keywordMap.Values.Sum(v => v.Count)} entries");
    Console.WriteLine($"  Statutes:  {statuteMap.Values.Sum(v => v.Count)} entries");
    Console.WriteLine($"  Citations: {citationMap.Values.Sum(v => v.Count)} entries");
}
else
{
    Console.WriteLine("Parser stage: skipping related data load (parser will fetch from CSJN API).");
}
Console.WriteLine();

var published = 0;
var skipped = 0;
var errors = 0;
var sw = System.Diagnostics.Stopwatch.StartNew();

for (var i = 0; i < rulings.Count; i += batchSize)
{
    var batch = rulings.Skip(i).Take(batchSize).ToList();
    var progress = $"[{Math.Min(i + batchSize, rulings.Count)}/{rulings.Count}]";

    foreach (var r in batch)
    {
        if (string.IsNullOrEmpty(r.BlobPath))
        {
            skipped++;
            continue;
        }

        try
        {
            string json;
            if (stage == "parser")
            {
                if (string.IsNullOrEmpty(r.AnalysisId))
                {
                    skipped++;
                    continue;
                }
                var message = new ParserMessage(
                    SourceId: r.SourceId,
                    DocumentId: r.ExternalId,
                    AnalysisId: r.AnalysisId,
                    BlobPathPdf: r.BlobPath,
                    ContentHash: r.ContentHash,
                    ApiMetadata: null,
                    UseCache: useCache,
                    Reprocess: true,
                    RulingDateHint: r.RulingDate,
                    CaseNumberHint: r.CaseNumber);
                json = JsonSerializer.Serialize(message, jsonOptions);
            }
            else
            {
                var textBlobPath = ToTextPath(r.BlobPath);
                var judges = judgeMap.TryGetValue(r.Id, out var jList) ? jList : [];
                var keywords = keywordMap.TryGetValue(r.Id, out var kList) ? kList : [];
                var statutes = statuteMap.TryGetValue(r.Id, out var sList) ? sList : [];
                var citations = citationMap.TryGetValue(r.Id, out var cList) ? cList : [];

                var message = new IndexerMessage(
                    DocumentId: r.ExternalId,
                    ContentHash: r.ContentHash,
                    SourceId: r.SourceId,
                    Ruling: new RulingData(
                        CaseTitle: r.CaseTitle,
                        RulingDate: r.RulingDate,
                        CaseNumber: r.CaseNumber,
                        JurisdictionArea: r.JurisdictionArea,
                        Instance: r.Instance,
                        Jurisdiction: r.Jurisdiction,
                        ResourceType: r.ResourceType,
                        RulingDirection: r.RulingDirection,
                        SubjectArea: r.SubjectArea,
                        IsUnconstitutional: r.IsUnconstitutional,
                        Summary: r.Summary,
                        Holding: r.Holding,
                        FullText: "",
                        BlobPath: r.BlobPath,
                        Court: r.CourtName),
                    Judges: judges,
                    Keywords: keywords,
                    Statutes: statutes,
                    Citations: citations,
                    Chunks: [],
                    TextBlobPath: textBlobPath,
                    AnalysisId: r.AnalysisId,
                    ForceReindex: true);
                json = JsonSerializer.Serialize(message, jsonOptions);
            }

            if (dryRun)
            {
                published++;
            }
            else
            {
                await SendWithRetryAsync(queueClient, json, r.ExternalId, maxRetries);
                published++;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  ERROR {r.ExternalId}: {ex.Message}");
            errors++;
        }
    }

    var elapsed = sw.Elapsed;
    var rate = published > 0 ? elapsed.TotalSeconds / published : 0;
    var remaining = published > 0
        ? TimeSpan.FromSeconds(rate * (rulings.Count - i - batch.Count))
        : TimeSpan.Zero;
    Console.WriteLine($"  {progress} published={published} skipped={skipped} errors={errors}  " +
                      $"elapsed={elapsed:hh\\:mm\\:ss} remaining~{remaining:hh\\:mm\\:ss}");
}

Console.WriteLine();
Console.WriteLine($"Done. Published={published} Skipped={skipped} Errors={errors} Time={sw.Elapsed:hh\\:mm\\:ss}");
return errors > 0 ? 1 : 0;

// ── DB queries ──────────────────────────────────────────────

async Task<List<RulingRow>> LoadRulingsAsync(string connStr, int maxRows, bool pendingOnly = false)
{
    var results = new List<RulingRow>();
    await using var c = new SqlConnection(connStr);
    await c.OpenAsync();
    await using var cmd = c.CreateCommand();
    var whereClause = pendingOnly
        ? "WHERE r.SourceId = 1 AND r.IndexedAt < '2026-04-29'"
        : "WHERE r.SourceId = 1";
    cmd.CommandText = $"""
        SELECT {(maxRows > 0 ? $"TOP {maxRows}" : "")}
               r.Id, r.ExternalId, r.ContentHash, r.SourceId, r.BlobPath,
               r.CaseTitle, r.RulingDate, r.CaseNumber,
               r.JurisdictionArea, r.Instance, r.Jurisdiction,
               r.ResourceType, r.RulingDirection, r.SubjectArea, r.IsUnconstitutional,
               r.Summary, r.Holding, c.Name AS CourtName, r.AnalysisId
        FROM Rulings r
        LEFT JOIN Courts c ON r.CourtId = c.Id
        {whereClause}
        ORDER BY r.RulingDate DESC
        """;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        results.Add(new RulingRow
        {
            Id = reader.GetGuid(0),
            ExternalId = reader.GetString(1),
            ContentHash = reader.IsDBNull(2) ? "" : reader.GetString(2),
            SourceId = reader.GetInt32(3),
            BlobPath = reader.IsDBNull(4) ? null : reader.GetString(4),
            CaseTitle = reader.IsDBNull(5) ? "" : reader.GetString(5),
            RulingDate = DateOnly.FromDateTime(reader.GetDateTime(6)),
            CaseNumber = reader.IsDBNull(7) ? null : reader.GetString(7),
            JurisdictionArea = reader.IsDBNull(8) ? null : reader.GetString(8),
            Instance = reader.IsDBNull(9) ? null : reader.GetString(9),
            Jurisdiction = reader.IsDBNull(10) ? null : reader.GetString(10),
            ResourceType = reader.IsDBNull(11) ? null : reader.GetString(11),
            RulingDirection = reader.IsDBNull(12) ? null : reader.GetString(12),
            SubjectArea = reader.IsDBNull(13) ? null : reader.GetString(13),
            IsUnconstitutional = reader.GetBoolean(14),
            Summary = reader.IsDBNull(15) ? null : reader.GetString(15),
            Holding = reader.IsDBNull(16) ? null : reader.GetString(16),
            CourtName = reader.IsDBNull(17) ? null : reader.GetString(17),
            AnalysisId = reader.IsDBNull(18) ? null : reader.GetString(18)
        });
    }
    return results;
}

async Task<Dictionary<Guid, List<JudgeData>>> LoadPersonsAsync(SqlConnection c, List<Guid> ids)
{
    var result = new Dictionary<Guid, List<JudgeData>>();
    if (ids.Count == 0) return result;

    await using var cmd = c.CreateCommand();
    cmd.CommandText = """
        SELECT rp.RulingId, p.FirstName, p.LastName, rp.ParticipationType
        FROM RulingParticipations rp
        JOIN Persons p ON rp.PersonId = p.Id
        WHERE rp.RulingId IN (SELECT value FROM OPENJSON(@ids))
        ORDER BY p.LastName
        """;
    cmd.Parameters.AddWithValue("@ids", JsonSerializer.Serialize(ids.Select(id => id.ToString())));
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var firstName = reader.IsDBNull(1) ? "" : reader.GetString(1);
        var lastName = reader.IsDBNull(2) ? "" : reader.GetString(2);
        var participation = reader.IsDBNull(3) ? "SIGNATORY" : reader.GetString(3);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = [];
        result[rulingId].Add(new JudgeData(firstName, lastName, participation));
    }
    return result;
}

async Task<Dictionary<Guid, List<KeywordData>>> LoadKeywordsAsync(SqlConnection c, List<Guid> ids)
{
    var result = new Dictionary<Guid, List<KeywordData>>();
    if (ids.Count == 0) return result;

    await using var cmd = c.CreateCommand();
    cmd.CommandText = """
        SELECT rk.RulingId, k.ExternalCode, k.Description, rk.SortOrder
        FROM RulingKeywords rk
        JOIN Keywords k ON rk.KeywordId = k.Id
        WHERE rk.RulingId IN (SELECT value FROM OPENJSON(@ids))
        ORDER BY rk.SortOrder
        """;
    cmd.Parameters.AddWithValue("@ids", JsonSerializer.Serialize(ids.Select(id => id.ToString())));
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var externalCode = reader.IsDBNull(1) ? (int?)null : reader.GetInt32(1);
        var desc = reader.GetString(2);
        var sortOrder = reader.GetInt32(3);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = [];
        result[rulingId].Add(new KeywordData(externalCode, desc, sortOrder));
    }
    return result;
}

async Task<Dictionary<Guid, List<StatuteData>>> LoadStatutesAsync(SqlConnection c, List<Guid> ids)
{
    var result = new Dictionary<Guid, List<StatuteData>>();
    if (ids.Count == 0) return result;

    await using var cmd = c.CreateCommand();
    cmd.CommandText = """
        SELECT rs.RulingId, s.Number, s.Name, rs.Articles
        FROM RulingStatutes rs
        JOIN Statutes s ON rs.StatuteId = s.Id
        WHERE rs.RulingId IN (SELECT value FROM OPENJSON(@ids))
        """;
    cmd.Parameters.AddWithValue("@ids", JsonSerializer.Serialize(ids.Select(id => id.ToString())));
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var number = reader.GetString(1);
        var name = reader.GetString(2);
        var articles = reader.IsDBNull(3) ? null : reader.GetString(3);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = [];
        result[rulingId].Add(new StatuteData(number, name, articles));
    }
    return result;
}

async Task<Dictionary<Guid, List<CitationData>>> LoadCitationsAsync(SqlConnection c, List<Guid> ids)
{
    var result = new Dictionary<Guid, List<CitationData>>();
    if (ids.Count == 0) return result;

    await using var cmd = c.CreateCommand();
    cmd.CommandText = """
        SELECT c.SourceRulingId, c.ExternalAlias, c.CsjnSummaryId, c.CitationType
        FROM Citations c
        WHERE c.SourceRulingId IN (SELECT value FROM OPENJSON(@ids))
        """;
    cmd.Parameters.AddWithValue("@ids", JsonSerializer.Serialize(ids.Select(id => id.ToString())));
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var alias = reader.GetString(1);
        var summaryId = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
        var citationType = reader.IsDBNull(3) ? "CITES" : reader.GetString(3);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = [];
        result[rulingId].Add(new CitationData(alias, summaryId, citationType));
    }
    return result;
}

// ── Helpers ─────────────────────────────────────────────────

static string ToTextPath(string blobPath) =>
    blobPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
        ? blobPath[..^4] + ".txt"
        : blobPath + ".txt";

static async Task SendWithRetryAsync(QueueClient queue, string json, string context, int maxRetries)
{
    for (var attempt = 0; ; attempt++)
    {
        try
        {
            await queue.SendMessageAsync(json);
            return;
        }
        catch (Exception ex) when (attempt < maxRetries &&
            (ex is Azure.RequestFailedException or System.Net.Http.HttpRequestException or TaskCanceledException
             or System.Net.Sockets.SocketException))
        {
            var delay = Math.Min(5 * (int)Math.Pow(2, attempt), 120);
            Console.WriteLine($"  RETRY {context}: {ex.GetType().Name} — waiting {delay}s (attempt {attempt + 1}/{maxRetries})");
            await Task.Delay(TimeSpan.FromSeconds(delay));
        }
    }
}

static void ShowUsage()
{
    Console.WriteLine("""
        Usage: dotnet run [options]
          --stage <stage>     Pipeline stage: parser, enrichment, indexer (default: indexer)
          --limit <N>         Max rulings to process (default: all)
          --batch-size <N>    Rulings per progress report (default: 200)
          --max-retries <N>   Max retries on VPN/network failure (default: 10)
          --use-cache         Use CSJN API download cache (only relevant for parser stage)
          --dry-run           Count without publishing
          --replay-dlq        Move all DLQ messages back to their origin queues
          --job-id <guid>     With --stage persister: filter documents to this ingestion job
        """);
}

async Task<int> RequeueFromCsvAsync(string csvFile)
{
    Console.WriteLine($"RequeueFromCSV — Publishing parser messages from {csvFile}\n");

    var storageConnStr = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
        ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
    if (string.IsNullOrWhiteSpace(storageConnStr)) { Console.WriteLine("ERROR: Storage connection string not configured."); return 1; }

    if (!File.Exists(csvFile)) { Console.WriteLine($"ERROR: File not found: {csvFile}"); return 1; }

    var parserQueue = new QueueClient(storageConnStr, queueNames.Parser);
    await parserQueue.CreateIfNotExistsAsync();

    var csvJsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };

    var lines = File.ReadAllLines(csvFile)
        .Where(l => !string.IsNullOrWhiteSpace(l) && !l.Contains("rows affected"))
        .ToList();

    Console.WriteLine($"  Found {lines.Count} entries in CSV");
    var published = 0;

    foreach (var line in lines)
    {
        var parts = line.Split(',');
        if (parts.Length < 4) { Console.WriteLine($"  SKIP (bad format): {line}"); continue; }

        var externalId = parts[0].Trim();
        var analysisId = parts[1].Trim();
        var contentHash = parts[2].Trim();
        var blobPath = parts[3].Trim();

        var msg = new ParserMessage(
            SourceId: 1,
            DocumentId: externalId,
            AnalysisId: analysisId,
            BlobPathPdf: blobPath,
            ContentHash: contentHash,
            ApiMetadata: null,
            UseCache: useCache,
            Reprocess: false);

        if (dryRun)
        {
            Console.WriteLine($"  [DRY] ExternalId={externalId}, AnalysisId={analysisId}");
        }
        else
        {
            var json = JsonSerializer.Serialize(msg, csvJsonOptions);
            await parserQueue.SendMessageAsync(json);
        }
        published++;
    }

    Console.WriteLine($"\nDone. Published={published} (dry-run={dryRun})");
    return 0;
}

async Task<int> ClearAllQueuesAsync()
{
    Console.WriteLine("ClearQueues — Clearing all pipeline queues\n");
    var storageConnStr = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
        ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
    if (string.IsNullOrWhiteSpace(storageConnStr)) { Console.WriteLine("ERROR: Storage connection string not configured."); return 1; }

    var allQueues = new[] { queueNames.Parser, queueNames.Enricher, queueNames.Indexer,
        queueNames.DlqFor("parser"), queueNames.DlqFor("enricher"), queueNames.DlqFor("indexer") };

    foreach (var q in allQueues)
    {
        var client = new QueueClient(storageConnStr, q);
        if (!await client.ExistsAsync()) { Console.WriteLine($"  {q}: does not exist"); continue; }
        var props = (await client.GetPropertiesAsync()).Value;
        Console.Write($"  {q}: ~{props.ApproximateMessagesCount} msgs... ");
        await client.ClearMessagesAsync();
        Console.WriteLine("cleared");
    }

    Console.WriteLine("\nDone.");
    return 0;
}

async Task<int> ReplayDlqAsync()
{
    Console.WriteLine("ReplayDLQ — Move all DLQ messages back to origin queues\n");

    var storageConnStr = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
        ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");
    if (string.IsNullOrWhiteSpace(storageConnStr))
    {
        Console.WriteLine("ERROR: Storage connection string not configured.");
        return 1;
    }

    var stages = new[] { "parser", "enrichment", "indexer" };
    var totalReplayed = 0;

    foreach (var s in stages)
    {
        var dlqName = queueNames.DlqFor(s);
        var originName = queueNames.QueueFor(s);

        var dlqClient = new QueueClient(storageConnStr, dlqName);
        var originClient = new QueueClient(storageConnStr, originName);
        await originClient.CreateIfNotExistsAsync();

        if (!await dlqClient.ExistsAsync())
        {
            Console.WriteLine($"  {dlqName}: queue does not exist, skipping");
            continue;
        }

        var props = (await dlqClient.GetPropertiesAsync()).Value;
        var approxCount = props.ApproximateMessagesCount;
        Console.WriteLine($"  {dlqName}: ~{approxCount} messages");

        var replayed = 0;
        while (true)
        {
            var msgs = await dlqClient.ReceiveMessagesAsync(maxMessages: 32, visibilityTimeout: TimeSpan.FromMinutes(5));
            if (msgs.Value.Length == 0) break;

            foreach (var msg in msgs.Value)
            {
                var body = msg.Body?.ToString() ?? "";
                var originalBody = ExtractOriginalBody(body);
                await originClient.SendMessageAsync(originalBody);
                await dlqClient.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
                replayed++;
            }

            Console.WriteLine($"    replayed {replayed} so far...");
        }

        Console.WriteLine($"  {dlqName}: {replayed} messages replayed to {originName}");
        totalReplayed += replayed;
    }

    Console.WriteLine($"\nDone. Total replayed: {totalReplayed}");
    return 0;

    static string ExtractOriginalBody(string body)
    {
        if (string.IsNullOrEmpty(body)) return body;
        try
        {
            using var doc = System.Text.Json.JsonDocument.Parse(body);
            if (doc.RootElement.TryGetProperty("originalMessage", out var orig))
                return orig.GetRawText();
        }
        catch (System.Text.Json.JsonException) { }
        return body;
    }
}

static string? GetArgValue(string[] arguments, string flag)
{
    var idx = Array.IndexOf(arguments, flag);
    return idx >= 0 && idx + 1 < arguments.Length ? arguments[idx + 1] : null;
}

static void LoadEnvFile()
{
    var dir = Directory.GetCurrentDirectory();
    while (dir is not null)
    {
        var envPath = Path.Combine(dir, ".env");
        if (File.Exists(envPath))
        {
            foreach (var line in File.ReadAllLines(envPath))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
                    continue;
                var eqIdx = trimmed.IndexOf('=');
                if (eqIdx <= 0)
                    continue;
                var key = trimmed[..eqIdx].Trim();
                var val = trimmed[(eqIdx + 1)..].Trim().Trim('"');
                if (string.IsNullOrEmpty(Environment.GetEnvironmentVariable(key)))
                    Environment.SetEnvironmentVariable(key, val);
            }
            return;
        }
        dir = Directory.GetParent(dir)?.FullName;
    }
}

async Task<int> RequeuePersisterAsync(int docLimit, Guid? ingestionJobId)
{
    Console.WriteLine("RequeuePersister — Re-queue failed/pending documents at Persister stage\n");

    var sqlConnStr = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    var storageConnStr = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString")
        ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString");

    if (string.IsNullOrWhiteSpace(sqlConnStr)) { Console.WriteLine("ERROR: SQL connection string not configured."); return 1; }
    if (string.IsNullOrWhiteSpace(storageConnStr)) { Console.WriteLine("ERROR: Storage connection string not configured."); return 1; }

    var persisterQueue = new QueueClient(storageConnStr, queueNames.Persister);
    await persisterQueue.CreateIfNotExistsAsync();

    await using var conn = new SqlConnection(sqlConnStr);
    await conn.OpenAsync();

    var partialMode = docLimit > 0;
    var docs = new List<PersisterDocRow>();
    await using (var cmd = conn.CreateCommand())
    {
        var topClause = partialMode ? $"TOP ({docLimit}) " : "";
        var statusClause = partialMode
            ? "d.Status = 'Failed'"
            : "d.Status IN ('Failed','Pending')";
        var jobClause = ingestionJobId.HasValue
            ? " AND d.IngestionJobId = @jobId"
            : "";

        cmd.CommandText = $"""
            SELECT {topClause}
                   d.Id, d.EntityType, d.SourceId, ISNULL(d.ContentHash,''), d.IngestionJobId, d.BlobPath
            FROM Documents d
            WHERE d.CurrentStage = 'Persister' AND {statusClause}{jobClause}
            ORDER BY d.Id
            """;
        if (ingestionJobId.HasValue)
            cmd.Parameters.AddWithValue("@jobId", ingestionJobId.Value);

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            docs.Add(new PersisterDocRow
            {
                Id = reader.GetGuid(0),
                EntityType = reader.GetString(1),
                SourceId = reader.GetInt32(2),
                ContentHash = reader.GetString(3),
                IngestionJobId = reader.IsDBNull(4) ? null : reader.GetGuid(4),
                PayloadBlobPath = reader.IsDBNull(5) ? null : reader.GetString(5)
            });
        }
    }

    Console.WriteLine($"  Found {docs.Count} documents to re-queue");
    if (docs.Count == 0) { Console.WriteLine("Nothing to process."); return 0; }

    if (!dryRun)
    {
        await using var resetCmd = conn.CreateCommand();
        var idList = string.Join(", ", docs.Select(d => $"'{d.Id}'"));
        resetCmd.CommandText = $"""
            UPDATE Documents
            SET Status = 'Pending', ErrorMessage = NULL, ErrorType = NULL, RetryCount = 0
            WHERE Id IN ({idList})
            """;
        var resetCount = await resetCmd.ExecuteNonQueryAsync();
        Console.WriteLine($"  Reset {resetCount} documents to Pending");
    }

    var published = 0;
    var errors = 0;
    var persisterJsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, WriteIndented = false };

    // Full requeue: clear stale queue/DLQ. Partial (--limit): leave queue intact so other jobs/tests are not disrupted.
    if (!dryRun && !partialMode)
    {
        var dlqClient = new QueueClient(storageConnStr, queueNames.PersisterDlq);
        await dlqClient.CreateIfNotExistsAsync();
        var persisterProps = (await persisterQueue.GetPropertiesAsync()).Value;
        var dlqProps = (await dlqClient.GetPropertiesAsync()).Value;
        if (persisterProps.ApproximateMessagesCount > 0)
        {
            Console.WriteLine($"  Clearing {persisterProps.ApproximateMessagesCount} leftover messages from persister queue...");
            await persisterQueue.ClearMessagesAsync();
        }
        if (dlqProps.ApproximateMessagesCount > 0)
        {
            Console.WriteLine($"  Clearing {dlqProps.ApproximateMessagesCount} leftover messages from persister DLQ...");
            await dlqClient.ClearMessagesAsync();
        }
    }
    else if (partialMode)
    {
        Console.WriteLine("  Partial mode: not clearing persister queue/DLQ.");
    }

    foreach (var doc in docs)
    {
        var entityTypeEnum = Enum.Parse<EntityType>(doc.EntityType, ignoreCase: true);

        var payloadPath = doc.PayloadBlobPath is not null
            ? (doc.PayloadBlobPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
                ? doc.PayloadBlobPath[..^4] + ".indexer.json"
                : doc.PayloadBlobPath + ".indexer.json")
            : null;

        var msg = new PersisterMessage(
            DocumentId: doc.Id,
            EntityType: entityTypeEnum,
            SourceId: doc.SourceId,
            ContentHash: doc.ContentHash,
            IngestionJobId: doc.IngestionJobId,
            PayloadBlobPath: payloadPath,
            Reprocess: true);

        if (dryRun)
        {
            Console.WriteLine($"  [DRY] DocId={doc.Id} EntityType={doc.EntityType}");
            published++;
        }
        else
        {
            try
            {
                var json = JsonSerializer.Serialize(msg, persisterJsonOptions);
                await persisterQueue.SendMessageAsync(json);
                published++;
                if (published % 10 == 0) Console.WriteLine($"  Published {published}...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ERROR DocId={doc.Id}: {ex.Message}");
                errors++;
            }
        }
    }

    Console.WriteLine($"\nDone. Published={published} Errors={errors} (dry-run={dryRun})");
    return errors > 0 ? 1 : 0;
}

// ── Message records (mirrors LegalAiAr.Core.Messages) ──────

record IndexerMessage(
    string DocumentId,
    string ContentHash,
    int SourceId,
    RulingData Ruling,
    IReadOnlyList<JudgeData> Judges,
    IReadOnlyList<KeywordData> Keywords,
    IReadOnlyList<StatuteData> Statutes,
    IReadOnlyList<CitationData> Citations,
    IReadOnlyList<ChunkData> Chunks,
    string? TextBlobPath = null,
    Guid? IngestionJobId = null,
    string? AnalysisId = null,
    IReadOnlyList<CitedByData>? CitedBy = null,
    bool ForceReindex = false);

record RulingData(
    string CaseTitle,
    DateOnly RulingDate,
    string? CaseNumber,
    string? JurisdictionArea,
    string? Instance,
    string? Jurisdiction,
    string? ResourceType,
    string? RulingDirection,
    string? SubjectArea,
    bool IsUnconstitutional,
    string? Summary,
    string? Holding,
    string FullText,
    string BlobPath,
    string? Court = null);

record JudgeData(string FirstName, string LastName, string ParticipationType);
record KeywordData(int? ExternalCode, string Description, int SortOrder);
record StatuteData(string Number, string Name, string? Articles);
record CitationData(string ExternalAlias, int? CsjnSummaryId, string CitationType);
record ChunkData(int Index, string Text);
record CitedByData(string AnalysisId, string CaseNumber);

record ParserMessage(
    int SourceId,
    string DocumentId,
    string? AnalysisId,
    string BlobPathPdf,
    string ContentHash,
    object? ApiMetadata,
    bool UseCache = false,
    bool Reprocess = false,
    DateOnly? RulingDateHint = null,
    string? CaseNumberHint = null);

record PersisterDocRow
{
    public Guid Id { get; init; }
    public string EntityType { get; init; } = "";
    public int SourceId { get; init; }
    public string ContentHash { get; init; } = "";
    public Guid? IngestionJobId { get; init; }
    public string? PayloadBlobPath { get; init; }
}

record RulingRow
{
    public Guid Id { get; init; }
    public string ExternalId { get; init; } = "";
    public string ContentHash { get; init; } = "";
    public int SourceId { get; init; }
    public string? BlobPath { get; init; }
    public string CaseTitle { get; init; } = "";
    public DateOnly RulingDate { get; init; }
    public string? CaseNumber { get; init; }
    public string? JurisdictionArea { get; init; }
    public string? Instance { get; init; }
    public string? Jurisdiction { get; init; }
    public string? ResourceType { get; init; }
    public string? RulingDirection { get; init; }
    public string? SubjectArea { get; init; }
    public bool IsUnconstitutional { get; init; }
    public string? Summary { get; init; }
    public string? Holding { get; init; }
    public string? CourtName { get; init; }
    public string? AnalysisId { get; init; }
}
