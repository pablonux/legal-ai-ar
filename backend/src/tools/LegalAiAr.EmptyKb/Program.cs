// Empty Knowledge Base: SQL, Blob, AI Search, optional Queues.
// Usage: dotnet run [--dry-run] [--clear-queues] [--clear-dlqs] [--peek-dlqs] [--fix-ingestion-jobs-migration] [--verify-empty] [--report]
// Env: AzureSql__ConnectionString, AzureBlob__ConnectionString, AzureSearch__Endpoint, AzureSearch__ApiKey
// Optional: EmptyKb__BlobPrefixes — comma-separated virtual prefixes (e.g. _cache/,legal-ai-ar-kb/) to delete only those trees; omit = entire container.
// Optional: EmptyKb__BlobProgressInterval — log progress every N blobs (default 250).

using System.Diagnostics;
using System.Text.RegularExpressions;
using Azure.Storage.Blobs;
using LegalAiAr.Core.Pipeline;

var prefix = Environment.GetEnvironmentVariable("Pipeline__QueuePrefix") ?? "pipeline";
var queueNames = new PipelineQueueNames(prefix);

var knownFlags = new HashSet<string> { "--dry-run", "--clear-queues", "--clear-dlqs", "--peek-dlqs", "--fix-ingestion-jobs-migration", "--verify-empty", "--report" };
var unknownFlags = args.Where(a => a.StartsWith("--") && !knownFlags.Contains(a)).ToList();
if (unknownFlags.Count > 0)
{
    Console.WriteLine($"Unknown flag(s): {string.Join(", ", unknownFlags)}");
    Console.WriteLine($"Usage: dotnet run [--dry-run] [--clear-queues] [--clear-dlqs] [--peek-dlqs] [--fix-ingestion-jobs-migration] [--verify-empty] [--report]");
    return 1;
}

var dryRun = args.Contains("--dry-run");
var clearQueues = args.Contains("--clear-queues");
var clearDlqs = args.Contains("--clear-dlqs");
var peekDlqs = args.Contains("--peek-dlqs");
var fixIngestionJobsMigration = args.Contains("--fix-ingestion-jobs-migration");
var verifyEmpty = args.Contains("--verify-empty");
var report = args.Contains("--report");

if (dryRun)
    Console.WriteLine("[DRY-RUN] No changes will be made.\n");

// 0a. Report counts (JSON for scripts)
if (report)
{
    await ReportCountsAsync(queueNames);
    return 0;
}

// 0b. Verify KB is empty (report counts)
if (verifyEmpty)
{
    var ok = await VerifyEmptyAsync(queueNames);
    Environment.Exit(ok ? 0 : 1);
}

// 0c. Clear only DLQs (isolated operation, does NOT touch SQL/Blob/Search)
if (clearDlqs)
{
    var dlqBlobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
    if (string.IsNullOrWhiteSpace(dlqBlobConn) || IsPlaceholder(dlqBlobConn))
        Console.WriteLine("DLQs: SKIP (connection string not configured)");
    else
        await DeleteDlqQueuesAsync(dlqBlobConn, dryRun, queueNames);
    return 0;
}

// 0d. Peek DLQ messages (read-only, shows message content)
if (peekDlqs)
{
    var peekBlobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
    if (string.IsNullOrWhiteSpace(peekBlobConn) || IsPlaceholder(peekBlobConn))
        Console.WriteLine("DLQs: SKIP (connection string not configured)");
    else
        await PeekDlqsAsync(peekBlobConn, queueNames);
    return 0;
}

// 0e. Fix IngestionJobs migration (one-time: creates table if migration was applied empty)
if (fixIngestionJobsMigration)
{
    var connStr = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    if (string.IsNullOrWhiteSpace(connStr) || IsPlaceholder(connStr))
        Console.WriteLine("Fix migration: SKIP (connection string not configured)");
    else
        await FixIngestionJobsMigrationAsync(connStr);
    return 0;
}

