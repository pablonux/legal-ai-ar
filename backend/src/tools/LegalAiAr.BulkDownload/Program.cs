// Bulk Download Orchestrator: enqueues crawl jobs year-by-year (2025 → 1994)
// to queue-crawler for CSJN acuerdo-based discovery (HTTP only, no Selenium).
// Each year becomes one by-range job; the CrawlerWorkerService handles the rest.
//
// Usage: dotnet run [--from-year <year>] [--to-year <year>] [--dry-run] [--method <acuerdo|sumarios>]
// Env: AzureBlob__ConnectionString (same Storage Account hosts queues)
//
// Example: dotnet run --from-year 2025 --to-year 1994
//   → Enqueues 32 jobs, one per year, newest first.

using System.Text;
using System.Text.Json;
using Azure.Storage.Queues;
using LegalAiAr.Core.Pipeline;
using Microsoft.Data.SqlClient;

LoadEnvFile();

var pipelineQueuePrefix = Environment.GetEnvironmentVariable("Pipeline__QueuePrefix") ?? "pipeline";
var queueNames = new PipelineQueueNames(pipelineQueuePrefix);

var knownFlags = new HashSet<string> { "--dry-run", "--from-year", "--to-year", "--method", "--month-split" };
var unknownFlags = args.Where(a => a.StartsWith("--") && !knownFlags.Contains(a)).ToList();
if (unknownFlags.Count > 0)
{
    Console.WriteLine($"Unknown flag(s): {string.Join(", ", unknownFlags)}");
    ShowUsage();
    return 1;
}

var dryRun = args.Contains("--dry-run");
var fromYear = int.TryParse(GetArgValue(args, "--from-year"), out var fy) ? fy : 2025;
var toYear = int.TryParse(GetArgValue(args, "--to-year"), out var ty) ? ty : 1994;
var method = GetArgValue(args, "--method") ?? "acuerdo";
var monthSplit = args.Contains("--month-split");

if (fromYear < toYear)
{
    Console.WriteLine("ERROR: --from-year must be >= --to-year (descends from recent to oldest).");
    return 1;
}

Console.WriteLine($"CSJN Bulk Download Orchestrator");
Console.WriteLine($"  Method:    {method}");
Console.WriteLine($"  Range:     {fromYear} → {toYear}");
Console.WriteLine($"  Month split: {monthSplit}");
Console.WriteLine($"  Dry run:   {dryRun}");
Console.WriteLine();

var blobConn = Environment.GetEnvironmentVariable("AzureBlob__ConnectionString");
if (string.IsNullOrWhiteSpace(blobConn) || blobConn.Contains("PLACEHOLDER"))
{
    Console.WriteLine("ERROR: AzureBlob__ConnectionString is not configured.");
    return 1;
}

var queueName = queueNames.Discoverer;
var queueClient = new QueueClient(blobConn, queueName, new QueueClientOptions
{
    MessageEncoding = QueueMessageEncoding.Base64
});
await queueClient.CreateIfNotExistsAsync();

var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

var totalJobs = 0;
var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

for (var year = fromYear; year >= toYear; year--)
{
    if (monthSplit)
    {
        for (var month = 12; month >= 1; month--)
        {
            var from = new DateOnly(year, month, 1);
            var to = from.AddMonths(1).AddDays(-1);

            if (to > DateOnly.FromDateTime(DateTime.UtcNow))
                to = DateOnly.FromDateTime(DateTime.UtcNow);

            if (from > DateOnly.FromDateTime(DateTime.UtcNow))
                continue;

            await EnqueueJobAsync(from, to, year, $"{year}-{month:D2}");
        }
    }
    else
    {
        var from = new DateOnly(year, 1, 1);
        var to = new DateOnly(year, 12, 31);

        if (to > DateOnly.FromDateTime(DateTime.UtcNow))
            to = DateOnly.FromDateTime(DateTime.UtcNow);

        await EnqueueJobAsync(from, to, year, year.ToString());
    }
}

Console.WriteLine($"\nDone. Enqueued {totalJobs} jobs to {queueName}.");
return 0;

async Task EnqueueJobAsync(DateOnly from, DateOnly to, int year, string label)
{
    var message = new
    {
        sourceId = 1,
        documentType = "ruling",
        type = "by-range",
        dateFrom = from.ToString("yyyy-MM-dd"),
        dateTo = to.ToString("yyyy-MM-dd"),
        useCache = false,
        reprocess = false
    };

    var json = JsonSerializer.Serialize(message, jsonOptions);

    if (dryRun)
    {
        Console.WriteLine($"  [DRY-RUN] {label}: {from:dd/MM/yyyy} → {to:dd/MM/yyyy}");
    }
    else
    {
        var existingJobs = sqlConn is not null
            ? await CountExistingJobsAsync(from, to)
            : 0;

        if (existingJobs > 0)
        {
            Console.WriteLine($"  [SKIP] {label}: {existingJobs} existing job(s) for this range");
            return;
        }

        await queueClient.SendMessageAsync(Convert.ToBase64String(Encoding.UTF8.GetBytes(json)));
        Console.WriteLine($"  [QUEUED] {label}: {from:dd/MM/yyyy} → {to:dd/MM/yyyy}");
    }

    totalJobs++;
}

async Task<int> CountExistingJobsAsync(DateOnly from, DateOnly to)
{
    if (string.IsNullOrWhiteSpace(sqlConn))
        return 0;

    try
    {
        await using var conn = new SqlConnection(sqlConn);
        await conn.OpenAsync();
        await using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT COUNT(*)
            FROM IngestionJobs
            WHERE SourceId = 1
              AND DateFrom = @from
              AND DateTo = @to
              AND Status IN ('running', 'success', 'pending')
            """;
        cmd.Parameters.AddWithValue("@from", from.ToDateTime(TimeOnly.MinValue));
        cmd.Parameters.AddWithValue("@to", to.ToDateTime(TimeOnly.MinValue));
        return (int)await cmd.ExecuteScalarAsync();
    }
    catch
    {
        return 0;
    }
}

static void ShowUsage()
{
    Console.WriteLine("""
        Usage: dotnet run [options]
          --from-year <year>   Start year (default: 2025, descends)
          --to-year <year>     End year (default: 1994)
          --method <name>      Discovery method: acuerdo (default) or sumarios
          --month-split        Split each year into monthly jobs
          --dry-run            Show what would be enqueued without sending
        """);
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
