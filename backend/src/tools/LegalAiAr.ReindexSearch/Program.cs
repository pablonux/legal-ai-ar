// ReindexSearch: syncs Azure AI Search documents with current DB metadata.
// Uses MergeDocuments to update only metadata fields without touching embeddings.
//
// Usage: dotnet run [--dry-run] [--limit <N>] [--batch-size <N>]
// Env: AzureSql__ConnectionString, AzureSearch__Endpoint, AzureSearch__ApiKey, AzureSearch__RulingIndexName

using System.Text.Json;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Microsoft.Data.SqlClient;

LoadEnvFile();

var knownFlags = new HashSet<string> { "--dry-run", "--limit", "--batch-size", "--reconcile" };
var unknownFlags = args.Where(a => a.StartsWith("--") && !knownFlags.Contains(a)).ToList();
if (unknownFlags.Count > 0)
{
    Console.WriteLine($"Unknown flag(s): {string.Join(", ", unknownFlags)}");
    ShowUsage();
    return 1;
}

if (args.Contains("--reconcile"))
{
    return await ReconcileAsync();
}

var dryRun = args.Contains("--dry-run");
var limit = int.TryParse(GetArgValue(args, "--limit"), out var lim) ? lim : 0;
var batchSize = int.TryParse(GetArgValue(args, "--batch-size"), out var bs) ? bs : 100;

Console.WriteLine("ReindexSearch — Sync AI Search with DB metadata");
Console.WriteLine($"  Dry run:    {dryRun}");
Console.WriteLine($"  Limit:      {(limit > 0 ? limit.ToString() : "all")}");
Console.WriteLine($"  Batch size: {batchSize}");
Console.WriteLine();

var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
var searchEndpoint = Environment.GetEnvironmentVariable("AzureSearch__Endpoint");
var searchApiKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
var indexName = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";

if (string.IsNullOrWhiteSpace(sqlConn) || sqlConn.Contains("PLACEHOLDER"))
{
    Console.WriteLine("ERROR: SQL connection string is not configured.");
    return 1;
}
if (string.IsNullOrWhiteSpace(searchEndpoint) || string.IsNullOrWhiteSpace(searchApiKey))
{
    Console.WriteLine("ERROR: AzureSearch__Endpoint and AzureSearch__ApiKey are required.");
    return 1;
}

var searchClient = new SearchClient(
    new Uri(searchEndpoint),
    indexName,
    new AzureKeyCredential(searchApiKey));

var rulings = await LoadRulingsForReindexAsync(sqlConn, limit);
Console.WriteLine($"Loaded {rulings.Count} rulings from DB to sync.");
Console.WriteLine();

var updated = 0;
var errors = 0;

for (var i = 0; i < rulings.Count; i += batchSize)
{
    var batch = rulings.Skip(i).Take(batchSize).ToList();
    var progress = $"[{Math.Min(i + batchSize, rulings.Count)}/{rulings.Count}]";

    var documents = batch.Select(r =>
    {
        var doc = new Dictionary<string, object?>
        {
            ["id"] = $"ruling-{r.RulingId}",
            ["rulingId"] = r.RulingId.ToString(),
            ["caseTitle"] = r.CaseTitle,
            ["summary"] = r.Summary,
            ["holding"] = r.Holding,
            ["caseNumber"] = r.CaseNumber,
            ["rulingDate"] = r.RulingDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc),
            ["jurisdictionArea"] = r.JurisdictionArea,
            ["instance"] = r.Instance,
            ["court"] = r.CourtName,
            ["rulingDirection"] = r.RulingDirection,
            ["subjectArea"] = r.SubjectArea,
            ["resourceType"] = r.ResourceType,
            ["isUnconstitutional"] = r.IsUnconstitutional,
            ["keywords"] = r.Keywords,
            ["judges"] = r.Judges,
            ["statutes"] = r.Statutes
        };
        return doc;
    }).ToList();

    if (dryRun)
    {
        foreach (var doc in documents)
            Console.WriteLine($"  DRY-RUN merge: id={doc["id"]} court={doc["court"]} subjectArea={doc["subjectArea"]} jurisdictionArea={doc["jurisdictionArea"]}");
        updated += documents.Count;
    }
    else
    {
        try
        {
            var mergeActions = documents
                .Select(d => IndexDocumentsAction.MergeOrUpload(new SearchDocument(d)))
                .ToList();
            var indexBatch = IndexDocumentsBatch.Create(mergeActions.ToArray());
            var response = await searchClient.IndexDocumentsAsync(indexBatch);
            var succeeded = response.Value.Results.Count(r => r.Succeeded);
            var failed = response.Value.Results.Count(r => !r.Succeeded);
            updated += succeeded;
            errors += failed;

            if (failed > 0)
            {
                foreach (var r in response.Value.Results.Where(r => !r.Succeeded))
                    Console.WriteLine($"  FAIL {r.Key}: {r.ErrorMessage}");
            }

            Console.WriteLine($"  {progress} merged {succeeded} docs ({failed} failed)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  {progress} ERROR: {ex.Message}");
            errors += batch.Count;
        }
    }
}

