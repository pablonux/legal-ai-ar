using System.Net.Http.Headers;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;
using LegalAiAr.Core.Pipeline;
using Microsoft.Data.SqlClient;

LoadEnvFile();

// benchmark-job-snapshot — ejecuta scripts/sql/benchmark-latest-job-snapshot.sql (desde repo raíz donde está .env)
if (args.Length > 0 && args[0].Equals("benchmark-job-snapshot", StringComparison.OrdinalIgnoreCase))
{
    var sqlCs = Environment.GetEnvironmentVariable("AzureSql__ConnectionString");
    if (string.IsNullOrWhiteSpace(sqlCs) || IsPlaceholder(sqlCs))
    {
        Console.WriteLine("ERROR: AzureSql__ConnectionString no configurado (.env).");
        return 1;
    }

    var repoRoot = FindRepoRootWithEnvFile() ?? Directory.GetCurrentDirectory();
    var sqlPath = args.Length > 1
        ? args[1]
        : Path.Combine(repoRoot, "scripts", "sql", "benchmark-latest-job-snapshot.sql");

    if (!File.Exists(sqlPath))
    {
        Console.WriteLine($"ERROR: SQL no encontrado: {sqlPath}");
        return 1;
    }

    var sql = await File.ReadAllTextAsync(sqlPath);
    try
    {
        await using var conn = new SqlConnection(sqlCs);
        conn.InfoMessage += (_, e) =>
        {
            foreach (SqlError line in e.Errors)
                if (line.Class <= 10)
                    Console.WriteLine(line.Message);
        };
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn)
        {
            CommandTimeout = 120
        };
        await using var reader = await cmd.ExecuteReaderAsync();
        var rs = 0;
        while (true)
        {
            if (reader.FieldCount > 0)
            {
                Console.WriteLine($"\n--- result set {++rs} ---");
                Console.WriteLine(string.Join('\t', BenchmarkSnapshotColumnNames(reader)));
                while (await reader.ReadAsync())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        if (i > 0)
                            Console.Write('\t');
                        var v = reader.IsDBNull(i) ? "" : reader.GetValue(i);
                        Console.Write(v);
                    }

                    Console.WriteLine();
                }
            }

            if (!await reader.NextResultAsync())
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        return 1;
    }

    return 0;
}

// benchmark-job-stages <JobId> — mismo resumen por Stage que benchmark-latest-job-snapshot (sin listar docs)
if (args.Length > 1 && args[0].Equals("benchmark-job-stages", StringComparison.OrdinalIgnoreCase))
{
    var sqlCs = Environment.GetEnvironmentVariable("AzureSql__ConnectionString");
    if (string.IsNullOrWhiteSpace(sqlCs) || IsPlaceholder(sqlCs))
    {
        Console.WriteLine("ERROR: AzureSql__ConnectionString no configurado (.env).");
        return 1;
    }

    if (!Guid.TryParse(args[1], out var jobId))
    {
        Console.WriteLine("ERROR: JobId GUID inválido.");
        return 1;
    }

    const string sql = """
SELECT
    l.Stage,
    COUNT(DISTINCT l.DocumentId) AS documents_with_log,
    SUM(l.DurationMs) AS sum_duration_ms,
    AVG(CAST(l.DurationMs AS FLOAT)) AS avg_duration_ms_per_row,
    SUM(CASE WHEN l.ErrorMessage IS NOT NULL THEN 1 ELSE 0 END) AS rows_with_error
FROM dbo.DocumentStageLogs AS l
INNER JOIN dbo.Documents AS d ON d.Id = l.DocumentId AND d.IngestionJobId = @pJobId
GROUP BY l.Stage
ORDER BY
    CASE l.Stage
        WHEN N'Discoverer' THEN 1
        WHEN N'Fetcher' THEN 2
        WHEN N'Parser' THEN 3
        WHEN N'Enricher' THEN 4
        WHEN N'Persister' THEN 5
        WHEN N'Indexer' THEN 6
        ELSE 99
    END;

SELECT @pJobId AS JobId,
       MIN(l.StartedAt) AS LogsFromUtcMin,
       MAX(COALESCE(l.CompletedAt, l.StartedAt)) AS LogsToUtcMax
FROM dbo.DocumentStageLogs AS l
INNER JOIN dbo.Documents AS d ON d.Id = l.DocumentId AND d.IngestionJobId = @pJobId;
""";

    try
    {
        await using var conn = new SqlConnection(sqlCs);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn) { CommandTimeout = 120 };
        cmd.Parameters.AddWithValue("@pJobId", jobId);
        await using var reader = await cmd.ExecuteReaderAsync();
        var rs = 0;
        while (true)
        {
            if (reader.FieldCount > 0)
            {
                Console.WriteLine($"\n--- result set {++rs} ---");
                Console.WriteLine(string.Join('\t', BenchmarkSnapshotColumnNames(reader)));
                while (await reader.ReadAsync())
                {
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        if (i > 0)
                            Console.Write('\t');
                        var v = reader.IsDBNull(i) ? "" : reader.GetValue(i);
                        Console.Write(v);
                    }

                    Console.WriteLine();
                }
            }

            if (!await reader.NextResultAsync())
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        return 1;
    }

    return 0;
}

