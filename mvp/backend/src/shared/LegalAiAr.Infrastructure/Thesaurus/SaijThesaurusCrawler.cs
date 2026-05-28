using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Thesaurus;

/// <summary>
/// Crawls the SAIJ TemaTres API to ingest the complete thesaurus into the repository.
/// Two-phase approach: (1) crawl all terms, (2) build relations.
/// </summary>
public sealed class SaijThesaurusCrawler : IThesaurusCrawler
{
    private const string BaseUrl = "http://vocabularios.saij.gob.ar/saij/services.php";
    private static readonly TimeSpan ThrottleDelay = TimeSpan.FromMilliseconds(150);

    private readonly HttpClient _http;
    private readonly IThesaurusRepository _repo;
    private readonly ILogger<SaijThesaurusCrawler> _logger;

    private readonly List<(int ChildExtId, int ParentExtId)> _pendingHierarchy = new();
    private readonly List<(int PreferredExtId, int AltExtId, string AltLabel)> _pendingAlt = new();
    private readonly List<(int SourceExtId, int TargetExtId)> _pendingRelated = new();

    private int _termsProcessed;

    public SaijThesaurusCrawler(HttpClient http, IThesaurusRepository repo, ILogger<SaijThesaurusCrawler> logger)
    {
        _http = http;
        _repo = repo;
        _logger = logger;
    }

    public async Task CrawlAsync(Action<string>? onProgress = null, CancellationToken cancellationToken = default)
    {
        // --- Phase 1: Crawl all preferred terms from the hierarchy ---
        onProgress?.Invoke("Phase 1: Crawling hierarchy (preferred terms)...");

        var topTerms = await FetchTopTermsAsync(cancellationToken);
        onProgress?.Invoke($"Found {topTerms.Count} branches.");

        foreach (var (extId, label) in topTerms)
        {
            cancellationToken.ThrowIfCancellationRequested();
            onProgress?.Invoke($"  Branch: {label}");

            await UpsertPreferredTermAsync(extId, label, label, depth: 0, cancellationToken);
            await CrawlChildrenAsync(extId, label, depth: 1, onProgress, cancellationToken);
        }

        await _repo.SaveChangesAsync(cancellationToken);
        onProgress?.Invoke($"Phase 1 complete: {_termsProcessed} preferred terms saved.");

        // --- Phase 2: Fetch alt terms (synonyms) and related terms for each preferred term ---
        onProgress?.Invoke("Phase 2: Fetching synonyms and related terms...");
        var allExternalIds = await _repo.GetAllExternalIdsAsync(cancellationToken);
        onProgress?.Invoke($"  {allExternalIds.Count} terms to process for synonyms/related.");

        var batch = 0;
        foreach (var extId in allExternalIds)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await FetchAltTermsAsync(extId, cancellationToken);
            await FetchRelatedTermsAsync(extId, cancellationToken);

            batch++;
            if (batch % 200 == 0)
            {
                onProgress?.Invoke($"  {batch}/{allExternalIds.Count} processed...");
            }
        }

        // Insert alt terms (non-preferred) as terms first
        foreach (var (_, altExtId, altLabel) in _pendingAlt)
        {
            await UpsertNonPreferredTermAsync(altExtId, altLabel, cancellationToken);
        }
        await _repo.SaveChangesAsync(cancellationToken);
        onProgress?.Invoke($"Phase 2 complete: {_pendingAlt.Count} synonyms, {_pendingRelated.Count} related pairs found.");

        // --- Phase 3: Build all relations using the full ExternalId → DbId map ---
        onProgress?.Invoke("Phase 3: Building relations...");
        var extToDb = await _repo.GetExternalIdToDbIdMapAsync(cancellationToken);
        var relationsCreated = 0;

        foreach (var (childExt, parentExt) in _pendingHierarchy)
        {
            if (extToDb.TryGetValue(childExt, out var childDb) && extToDb.TryGetValue(parentExt, out var parentDb))
            {
                await _repo.UpsertRelationAsync(childDb, parentDb, ThesaurusRelationType.BT, cancellationToken);
                await _repo.UpsertRelationAsync(parentDb, childDb, ThesaurusRelationType.NT, cancellationToken);
                relationsCreated += 2;
            }
        }

        foreach (var (prefExt, altExt, _) in _pendingAlt)
        {
            if (extToDb.TryGetValue(prefExt, out var prefDb) && extToDb.TryGetValue(altExt, out var altDb))
            {
                await _repo.UpsertRelationAsync(prefDb, altDb, ThesaurusRelationType.UF, cancellationToken);
                relationsCreated++;
            }
        }

        foreach (var (srcExt, tgtExt) in _pendingRelated)
        {
            if (extToDb.TryGetValue(srcExt, out var srcDb) && extToDb.TryGetValue(tgtExt, out var tgtDb))
            {
                await _repo.UpsertRelationAsync(srcDb, tgtDb, ThesaurusRelationType.RT, cancellationToken);
                relationsCreated++;
            }
        }

        await _repo.SaveChangesAsync(cancellationToken);