// Guard: if no flags that trigger the destructive empty path, show help
if (!clearQueues && args.Length > 0 && !args.Any(a => !a.StartsWith("--")))
{
    // Only --dry-run was passed, that's fine, continue to empty
}
else if (args.Length == 0)
{
    Console.WriteLine("EmptyKb: empties the entire Knowledge Base (SQL, Blob, Search).");
    Console.WriteLine("This is a DESTRUCTIVE operation. Pass at least one flag:");
    Console.WriteLine("  --dry-run               Show what would be deleted (blob listing skipped — large containers)");
    Console.WriteLine("  --clear-queues          Delete pipeline Storage Queue resources (+ legacy csjn-ruling-*), not just drain");
    Console.WriteLine("  --clear-dlqs            Delete DLQ queues only (safe: no KB data / main queues touched)");
    Console.WriteLine("  --peek-dlqs             Show DLQ message contents (read-only)");
    Console.WriteLine("  --verify-empty          Report KB counts and verify empty");
    Console.WriteLine("  --report                JSON report of rulings + DLQ counts");
    Console.WriteLine("  --fix-ingestion-jobs-migration  One-time migration fix");
    Console.WriteLine("\nTo empty the KB: dotnet run -- --dry-run  (preview)");
    Console.WriteLine("                 dotnet run -- --clear-queues  (empty KB + delete queue resources)");
    return 0;
}

// 1. Azure SQL
var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
if (string.IsNullOrWhiteSpace(sqlConn) || IsPlaceholder(sqlConn))
{
    Console.WriteLine("Azure SQL: SKIP (connection string not configured)");
}
else
{
    await EmptySqlAsync(sqlConn, dryRun);
}

// 2. Azure Blob
var blobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
var containerName = Environment.GetEnvironmentVariable("AzureBlob__ContainerName") ?? "rulings-pdfs";
if (string.IsNullOrWhiteSpace(blobConn) || IsPlaceholder(blobConn))
{
    Console.WriteLine("Azure Blob: SKIP (connection string not configured)");
}
else
{
    await EmptyBlobAsync(blobConn, containerName, dryRun);
}

// 3. Azure AI Search
var searchEndpoint = Environment.GetEnvironmentVariable("AzureSearch__Endpoint")?.TrimEnd('/');
var searchKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
var rulingIndex = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";
var chunkIndex = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName") ?? "rulings-by-chunk";
if (string.IsNullOrWhiteSpace(searchKey) || IsPlaceholder(searchKey) || string.IsNullOrWhiteSpace(searchEndpoint))
{
    Console.WriteLine("Azure AI Search: SKIP (endpoint or api-key not configured)");
}
else
{
    await EmptySearchAsync(searchEndpoint, searchKey, rulingIndex, chunkIndex, dryRun);
}

// 4. Storage Queues (optional)
if (clearQueues)
{
    if (string.IsNullOrWhiteSpace(blobConn) || IsPlaceholder(blobConn))
        Console.WriteLine("Storage Queues: SKIP (connection string not configured)");
    else
        await DeletePipelineQueuesAsync(blobConn, dryRun, queueNames);
}
else
{
    Console.WriteLine("Storage Queues: SKIP (use --clear-queues to delete queue resources)");
}

Console.WriteLine("\nKB empty. Ready for first ingestion.");
return 0;

static async Task<int> GetQueueCountAsync(Azure.Storage.Queues.QueueServiceClient client, string name)
{
    var qc = client.GetQueueClient(name);
    if (!await qc.ExistsAsync()) return 0;
    var props = await qc.GetPropertiesAsync();
    return props.Value.ApproximateMessagesCount;
}

static bool IsPlaceholder(string? v) =>
    string.IsNullOrWhiteSpace(v) ||
    v is "DB_SECRET" or "STORAGE_KEY" or "SEARCH_KEY" or "OPENAI_KEY";