Console.WriteLine();
Console.WriteLine($"Done. Updated={updated} Errors={errors}");
return errors > 0 ? 1 : 0;

// ── DB query ────────────────────────────────────────────────

async Task<List<RulingForReindex>> LoadRulingsForReindexAsync(string connStr, int maxRows)
{
    var results = new List<RulingForReindex>();
    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = $"""
        SELECT {(maxRows > 0 ? $"TOP {maxRows}" : "")}
               r.Id, r.CaseTitle, r.Summary, r.Holding, r.RulingDate,
               r.JurisdictionArea, r.Instance, c.Name AS CourtName,
               r.RulingDirection, r.SubjectArea, r.ResourceType, r.IsUnconstitutional,
               r.CaseNumber
        FROM Rulings r
        LEFT JOIN Courts c ON r.CourtId = c.Id
        WHERE r.SourceId = 1
        ORDER BY r.RulingDate DESC
        """;
    {
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var rulingId = reader.GetGuid(0);
            results.Add(new RulingForReindex
            {
                RulingId = rulingId,
                CaseTitle = reader.IsDBNull(1) ? "" : reader.GetString(1),
                Summary = reader.IsDBNull(2) ? null : reader.GetString(2),
                Holding = reader.IsDBNull(3) ? null : reader.GetString(3),
                RulingDate = DateOnly.FromDateTime(reader.GetDateTime(4)),
                JurisdictionArea = reader.IsDBNull(5) ? null : reader.GetString(5),
                Instance = reader.IsDBNull(6) ? null : reader.GetString(6),
                CourtName = reader.IsDBNull(7) ? null : reader.GetString(7),
                RulingDirection = reader.IsDBNull(8) ? null : reader.GetString(8),
                SubjectArea = reader.IsDBNull(9) ? null : reader.GetString(9),
                ResourceType = reader.IsDBNull(10) ? null : reader.GetString(10),
                IsUnconstitutional = reader.GetBoolean(11),
                CaseNumber = reader.IsDBNull(12) ? null : reader.GetString(12),
                Keywords = new List<string>()
            });
        }
    }

    if (results.Count > 0)
    {
        var ids = results.Select(r => r.RulingId).ToList();
        var keywordMap = await LoadKeywordsAsync(conn, ids);
        var judgeMap = await LoadJudgesAsync(conn, ids);
        var statuteMap = await LoadStatutesAsync(conn, ids);
        foreach (var r in results)
        {
            if (keywordMap.TryGetValue(r.RulingId, out var kws))
                r.Keywords = kws;
            if (judgeMap.TryGetValue(r.RulingId, out var jdg))
                r.Judges = jdg;
            if (statuteMap.TryGetValue(r.RulingId, out var sts))
                r.Statutes = sts;
        }
    }

    return results;
}