// search-counts — one-shot doc counts via REST ($count)
if (args.Length > 0 && args[0].Equals("search-counts", StringComparison.OrdinalIgnoreCase))
{
    const string apiVersion = "2024-07-01";
    var searchEp = Environment.GetEnvironmentVariable("AzureSearch__Endpoint")?.TrimEnd('/');
    var searchApiKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
    var rulingIdx = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";
    var chunkIdx = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName") ?? "rulings-by-chunk";
    var statuteIdx = Environment.GetEnvironmentVariable("AzureSearch__StatuteIndexName");

    if (string.IsNullOrWhiteSpace(searchApiKey) || IsPlaceholder(searchApiKey)
        || string.IsNullOrWhiteSpace(searchEp))
    {
        Console.WriteLine("ERROR: AzureSearch__Endpoint and AzureSearch__ApiKey are required (.env).");
        return 1;
    }

    using var http = new HttpClient();
    http.DefaultRequestHeaders.Add("api-key", searchApiKey);

    async Task<long?> CountAsync(string indexName)
    {
        try
        {
            var resp = await http.GetAsync($"{searchEp}/indexes/{Uri.EscapeDataString(indexName)}/docs/$count?api-version={apiVersion}");
            var body = (await resp.Content.ReadAsStringAsync()).Trim();
            if (!resp.IsSuccessStatusCode)
                return null;
            return long.TryParse(body, out var n) ? n : null;
        }
        catch
        {
            return null;
        }
    }

    Console.WriteLine($"Endpoint: {searchEp}");
    var r = await CountAsync(rulingIdx);
    var c = await CountAsync(chunkIdx);
    Console.WriteLine($"  {rulingIdx,-26} {(r.HasValue ? r.Value.ToString() : "ERROR")} docs");
    Console.WriteLine($"  {chunkIdx,-26} {(c.HasValue ? c.Value.ToString() : "ERROR")} docs");
    if (!string.IsNullOrWhiteSpace(statuteIdx))
    {
        var s = await CountAsync(statuteIdx);
        Console.WriteLine($"  {statuteIdx,-26} {(s.HasValue ? s.Value.ToString() : "ERROR")} docs");
    }

    return 0;
}

var pipelineQueuePrefix = Environment.GetEnvironmentVariable("Pipeline__QueuePrefix") ?? "pipeline";
var pipelineQueueNames = new PipelineQueueNames(pipelineQueuePrefix);