static async Task<bool> VerifyEmptyAsync(PipelineQueueNames queueNames)
{
    var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    var blobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
    var containerName = Environment.GetEnvironmentVariable("AzureBlob__ContainerName") ?? "rulings-pdfs";
    var searchEndpoint = Environment.GetEnvironmentVariable("AzureSearch__Endpoint")?.TrimEnd('/');
    var searchKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
    var rulingIndex = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";
    var chunkIndex = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName") ?? "rulings-by-chunk";

    var allEmpty = true;

    // DB
    if (!string.IsNullOrWhiteSpace(sqlConn) && !IsPlaceholder(sqlConn))
    {
        var tables = new[] {
            "Rulings", "Documents", "IngestionJobs", "Citations", "Persons", "Courts",
            "Keywords", "Statutes", "JudicialProceedings", "FieldProvenance",
            "DocumentStageLogs", "GraphCommunities", "ThesaurusTerms", "StateOrgans"
        };
        await using var conn = new Microsoft.Data.SqlClient.SqlConnection(sqlConn);
        await conn.OpenAsync();
        foreach (var table in tables)
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = $"SELECT COUNT(*) FROM [{table}]";
            var count = (int)(await cmd.ExecuteScalarAsync() ?? 0);
            var status = count == 0 ? "OK" : "NOT EMPTY";
            if (count > 0) allEmpty = false;
            Console.WriteLine($"DB [{table}]: {count} rows — {status}");
        }
    }
    else
        Console.WriteLine("DB: SKIP (not configured)");

    // Blob — respect EmptyKb__BlobPrefixes when counting
    if (!string.IsNullOrWhiteSpace(blobConn) && !IsPlaceholder(blobConn))
    {
        var client = new Azure.Storage.Blobs.BlobServiceClient(blobConn);
        var container = client.GetBlobContainerClient(containerName);
        var prefixes = ResolveBlobPrefixes();
        var count = 0;
        if (await container.ExistsAsync())
        {
            if (prefixes.Length == 0)
                await foreach (var _ in container.GetBlobsAsync()) count++;
            else
            {
                foreach (var p in prefixes)
                {
                    await foreach (var _ in container.GetBlobsAsync(prefix: p)) count++;
                }
            }
        }

        string scope = prefixes.Length == 0 ? "all blobs" : string.Join(", ", prefixes);
        var status = count == 0 ? "OK" : "NOT EMPTY";
        if (count > 0) allEmpty = false;
        Console.WriteLine($"Blob [{containerName}] ({scope}): {count} blobs — {status}");
    }
    else
        Console.WriteLine("Blob: SKIP (not configured)");

    // Search
    if (!string.IsNullOrWhiteSpace(searchKey) && !IsPlaceholder(searchKey) && !string.IsNullOrWhiteSpace(searchEndpoint))
    {
        var credential = new Azure.AzureKeyCredential(searchKey);
        var rulingClient = new Azure.Search.Documents.SearchClient(new Uri(searchEndpoint), rulingIndex, credential);
        var chunkClient = new Azure.Search.Documents.SearchClient(new Uri(searchEndpoint), chunkIndex, credential);
        var searchOptions = new Azure.Search.Documents.SearchOptions { IncludeTotalCount = true };
        var rulingCount = (long)((await rulingClient.SearchAsync<SearchDoc>("*", searchOptions)).Value.TotalCount ?? 0);
        var chunkCount = (long)((await chunkClient.SearchAsync<SearchDoc>("*", searchOptions)).Value.TotalCount ?? 0);
        var rStatus = rulingCount == 0 ? "OK" : "NOT EMPTY";
        var cStatus = chunkCount == 0 ? "OK" : "NOT EMPTY";
        if (rulingCount > 0 || chunkCount > 0) allEmpty = false;
        Console.WriteLine($"Search [{rulingIndex}]: {rulingCount} docs — {rStatus}");
        Console.WriteLine($"Search [{chunkIndex}]: {chunkCount} docs — {cStatus}");
    }
    else
        Console.WriteLine("Search: SKIP (not configured)");

    // Queues — no managed pipeline queue should still exist
    if (!string.IsNullOrWhiteSpace(blobConn) && !IsPlaceholder(blobConn))
    {
        var queueClient = new Azure.Storage.Queues.QueueServiceClient(blobConn);
        var prefixes = GetPipelinePrefixesForQueueDeletion();
        var managed = await ListManagedQueueNamesInAccountAsync(queueClient, prefixes);
        foreach (var name in managed)
        {
            allEmpty = false;
            Console.WriteLine($"Queue [{name}]: still exists — NOT EMPTY (delete with --clear-queues)");
        }
        if (managed.Count == 0)
            Console.WriteLine("Queues: OK (no managed pipeline queues in account)");
    }
    else
        Console.WriteLine("Queues: SKIP (not configured)");

    Console.WriteLine();
    if (allEmpty)
        Console.WriteLine("KB is completely empty.");
    else
    {
        Console.WriteLine("KB is NOT empty. Run: dotnet run -- --clear-queues (deletes queue resources)");
        Console.WriteLine("Note: Stop API and workers before clearing queues. Queue counts are approximate.");
    }
    return allEmpty;
}