async Task<Dictionary<Guid, List<string>>> LoadKeywordsAsync(SqlConnection conn, List<Guid> rulingIds)
{
    var result = new Dictionary<Guid, List<string>>();
    if (rulingIds.Count == 0) return result;

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = """
        SELECT rk.RulingId, k.Description
        FROM RulingKeywords rk
        JOIN Keywords k ON rk.KeywordId = k.Id
        WHERE rk.RulingId IN (SELECT value FROM OPENJSON(@ids))
        ORDER BY rk.SortOrder
        """;
    var idsJson = JsonSerializer.Serialize(rulingIds.Select(id => id.ToString()));
    cmd.Parameters.AddWithValue("@ids", idsJson);
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var desc = reader.GetString(1);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = new List<string>();
        result[rulingId].Add(desc);
    }
    return result;
}

async Task<Dictionary<Guid, List<string>>> LoadJudgesAsync(SqlConnection conn, List<Guid> rulingIds)
{
    var result = new Dictionary<Guid, List<string>>();
    if (rulingIds.Count == 0) return result;

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = """
        SELECT rj.RulingId, j.LastName + ', ' + j.FirstName
        FROM RulingJudges rj
        JOIN Judges j ON rj.JudgeId = j.Id
        WHERE rj.RulingId IN (SELECT value FROM OPENJSON(@ids))
        ORDER BY j.LastName
        """;
    cmd.Parameters.AddWithValue("@ids", JsonSerializer.Serialize(rulingIds.Select(id => id.ToString())));
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var name = reader.GetString(1);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = new List<string>();
        result[rulingId].Add(name);
    }
    return result;
}

async Task<Dictionary<Guid, List<string>>> LoadStatutesAsync(SqlConnection conn, List<Guid> rulingIds)
{
    var result = new Dictionary<Guid, List<string>>();
    if (rulingIds.Count == 0) return result;

    await using var cmd = conn.CreateCommand();
    cmd.CommandText = """
        SELECT rs.RulingId, s.Number + ' ' + s.Name + COALESCE(' ' + rs.Articles, '')
        FROM RulingStatutes rs
        JOIN Statutes s ON rs.StatuteId = s.Id
        WHERE rs.RulingId IN (SELECT value FROM OPENJSON(@ids))
        """;
    cmd.Parameters.AddWithValue("@ids", JsonSerializer.Serialize(rulingIds.Select(id => id.ToString())));
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        var rulingId = reader.GetGuid(0);
        var label = reader.GetString(1);
        if (!result.ContainsKey(rulingId))
            result[rulingId] = new List<string>();
        result[rulingId].Add(label);
    }
    return result;
}

// ── Infrastructure ──────────────────────────────────────────

static void ShowUsage()
{
    Console.WriteLine("""
        Usage: dotnet run [options]
          --limit <N>         Max rulings to process (default: all)
          --batch-size <N>    Documents per Azure Search batch (default: 100)
          --dry-run           Show what would be updated without writing
          --reconcile         Compare DB vs index, delete orphans, report missing
        """);
}

