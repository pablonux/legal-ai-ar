// Ingest SAIJ Thesaurus: crawls the TemaTres API and persists terms + relations to Azure SQL.
// Idempotent: upserts by ExternalId, safe to re-run.
//
// Usage: dotnet run [--stats | --normalize | --crawl (default)]
// Env: AzureSql__ConnectionString
// --normalize: warms EntityCache then links Keywords.ThesaurusTermId where missing (idempotent).

using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Thesaurus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

LoadEnvFile();

var statsOnly = args.Contains("--stats");
var normalizeOnly = args.Contains("--normalize");

var config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .Build();

var services = new ServiceCollection();
services.AddSingleton<IConfiguration>(config);
services.AddLegalAiArInfrastructure(config);
services.AddHttpClient<SaijThesaurusCrawler>();
services.AddTransient<IThesaurusCrawler, SaijThesaurusCrawler>();
services.AddLogging(b =>
{
    b.AddConsole().SetMinimumLevel(LogLevel.Warning);
    b.AddFilter("LegalAiAr", LogLevel.Information);
});

var sp = services.BuildServiceProvider();

if (statsOnly)
{
    using var scope = sp.CreateScope();
    var repo = scope.ServiceProvider.GetRequiredService<IThesaurusRepository>();
    var terms = await repo.GetTermCountAsync();
    var relations = await repo.GetRelationCountAsync();
    Console.WriteLine($"Thesaurus stats: {terms} terms, {relations} relations");
    return 0;
}

if (normalizeOnly)
{
    Console.WriteLine("=== Keyword Normalization (backfill) ===");
    Console.WriteLine();
    try
    {
        using var scope = sp.CreateScope();
        var cache = scope.ServiceProvider.GetRequiredService<EntityCacheService>();
        Console.WriteLine("Warming entity cache (thesaurus + keywords) for normalized lookups…");
        await cache.WarmUpAsync(scope.ServiceProvider, CancellationToken.None);

        var normalizer = scope.ServiceProvider.GetRequiredService<IKeywordNormalizationService>();

        var (matched, total) = await normalizer.NormalizeAllAsync(
            onProgress: msg => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}"),
            cancellationToken: CancellationToken.None);

        Console.WriteLine();
        Console.WriteLine($"Normalization complete: {matched}/{total} keywords linked to thesaurus descriptors.");
        return 0;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine(ex.StackTrace);
        return 1;
    }
}

Console.WriteLine("=== SAIJ Thesaurus Ingestion ===");
Console.WriteLine();

try
{
    using var scope = sp.CreateScope();
    var crawler = scope.ServiceProvider.GetRequiredService<IThesaurusCrawler>();

    await crawler.CrawlAsync(
        onProgress: msg => Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}"),
        cancellationToken: CancellationToken.None);

    Console.WriteLine();
    Console.WriteLine("Ingestion completed successfully.");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    Console.WriteLine(ex.StackTrace);
    return 1;
}

static void LoadEnvFile()
{
    var dir = new DirectoryInfo(Directory.GetCurrentDirectory());
    while (dir != null)
    {
        var envFile = Path.Combine(dir.FullName, ".env");
        if (File.Exists(envFile))
        {
            foreach (var line in File.ReadAllLines(envFile))
            {
                var trimmed = line.Trim();
                if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#')) continue;
                var eqIndex = trimmed.IndexOf('=');
                if (eqIndex <= 0) continue;
                var key = trimmed[..eqIndex].Trim();
                var value = trimmed[(eqIndex + 1)..].Trim().Trim('"');
                Environment.SetEnvironmentVariable(key, value);
            }
            return;
        }
        dir = dir.Parent;
    }
}