static async Task ReportCountsAsync(PipelineQueueNames queueNames)
{
    var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    var blobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");

    var rulings = 0;
    var dlqCrawler = 0;
    var dlqParser = 0;
    var dlqEnrichment = 0;
    var dlqIndexer = 0;

    if (!string.IsNullOrWhiteSpace(sqlConn) && !IsPlaceholder(sqlConn))
    {
        await using var conn = new Microsoft.Data.SqlClient.SqlConnection(sqlConn);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT COUNT(*) FROM [Rulings]";
        rulings = (int)(await cmd.ExecuteScalarAsync() ?? 0);
    }

    if (!string.IsNullOrWhiteSpace(blobConn) && !IsPlaceholder(blobConn))
    {
        var queueClient = new Azure.Storage.Queues.QueueServiceClient(blobConn);
        dlqCrawler = await GetQueueCountAsync(queueClient, queueNames.DiscovererDlq);
        dlqParser = await GetQueueCountAsync(queueClient, queueNames.ParserDlq);
        dlqEnrichment = await GetQueueCountAsync(queueClient, queueNames.EnricherDlq);
        dlqIndexer = await GetQueueCountAsync(queueClient, queueNames.IndexerDlq);
    }

    var dlqTotal = dlqCrawler + dlqParser + dlqEnrichment + dlqIndexer;
    var json = System.Text.Json.JsonSerializer.Serialize(new
    {
        rulings,
        dlqCrawler,
        dlqParser,
        dlqEnrichment,
        dlqIndexer,
        dlqTotal,
        rulingsPlusDlq = rulings + dlqTotal
    });
    Console.WriteLine(json);
}

static async Task FixIngestionJobsMigrationAsync(string connectionString)
{
    await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    await conn.OpenAsync();

    // 1. Remove migration from history so EF will re-apply it
    await using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "DELETE FROM [__EFMigrationsHistory] WHERE [MigrationId] = '20260313160255_AddIngestionJobsTable'";
        var rows = await cmd.ExecuteNonQueryAsync();
        Console.WriteLine($"Fix migration: Removed from history ({rows} row(s))");
    }

    // 2. Create IngestionJobs table if not exists
    await using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = """
            IF OBJECT_ID(N'[IngestionJobs]', 'U') IS NULL
            BEGIN
                CREATE TABLE [IngestionJobs] (
                    [Id] uniqueidentifier NOT NULL,
                    [SourceId] int NOT NULL,
                    [Type] nvarchar(20) NOT NULL,
                    [TriggeredBy] nvarchar(100) NOT NULL,
                    [StartedAt] datetime2 NOT NULL,
                    [CompletedAt] datetime2 NULL,
                    [Status] nvarchar(20) NOT NULL,
                    [DocumentsDiscovered] int NOT NULL,
                    [DocumentsIndexed] int NOT NULL,
                    [DocumentsFailed] int NOT NULL,
                    [ErrorSummary] nvarchar(max) NULL,
                    CONSTRAINT [PK_IngestionJobs] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_IngestionJobs_Sources_SourceId] FOREIGN KEY ([SourceId]) REFERENCES [Sources] ([Id]) ON DELETE NO ACTION
                );
                CREATE INDEX [IX_IngestionJobs_SourceId] ON [IngestionJobs] ([SourceId]);
                CREATE INDEX [IX_IngestionJobs_StartedAt] ON [IngestionJobs] ([StartedAt]);
            END
            """;
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Fix migration: IngestionJobs table created (or already exists)");
    }

    // 3. Add IngestionJobId to Rulings if not exists
    await using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = """
            IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('Rulings') AND name = 'IngestionJobId')
            BEGIN
                ALTER TABLE [Rulings] ADD [IngestionJobId] uniqueidentifier NULL;
                CREATE INDEX [IX_Rulings_IngestionJobId] ON [Rulings] ([IngestionJobId]);
                ALTER TABLE [Rulings] ADD CONSTRAINT [FK_Rulings_IngestionJobs_IngestionJobId] FOREIGN KEY ([IngestionJobId]) REFERENCES [IngestionJobs] ([Id]) ON DELETE SET NULL;
            END
            """;
        await cmd.ExecuteNonQueryAsync();
        Console.WriteLine("Fix migration: Rulings.IngestionJobId column (or already exists)");
    }

    // 4. Re-insert migration into history
    await using (var cmd = conn.CreateCommand())
    {
        cmd.CommandText = "INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion]) VALUES ('20260313160255_AddIngestionJobsTable', '8.0.11')";
        try
        {
            await cmd.ExecuteNonQueryAsync();
            Console.WriteLine("Fix migration: Migration recorded in history");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2627) // Duplicate key
        {
            Console.WriteLine("Fix migration: Migration already in history");
        }
    }

    Console.WriteLine("\nIngestionJobs migration fix complete.");
}