// monitor — poll queue lengths + index doc counts every N seconds
if (args.Length > 0 && args[0].Equals("monitor", StringComparison.OrdinalIgnoreCase))
{
    var intervalSec = args.Length > 1 && int.TryParse(args[1], out var s) ? s : 15;
    var storageConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
    var searchEp = Environment.GetEnvironmentVariable("AzureSearch__Endpoint")?.TrimEnd('/');
    var searchApiKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
    var rulingIdx = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";
    var chunkIdx = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName") ?? "rulings-by-chunk";

    if (string.IsNullOrWhiteSpace(storageConn) || string.IsNullOrWhiteSpace(searchApiKey))
    {
        Console.WriteLine("ERROR: AzureBlob__ConnectionString and AzureSearch__ApiKey are required.");
        return 1;
    }

    var mainQueueNames = pipelineQueueNames.AllMain;
    var dlqNameList = pipelineQueueNames.AllDlq;
    var queueClients = mainQueueNames.Select(n => new QueueClient(storageConn, n)).ToArray();
    var dlqClients = dlqNameList.Select(n => new QueueClient(storageConn, n)).ToArray();
    using var http = new HttpClient();
    http.DefaultRequestHeaders.Add("api-key", searchApiKey);

    Console.WriteLine($"Monitoring pipeline (refresh every {intervalSec}s). Press Ctrl+C to stop.\n");

    while (true)
    {
        var now = DateTime.Now.ToString("HH:mm:ss");
        Console.WriteLine($"--- {now} ---");

        Console.WriteLine("  Queues:");
        for (var qi = 0; qi < queueClients.Length; qi++)
        {
            try
            {
                var props = await queueClients[qi].GetPropertiesAsync();
                var count = props.Value.ApproximateMessagesCount;
                var label = mainQueueNames[qi].Replace($"{pipelineQueueNames.Prefix}-", "", StringComparison.Ordinal);
                Console.Write($"    {label,-14} {count,6} msgs");

                try
                {
                    var dlqProps = await dlqClients[qi].GetPropertiesAsync();
                    var dlqCount = dlqProps.Value.ApproximateMessagesCount;
                    if (dlqCount > 0) Console.Write($"  (DLQ: {dlqCount})");
                }
                catch { /* DLQ may not exist */ }

                Console.WriteLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    {mainQueueNames[qi]}: ERROR - {ex.Message}");
            }
        }

        Console.WriteLine("  Search indexes:");
        foreach (var idx in new[] { rulingIdx, chunkIdx })
        {
            try
            {
                var resp = await http.GetAsync($"{searchEp}/indexes/{idx}/docs/$count?api-version=2024-07-01");
                var body = await resp.Content.ReadAsStringAsync();
                Console.WriteLine($"    {idx,-25} {body.Trim(),8} docs");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    {idx}: ERROR - {ex.Message}");
            }
        }

        Console.WriteLine();
        await Task.Delay(intervalSec * 1000);
    }
}

// dlq [parser|crawler|enrichment|indexer] — read DLQ messages for debugging
if (args.Length > 0 && args[0].Equals("dlq", StringComparison.OrdinalIgnoreCase))
{
    var queueName = args.Length > 1
        ? pipelineQueueNames.DlqFor(args[1].ToLowerInvariant())
        : pipelineQueueNames.ParserDlq;
    var connStr = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
    if (string.IsNullOrWhiteSpace(connStr))
    {
        Console.WriteLine("AzureBlob__ConnectionString not set. Run with load-env.ps1.");
        return 1;
    }
    var client = new QueueClient(connStr, queueName);
    if (!await client.ExistsAsync())
    {
        Console.WriteLine($"Queue {queueName} does not exist.");
        return 0;
    }
    var response = await client.PeekMessagesAsync(32);
    var messages = response.Value;
    Console.WriteLine($"DLQ ({queueName}): {messages.Length} messages\n");
    foreach (var m in messages)
    {
        var body = m.Body?.ToString() ?? "";
        Console.WriteLine($"--- Message {m.MessageId} (DequeueCount={m.DequeueCount}) ---");
        Console.WriteLine(body.Length > 2000 ? body[..2000] + "\n... (truncated)" : body);
        Console.WriteLine();
    }
    return 0;
}

var results = new List<(string Service, bool Ok, string Message)>();

// 1. Azure SQL
var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString");
if (string.IsNullOrWhiteSpace(sqlConn) || IsPlaceholder(sqlConn))
{
    results.Add(("Azure SQL", false, "Connection string no configurada"));
}
else
{
    try
    {
        await using var conn = new SqlConnection(sqlConn);
        await conn.OpenAsync();
        results.Add(("Azure SQL", true, "OK"));
    }
    catch (Exception ex)
    {
        results.Add(("Azure SQL", false, ex.Message));
    }
}