        var totalTerms = await _repo.GetTermCountAsync(cancellationToken);
        var totalRelations = await _repo.GetRelationCountAsync(cancellationToken);
        onProgress?.Invoke($"Done. Total: {totalTerms} terms, {totalRelations} relations.");
    }

    private async Task CrawlChildrenAsync(int parentExtId, string branchName, int depth, Action<string>? onProgress, CancellationToken ct)
    {
        var children = await FetchDownAsync(parentExtId, ct);
        if (children.Count == 0) return;

        foreach (var (childExtId, childLabel, hasMoreDown) in children)
        {
            ct.ThrowIfCancellationRequested();

            await UpsertPreferredTermAsync(childExtId, childLabel, branchName, depth, ct);
            _pendingHierarchy.Add((childExtId, parentExtId));

            if (_termsProcessed % 200 == 0)
            {
                await _repo.SaveChangesAsync(ct);
                onProgress?.Invoke($"    {_termsProcessed} terms crawled...");
            }

            if (hasMoreDown)
                await CrawlChildrenAsync(childExtId, branchName, depth + 1, onProgress, ct);
        }
    }

    private async Task FetchAltTermsAsync(int extId, CancellationToken ct)
    {
        var alts = await FetchAltAsync(extId, ct);
        foreach (var (altExtId, altLabel) in alts)
            _pendingAlt.Add((extId, altExtId, altLabel));
    }

    private async Task FetchRelatedTermsAsync(int extId, CancellationToken ct)
    {
        var related = await FetchRelatedAsync(extId, ct);
        foreach (var (relExtId, _) in related)
            _pendingRelated.Add((extId, relExtId));
    }

    private async Task UpsertPreferredTermAsync(int extId, string label, string branchName, int depth, CancellationToken ct)
    {
        await _repo.UpsertTermAsync(new ThesaurusTerm
        {
            ExternalId = extId,
            Label = label,
            IsPreferred = true,
            BranchName = branchName,
            Depth = depth
        }, ct);
        _termsProcessed++;
    }

    private async Task UpsertNonPreferredTermAsync(int extId, string label, CancellationToken ct)
    {
        await _repo.UpsertTermAsync(new ThesaurusTerm
        {
            ExternalId = extId,
            Label = label,
            IsPreferred = false,
            BranchName = null,
            Depth = 0
        }, ct);
    }

    // --- TemaTres API calls ---

    private async Task<List<(int ExtId, string Label)>> FetchTopTermsAsync(CancellationToken ct)
    {
        var json = await CallApiAsync("fetchTopTerms", null, ct);
        var result = new List<(int, string)>();
        if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("result", out var resultObj) && resultObj.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in resultObj.EnumerateObject())
            {
                if (int.TryParse(GetString(prop.Value, "term_id"), out var id))
                    result.Add((id, GetString(prop.Value, "string")));
            }
        }
        return result;
    }

    private async Task<List<(int ExtId, string Label, bool HasMoreDown)>> FetchDownAsync(int termId, CancellationToken ct)
    {
        var json = await CallApiAsync("fetchDown", termId.ToString(), ct);
        var result = new List<(int, string, bool)>();
        if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("result", out var resultObj) && resultObj.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in resultObj.EnumerateObject())
            {
                if (int.TryParse(GetString(prop.Value, "term_id"), out var id))
                {
                    var hasMore = GetString(prop.Value, "hasMoreDown") == "1";
                    result.Add((id, GetString(prop.Value, "string"), hasMore));
                }
            }
        }
        return result;
    }

    private async Task<List<(int ExtId, string Label)>> FetchAltAsync(int termId, CancellationToken ct)
    {
        var json = await CallApiAsync("fetchAlt", termId.ToString(), ct);
        return ParseSimpleTermList(json);
    }

    private async Task<List<(int ExtId, string Label)>> FetchRelatedAsync(int termId, CancellationToken ct)
    {
        var json = await CallApiAsync("fetchRelated", termId.ToString(), ct);
        return ParseSimpleTermList(json);
    }

    private static List<(int ExtId, string Label)> ParseSimpleTermList(JsonElement json)
    {
        var result = new List<(int, string)>();
        if (json.ValueKind == JsonValueKind.Object && json.TryGetProperty("result", out var resultObj) && resultObj.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in resultObj.EnumerateObject())
            {
                if (int.TryParse(GetString(prop.Value, "term_id"), out var id))
                    result.Add((id, GetString(prop.Value, "string")));
            }
        }
        return result;
    }

    private async Task<JsonElement> CallApiAsync(string task, string? arg, CancellationToken ct)
    {
        var url = $"{BaseUrl}?task={task}&output=json";
        if (!string.IsNullOrEmpty(arg))
            url += $"&arg={Uri.EscapeDataString(arg)}";

        const int maxRetries = 3;
        for (var attempt = 1; attempt <= maxRetries; attempt++)
        {
            await Task.Delay(ThrottleDelay, ct);
            try
            {
                using var response = await _http.GetAsync(url, ct);
                response.EnsureSuccessStatusCode();
                var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync(ct), cancellationToken: ct);
                return doc.RootElement.Clone();
            }
            catch (Exception ex) when (attempt < maxRetries)
            {
                _logger.LogWarning("API call attempt {Attempt} failed: {Task} arg={Arg} — {Error}. Retrying...",
                    attempt, task, arg, ex.Message);
                await Task.Delay(TimeSpan.FromSeconds(attempt * 2), ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "API call failed after {MaxRetries} attempts: {Task} arg={Arg}", maxRetries, task, arg);
                return default;
            }
        }
        return default;
    }

    private static string GetString(JsonElement element, string property)
    {
        if (!element.TryGetProperty(property, out var val))
            return string.Empty;

        return val.ValueKind switch
        {
            JsonValueKind.String => val.GetString() ?? string.Empty,
            JsonValueKind.Number => val.GetRawText(),
            JsonValueKind.True => "1",
            JsonValueKind.False => "0",
            JsonValueKind.Null => string.Empty,
            _ => val.GetRawText()
        };
    }
}