static async Task EmptySqlAsync(string connectionString, bool dryRun)
{
    var commands = new[]
    {
        // Leaf tables (no dependents)
        "DELETE FROM [ChunkEntityMentions]",
        "DELETE FROM [CommunityMemberships]",
        "DELETE FROM [RulingEmbeddingStates]",
        "DELETE FROM [RulingSourceMetadata]",
        "DELETE FROM [ExternalIdentifiers]",
        "DELETE FROM [DocumentStageLogs]",
        "DELETE FROM [FieldProvenance]",
        "DELETE FROM [EntityAuditLogs]",
        "DELETE FROM [IngestionJobDetails]",
        "DELETE FROM [RulingLinks]",
        "DELETE FROM [RulingSyntheses]",
        "DELETE FROM [SumarioKeywords]",
        "DELETE FROM [Sumarios]",
        "DELETE FROM [RulingKeywords]",
        "DELETE FROM [RulingStatuteArticles]",

        // Mid-level (RulingParticipations before Votes due to FK)
        "DELETE FROM [RulingStatutes]",
        "DELETE FROM [ProsecutorOpinions]",
        "DELETE FROM [RulingParticipations]",
        "DELETE FROM [Votes]",
        "DELETE FROM [LegalDoctrines]",
        "DELETE FROM [ProceduralRemedies]",
        "DELETE FROM [LegalRepresentations]",
        "DELETE FROM [ProceedingParties]",
        "DELETE FROM [Citations]",
        "DELETE FROM [NormRelations]",
        "DELETE FROM [ThesaurusRelations]",

        // Core entities with FKs
        "DELETE FROM [Documents]",
        "DELETE FROM [Rulings]",
        "DELETE FROM [JudicialOffices]",
        "DELETE FROM [JudicialProceedings]",
        "DELETE FROM [GraphCommunities]",

        // Root transactional tables
        "DELETE FROM [IngestionJobs]",
        "DELETE FROM [Persons]",
        "DELETE FROM [Courts]",
        "DELETE FROM [Keywords]",
        "DELETE FROM [ThesaurusTerms]",
        "DELETE FROM [Statutes]",
        "DELETE FROM [StateOrgans]",

        // Reset reference data state
        "UPDATE [CrawlerConfigs] SET [LastCrawledAt] = NULL, [LastCrawledStatus] = NULL, [LastDocumentCount] = NULL",
        "UPDATE [WorkerPauseStates] SET [IsPaused] = 0",
    };

    if (dryRun)
    {
        Console.WriteLine($"Azure SQL: Would execute {commands.Length} commands:");
        foreach (var c in commands) Console.WriteLine($"  {c}");
        return;
    }

    await using var conn = new Microsoft.Data.SqlClient.SqlConnection(connectionString);
    await conn.OpenAsync();
    foreach (var cmdText in commands)
    {
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = cmdText;
        cmd.CommandTimeout = 300;
        try
        {
            var rows = await cmd.ExecuteNonQueryAsync();
            Console.WriteLine($"  {cmdText} — {rows} rows");
        }
        catch (Microsoft.Data.SqlClient.SqlException ex)
        {
            Console.WriteLine($"  {cmdText} — ERROR: {ex.Message}");
        }
    }
}