// 2. Azure Blob
var blobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
var containerName = Environment.GetEnvironmentVariable("AzureBlob__ContainerName") ?? "rulings-pdfs";
if (string.IsNullOrWhiteSpace(blobConn) || IsPlaceholder(blobConn))
{
    results.Add(("Azure Blob", false, "Connection string no configurada"));
}
else
{
    try
    {
        var blobClient = new BlobServiceClient(blobConn);
        var container = blobClient.GetBlobContainerClient(containerName);
        var exists = await container.ExistsAsync();
        results.Add(("Azure Blob", exists.Value, exists.Value ? "OK" : $"Container '{containerName}' no existe"));
    }
    catch (Exception ex)
    {
        results.Add(("Azure Blob", false, ex.Message));
    }
}

// 3. Storage Queues
var queues = pipelineQueueNames.AllMain;
if (string.IsNullOrWhiteSpace(blobConn) || IsPlaceholder(blobConn))
{
    results.Add(("Storage Queues", false, "Connection string no configurada"));
}
else
{
    try
    {
        var queueClient = new QueueServiceClient(blobConn);
        var missing = new List<string>();
        foreach (var q in queues)
        {
            var qc = queueClient.GetQueueClient(q);
            if (!await qc.ExistsAsync()) missing.Add(q);
        }
        results.Add(("Storage Queues", missing.Count == 0, missing.Count == 0 ? "OK (4 colas)" : $"Faltan: {string.Join(", ", missing)}"));
    }
    catch (Exception ex)
    {
        results.Add(("Storage Queues", false, ex.Message));
    }
}

// 4. Azure AI Search
var searchEndpoint = Environment.GetEnvironmentVariable("AzureSearch__Endpoint")?.TrimEnd('/');
var searchKey = Environment.GetEnvironmentVariable("AzureSearch__ApiKey");
var rulingIndex = Environment.GetEnvironmentVariable("AzureSearch__RulingIndexName") ?? "rulings-by-ruling";
var chunkIndex = Environment.GetEnvironmentVariable("AzureSearch__ChunkIndexName") ?? "rulings-by-chunk";
if (string.IsNullOrWhiteSpace(searchKey) || IsPlaceholder(searchKey))
{
    results.Add(("Azure AI Search", false, "ApiKey no configurada"));
}
else if (string.IsNullOrWhiteSpace(searchEndpoint))
{
    results.Add(("Azure AI Search", false, "Endpoint no configurado"));
}
else
{
    try
    {
        using var http = new HttpClient();
        http.DefaultRequestHeaders.Add("api-key", searchKey);
        var r1 = await http.GetAsync($"{searchEndpoint}/indexes/{rulingIndex}?api-version=2024-07-01");
        var r2 = await http.GetAsync($"{searchEndpoint}/indexes/{chunkIndex}?api-version=2024-07-01");
        var ok = r1.IsSuccessStatusCode && r2.IsSuccessStatusCode;
        results.Add(("Azure AI Search", ok, ok ? "OK (ambos índices)" : $"rulings: {r1.StatusCode}, chunks: {r2.StatusCode}"));
    }
    catch (Exception ex)
    {
        results.Add(("Azure AI Search", false, ex.Message));
    }
}

// Output
foreach (var (service, ok, msg) in results)
{
    var status = ok ? "OK" : "FAIL";
    Console.WriteLine($"{service}|{status}|{msg}");
}

var exitCode = results.All(r => r.Ok) ? 0 : 1;
return exitCode;

static bool IsPlaceholder(string? v) =>
    string.IsNullOrWhiteSpace(v) ||
    v is "DB_SECRET" or "STORAGE_KEY" or "SEARCH_KEY" or "OPENAI_KEY";

static string[] BenchmarkSnapshotColumnNames(SqlDataReader reader)
{
    var names = new string[reader.FieldCount];
    for (var i = 0; i < reader.FieldCount; i++)
        names[i] = reader.GetName(i);
    return names;
}

static string? FindRepoRootWithEnvFile()
{
    var dir = Directory.GetCurrentDirectory();
    while (dir is not null)
    {
        if (File.Exists(Path.Combine(dir, ".env")))
            return dir;
        dir = Directory.GetParent(dir)?.FullName;
    }

    return null;
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