async Task<int> ReconcileAsync()
{
    Console.WriteLine("Reconcile — DB is source of truth, index must match");
    Console.WriteLine();

    var sqlCs = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    var ep = Environment.GetEnvironmentVariable("AzureSearch__Endpoint");
    var key = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
    var rulingIdx = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";
    var chunkIdx = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName") ?? "rulings-by-chunk";

    if (string.IsNullOrWhiteSpace(sqlCs) || string.IsNullOrWhiteSpace(ep) || string.IsNullOrWhiteSpace(key))
    {
        Console.WriteLine("ERROR: SQL and Search config required.");
        return 1;
    }

    var cred = new AzureKeyCredential(key);
    var rulingClient = new SearchClient(new Uri(ep), rulingIdx, cred);
    var chunkClient = new SearchClient(new Uri(ep), chunkIdx, cred);

    // 1. Load ruling IDs from DB
    Console.Write("Loading ruling IDs from DB... ");
    var dbIds = new HashSet<Guid>();
    await using (var conn = new SqlConnection(sqlCs))
    {
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id FROM Rulings WHERE SourceId = 1";
        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            dbIds.Add(reader.GetGuid(0));
    }
    Console.WriteLine($"{dbIds.Count}");

    // 2. Load document IDs from ruling index
    Console.Write("Loading IDs from ruling index... ");
    var indexRulingIds = new HashSet<Guid>();
    var rulingDocKeys = new List<string>();
    var opts = new SearchOptions { Size = 1000, Select = { "id", "rulingId" } };
    int skip = 0;
    while (true)
    {
        opts.Skip = skip;
        var resp = await rulingClient.SearchAsync<SearchDocument>("*", opts);
        int count = 0;
        await foreach (var r in resp.Value.GetResultsAsync())
        {
            var docId = r.Document["id"]?.ToString() ?? "";
            var rulingIdStr = r.Document.ContainsKey("rulingId") ? r.Document["rulingId"]?.ToString() : null;
            if (Guid.TryParse(rulingIdStr, out var rid))
                indexRulingIds.Add(rid);
            rulingDocKeys.Add(docId);
            count++;
        }
        if (count < 1000) break;
        skip += 1000;
    }
    Console.WriteLine($"{indexRulingIds.Count} unique rulings ({rulingDocKeys.Count} docs)");

    // 3. Load chunk index doc keys grouped by rulingId
    Console.Write("Loading IDs from chunk index... ");
    var chunkDocKeys = new List<string>();
    var chunkRulingIds = new HashSet<Guid>();
    var cOpts = new SearchOptions { Size = 1000, Select = { "id", "rulingId" } };
    skip = 0;
    while (true)
    {
        cOpts.Skip = skip;
        var resp = await chunkClient.SearchAsync<SearchDocument>("*", cOpts);
        int count = 0;
        await foreach (var r in resp.Value.GetResultsAsync())
        {
            var docId = r.Document["id"]?.ToString() ?? "";
            var rulingIdStr = r.Document.ContainsKey("rulingId") ? r.Document["rulingId"]?.ToString() : null;
            if (Guid.TryParse(rulingIdStr, out var rid))
                chunkRulingIds.Add(rid);
            chunkDocKeys.Add(docId);
            count++;
        }
        if (count < 1000) break;
        skip += 1000;
    }
    Console.WriteLine($"{chunkRulingIds.Count} unique rulings ({chunkDocKeys.Count} docs)");
    Console.WriteLine();

    // 4. Compute diffs
    var orphanRulingIds = indexRulingIds.Except(dbIds).ToList();
    var orphanChunkRulingIds = chunkRulingIds.Except(dbIds).ToList();
    var missingFromRulingIndex = dbIds.Except(indexRulingIds).ToList();
    var missingFromChunkIndex = dbIds.Except(chunkRulingIds).ToList();

    Console.WriteLine($"Orphans in ruling index (not in DB): {orphanRulingIds.Count}");
    Console.WriteLine($"Orphans in chunk index (not in DB):  {orphanChunkRulingIds.Count}");
    Console.WriteLine($"Missing from ruling index:           {missingFromRulingIndex.Count}");
    Console.WriteLine($"Missing from chunk index:            {missingFromChunkIndex.Count}");
    Console.WriteLine();

    // 5. Build set of valid doc keys from DB
    //    Ruling index keys: just the GUID (e.g. "53f9f6e9-7654-...")
    //    Chunk index keys:  "{guid}-{chunkIndex}" (e.g. "53f9f6e9-...-0")
    var dbIdStrings = dbIds.Select(id => id.ToString()).ToHashSet(StringComparer.OrdinalIgnoreCase);

    var phantomRulingDocs = rulingDocKeys.Where(k => !dbIdStrings.Contains(k)).ToList();
    var phantomChunkDocs = chunkDocKeys.Where(k =>
    {
        var lastDash = k.LastIndexOf('-');
        if (lastDash <= 0) return true;
        var prefix = k[..lastDash];
        return !dbIdStrings.Contains(prefix);
    }).ToList();

    Console.WriteLine($"Phantom docs in ruling index (no matching DB ruling): {phantomRulingDocs.Count}");
    Console.WriteLine($"Phantom docs in chunk index (no matching DB ruling):  {phantomChunkDocs.Count}");

    // 6. Delete orphans + phantoms from ruling index
    var toDeleteRuling = new HashSet<string>(phantomRulingDocs);
    foreach (var oid in orphanRulingIds)
        toDeleteRuling.Add(oid.ToString());

    if (toDeleteRuling.Count > 0)
    {
        Console.Write($"Deleting {toDeleteRuling.Count} docs from ruling index... ");
        var delList = toDeleteRuling.ToList();
        for (int i = 0; i < delList.Count; i += 100)
        {
            var batch = delList.Skip(i).Take(100)
                .Select(id => IndexDocumentsAction.Delete("id", id))
                .ToArray();
            await rulingClient.IndexDocumentsAsync(IndexDocumentsBatch.Create(batch));
        }
        Console.WriteLine("done");
    }

    // 7. Delete orphans + phantoms from chunk index
    var toDeleteChunk = new HashSet<string>(phantomChunkDocs);
    foreach (var oid in orphanChunkRulingIds)
        toDeleteChunk.UnionWith(chunkDocKeys.Where(k => k.StartsWith($"{oid}-")));

    if (toDeleteChunk.Count > 0)
    {
        Console.Write($"Deleting {toDeleteChunk.Count} docs from chunk index... ");
        var delList = toDeleteChunk.ToList();
        for (int i = 0; i < delList.Count; i += 100)
        {
            var batch = delList.Skip(i).Take(100)
                .Select(id => IndexDocumentsAction.Delete("id", id))
                .ToArray();
            await chunkClient.IndexDocumentsAsync(IndexDocumentsBatch.Create(batch));
        }
        Console.WriteLine("done");
    }

    // 7. Report missing
    if (missingFromRulingIndex.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine($"WARNING: {missingFromRulingIndex.Count} DB rulings missing from ruling index:");
        await using var mConn = new SqlConnection(sqlCs);
        await mConn.OpenAsync();
        var idsCsv = string.Join(",", missingFromRulingIndex.Select(id => $"'{id}'"));
        await using var mCmd = mConn.CreateCommand();
        mCmd.CommandText = $"SELECT Id, ExternalId, SourceId, BlobPath, ContentHash, CaseTitle FROM Rulings WHERE Id IN ({idsCsv})";
        await using var mReader = await mCmd.ExecuteReaderAsync();
        while (await mReader.ReadAsync())
        {
            var rid = mReader.GetGuid(0);
            var extId = mReader.IsDBNull(1) ? "null" : mReader.GetString(1);
            var srcId = mReader.GetInt32(2);
            var blob = mReader.IsDBNull(3) ? "NULL" : mReader.GetString(3);
            var hash = mReader.IsDBNull(4) ? "NULL" : mReader.GetString(4);
            var title = mReader.IsDBNull(5) ? "" : mReader.GetString(5);
            Console.WriteLine($"  {rid}  extId={extId}  src={srcId}  blob={blob}  hash={hash.Substring(0, Math.Min(12, hash.Length))}...  title={title.Substring(0, Math.Min(60, title.Length))}");
        }
    }
    if (missingFromChunkIndex.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine($"WARNING: {missingFromChunkIndex.Count} DB rulings missing from chunk index.");
        Console.WriteLine("  Re-run BulkRequeue to index them. First 20:");
        foreach (var id in missingFromChunkIndex.Take(20))
            Console.WriteLine($"    {id}");
    }

    if (orphanRulingIds.Count == 0 && orphanChunkRulingIds.Count == 0 &&
        missingFromRulingIndex.Count == 0 && missingFromChunkIndex.Count == 0)
    {
        Console.WriteLine("Index is perfectly in sync with DB.");
    }

    return 0;
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

record RulingForReindex
{
    public Guid RulingId { get; init; }
    public string CaseTitle { get; init; } = "";
    public string? Summary { get; init; }
    public string? Holding { get; init; }
    public string? CaseNumber { get; init; }
    public DateOnly RulingDate { get; init; }
    public string? JurisdictionArea { get; init; }
    public string? Instance { get; init; }
    public string? CourtName { get; init; }
    public string? RulingDirection { get; init; }
    public string? SubjectArea { get; init; }
    public string? ResourceType { get; init; }
    public bool IsUnconstitutional { get; init; }
    public List<string> Keywords { get; set; } = new();
    public List<string> Judges { get; set; } = new();
    public List<string> Statutes { get; set; } = new();
}