static async Task EmptyBlobAsync(string connectionString, string containerName, bool dryRun)
{
    var client = new BlobServiceClient(connectionString);
    var container = client.GetBlobContainerClient(containerName);
    if (!await container.ExistsAsync())
    {
        Console.WriteLine($"Azure Blob: Container '{containerName}' does not exist.");
        return;
    }

    var prefixes = ResolveBlobPrefixes();
    var interval = GetBlobProgressInterval();

    // Dry-run: do not list blobs (large containers can take minutes and look hung).
    if (dryRun)
    {
        if (prefixes.Length == 0)
            Console.WriteLine($"Azure Blob [DRY-RUN]: Would delete ALL blobs in container '{containerName}' (listing skipped).");
        else
            Console.WriteLine($"Azure Blob [DRY-RUN]: Would delete blobs under prefix(es) {string.Join(", ", prefixes)} in '{containerName}' (listing skipped).");
        Console.WriteLine("Azure Blob [DRY-RUN]: Run without --dry-run to perform deletes.");
        return;
    }

    Console.WriteLine(
        $"Azure Blob: Deleting in '{containerName}' (progress every {interval} blob(s); scanning can take time before the first line).");

    if (prefixes.Length == 0)
    {
        var n = await DeleteBlobsUnderScopeAsync(container, blobPrefix: null, displayScope: "full container", progressInterval: interval);
        Console.WriteLine($"Azure Blob: Done — {n} blob(s) removed from '{containerName}' (full container).");
        return;
    }

    var total = 0;
    foreach (var p in prefixes)
        total += await DeleteBlobsUnderScopeAsync(container, blobPrefix: p, displayScope: $"prefix '{p}'", progressInterval: interval);

    Console.WriteLine($"Azure Blob: Done — {total} blob(s) removed across prefix(es) [{string.Join(", ", prefixes)}].");
}

/// <summary>
/// Enumerates and deletes blobs under an optional prefix; logs heartbeat so long runs are not silent.
/// </summary>
static async Task<int> DeleteBlobsUnderScopeAsync(
    BlobContainerClient container,
    string? blobPrefix,
    string displayScope,
    int progressInterval)
{
    Console.WriteLine($"Azure Blob: {displayScope} — calling Storage to list blobs...");
    var sw = Stopwatch.StartNew();
    var deleted = 0;
    var sawAny = false;

    var enumerable = string.IsNullOrEmpty(blobPrefix)
        ? container.GetBlobsAsync()
        : container.GetBlobsAsync(prefix: blobPrefix);

    await foreach (var blob in enumerable)
    {
        if (!sawAny)
        {
            sawAny = true;
            Console.WriteLine($"Azure Blob: {displayScope} — first listing page after {FormatElapsed(sw)}, starting deletes...");
        }

        await container.GetBlobClient(blob.Name).DeleteIfExistsAsync();
        deleted++;

        if (deleted == 1 || deleted % progressInterval == 0)
        {
            Console.WriteLine($"Azure Blob: {displayScope} — deleted {deleted} blob(s), elapsed {FormatElapsed(sw)}");
            Console.Out.Flush();
        }
    }

    if (!sawAny)
        Console.WriteLine($"Azure Blob: {displayScope} — nothing to delete ({FormatElapsed(sw)}).");
    else
        Console.WriteLine($"Azure Blob: {displayScope} — finished {deleted} blob(s) in {FormatElapsed(sw)}.");

    return deleted;
}

static string FormatElapsed(Stopwatch sw) =>
    sw.Elapsed.TotalHours >= 1
        ? sw.Elapsed.ToString(@"hh\:mm\:ss")
        : sw.Elapsed.ToString(@"mm\:ss\.fff");

static async Task EmptySearchAsync(string endpoint, string apiKey, string rulingIndex, string chunkIndex, bool dryRun)
{
    var credential = new Azure.AzureKeyCredential(apiKey);
    var rulingClient = new Azure.Search.Documents.SearchClient(new Uri(endpoint), rulingIndex, credential);
    var chunkClient = new Azure.Search.Documents.SearchClient(new Uri(endpoint), chunkIndex, credential);

    if (dryRun)
    {
        Console.WriteLine("Azure AI Search: Would delete all documents from rulings and chunks indexes.");
        return;
    }

    var rulingCount = await DeleteAllFromIndexAsync(rulingClient, "id", rulingIndex);
    Console.WriteLine($"Azure AI Search: Finished '{rulingIndex}' ({rulingCount} document(s)).");

    var chunkCount = await DeleteAllFromIndexAsync(chunkClient, "id", chunkIndex);
    Console.WriteLine($"Azure AI Search: Finished '{chunkIndex}' ({chunkCount} document(s)).");
}

static async Task<int> DeleteAllFromIndexAsync(
    Azure.Search.Documents.SearchClient client,
    string keyField,
    string indexLabel)
{
    const int keyProgressInterval = 25_000;
    Console.WriteLine($"Azure AI Search [{indexLabel}]: listing keys (may take a while)...");

    var sw = Stopwatch.StartNew();
    var allIds = new List<string>();
    var response = await client.SearchAsync<SearchDoc>("*");
    await foreach (var result in response.Value.GetResultsAsync())
    {
        if (result.Document?.Id != null)
            allIds.Add(result.Document.Id);
        if (allIds.Count == 1 || allIds.Count % keyProgressInterval == 0)
        {
            Console.WriteLine($"Azure AI Search [{indexLabel}]: gathered {allIds.Count} key(s), elapsed {FormatElapsed(sw)}");
            Console.Out.Flush();
        }
    }

    Console.WriteLine($"Azure AI Search [{indexLabel}]: deleting {allIds.Count} document(s) in batches of 1000...");

    const int batchSize = 1000;
    var batchNum = 0;
    for (var i = 0; i < allIds.Count; i += batchSize)
    {
        var batch = allIds.Skip(i).Take(batchSize).ToList();
        await client.DeleteDocumentsAsync(keyField, batch);
        batchNum++;
        if (batchNum == 1 || batchNum % 10 == 0 || i + batchSize >= allIds.Count)
        {
            Console.WriteLine(
                $"Azure AI Search [{indexLabel}]: batch {batchNum}, removed up to {Math.Min(i + batchSize, allIds.Count)} / {allIds.Count}, elapsed {FormatElapsed(sw)}");
            Console.Out.Flush();
        }
    }

    return allIds.Count;
}

static async Task PeekDlqsAsync(string connectionString, PipelineQueueNames queueNames)
{
    var queueClient = new Azure.Storage.Queues.QueueServiceClient(connectionString);
    foreach (var name in queueNames.AllDlq)
    {
        var qc = queueClient.GetQueueClient(name);
        if (!await qc.ExistsAsync()) { Console.WriteLine($"[{name}] (does not exist)"); continue; }
        var props = await qc.GetPropertiesAsync();
        var count = props.Value.ApproximateMessagesCount;
        if (count == 0) { Console.WriteLine($"[{name}] empty"); continue; }
        Console.WriteLine($"[{name}] ~{count} message(s):");
        var msgs = await qc.PeekMessagesAsync(Math.Min(count, 5));
        foreach (var m in msgs.Value)
        {
            var body = m.Body.ToString();
            Console.WriteLine($"  MessageId={m.MessageId} InsertedOn={m.InsertedOn}");
            Console.WriteLine($"  {body}");
            Console.WriteLine();
        }
    }
}

static async Task DeleteDlqQueuesAsync(string connectionString, bool dryRun, PipelineQueueNames queueNames)
{
    var prefixes = GetPipelinePrefixesForQueueDeletion();
    var discovered = await ListManagedQueueNamesInAccountAsync(
        new Azure.Storage.Queues.QueueServiceClient(connectionString), prefixes);
    var dlqNames = discovered
        .Where(n => n.EndsWith("-dlq", StringComparison.OrdinalIgnoreCase))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
        .ToList();
    // Ensure current config DLQs are included even if listing API lagged
    foreach (var q in queueNames.AllDlq)
        if (!dlqNames.Contains(q, StringComparer.OrdinalIgnoreCase))
            dlqNames.Add(q);
    dlqNames.Sort(StringComparer.OrdinalIgnoreCase);
    await DeleteQueueResourcesByNameAsync(connectionString, dryRun, dlqNames);
}

static async Task DeletePipelineQueuesAsync(string connectionString, bool dryRun, PipelineQueueNames queueNames)
{
    var queueService = new Azure.Storage.Queues.QueueServiceClient(connectionString);
    var prefixes = GetPipelinePrefixesForQueueDeletion();
    var discovered = await ListManagedQueueNamesInAccountAsync(queueService, prefixes);

    var targets = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
    foreach (var n in queueNames.All) targets.Add(n);
    foreach (var n in discovered) targets.Add(n);

    Console.WriteLine($"Storage Queues: Deleting {targets.Count} pipeline queue resource(s) (prefixes: {string.Join(", ", prefixes)} + legacy csjn-ruling-*).");
    await DeleteQueueResourcesByNameAsync(connectionString, dryRun, targets.ToList());
}

static async Task DeleteQueueResourcesByNameAsync(string connectionString, bool dryRun, IReadOnlyList<string> names)
{
    var queueClient = new Azure.Storage.Queues.QueueServiceClient(connectionString);
    foreach (var name in names)
    {
        var qc = queueClient.GetQueueClient(name);
        if (!await qc.ExistsAsync())
        {
            Console.WriteLine($"Storage Queue '{name}': (does not exist)");
            continue;
        }

        if (dryRun)
        {
            Console.WriteLine($"Storage Queue '{name}': Would DELETE resource");
            continue;
        }

        await qc.DeleteIfExistsAsync();
        Console.WriteLine($"Storage Queue '{name}': deleted");
    }
}

static string[] ResolveBlobPrefixes()
{
    var raw = Environment.GetEnvironmentVariable("EmptyKb__BlobPrefixes");
    if (string.IsNullOrWhiteSpace(raw)) return [];
    return raw.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(static s => s.TrimEnd('/') + "/")
        .ToArray();
}

static int GetBlobProgressInterval()
{
    var raw = Environment.GetEnvironmentVariable("EmptyKb__BlobProgressInterval");
    if (string.IsNullOrWhiteSpace(raw))
        return 250;
    return int.TryParse(raw, out var n) && n > 0 ? n : 250;
}

static HashSet<string> GetPipelinePrefixesForQueueDeletion()
{
    var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        (Environment.GetEnvironmentVariable("Pipeline__QueuePrefix") ?? "pipeline").Trim()
    };
    var extra = Environment.GetEnvironmentVariable("EmptyKb__ExtraQueuePrefixes");
    if (string.IsNullOrWhiteSpace(extra)) return set;
    foreach (var e in extra.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        var t = e.Trim();
        if (t.Length > 0) set.Add(t);
    }
    return set;
}

static bool MatchesLegacyCsjnQueue(string name) =>
    Regex.IsMatch(
        name,
        @"^csjn-ruling-(crawler|parser|enrichment|indexer)(-dlq)?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

static bool MatchesModernPipelineQueue(string name, string prefix)
{
    if (string.IsNullOrWhiteSpace(prefix)) return false;
    var escaped = Regex.Escape(prefix.Trim());
    return Regex.IsMatch(
        name,
        $"^{escaped}-(discoverer|crawler|fetcher|parser|enricher|enrichment|persister|indexer)(-dlq)?$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
}

static bool IsManagedLegalAiQueueName(string name, IReadOnlyCollection<string> prefixes)
{
    if (MatchesLegacyCsjnQueue(name)) return true;
    return prefixes.Any(p => MatchesModernPipelineQueue(name, p));
}

static async Task<List<string>> ListManagedQueueNamesInAccountAsync(
    Azure.Storage.Queues.QueueServiceClient client,
    IReadOnlyCollection<string> prefixes)
{
    var result = new SortedSet<string>(StringComparer.OrdinalIgnoreCase);
    await foreach (var item in client.GetQueuesAsync())
    {
        if (IsManagedLegalAiQueueName(item.Name, prefixes))
            result.Add(item.Name);
    }
    return result.ToList();
}

file class SearchDoc
{
    [System.Text.Json.Serialization.JsonPropertyName("id")]
    public string? Id { get; init; }
}
