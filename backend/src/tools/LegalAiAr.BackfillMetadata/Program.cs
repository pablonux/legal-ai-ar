// Backfill Metadata Tool: updates CSJN rulings that have NULL metadata fields
// by re-fetching from the CSJN sjconsulta API (abrirAnalisis.html).
//
// Usage: dotnet run [--dry-run] [--limit <N>] [--delay-ms <ms>]
// Env: AzureSql__ConnectionString or ConnectionStrings__DefaultConnection
//
// Scope: Rulings with SourceId=1 and (SubjectArea IS NULL OR Jurisdiction IS NULL
//        OR ResourceType IS NULL OR AnalysisId IS NULL).
// Requirement: ruling must have AnalysisId in DB (or ExternalId as fallback).

using System.Globalization;
using System.Net;
using System.Text.Json;
using Microsoft.Data.SqlClient;

LoadEnvFile();

var knownFlags = new HashSet<string> { "--dry-run", "--limit", "--delay-ms", "--keywords" };
var unknownFlags = args.Where(a => a.StartsWith("--") && !knownFlags.Contains(a)).ToList();
if (unknownFlags.Count > 0)
{
    Console.WriteLine($"Unknown flag(s): {string.Join(", ", unknownFlags)}");
    ShowUsage();
    return 1;
}

var dryRun = args.Contains("--dry-run");
var keywordsMode = args.Contains("--keywords");
var limit = int.TryParse(GetArgValue(args, "--limit"), out var lim) ? lim : 0;
var delayMs = int.TryParse(GetArgValue(args, "--delay-ms"), out var dm) ? dm : (keywordsMode ? 500 : 1200);

Console.WriteLine(keywordsMode ? "CSJN Keywords Backfill Tool" : "CSJN Metadata Backfill Tool");
Console.WriteLine($"  Mode:      {(keywordsMode ? "keywords" : "metadata")}");
Console.WriteLine($"  Dry run:   {dryRun}");
Console.WriteLine($"  Limit:     {(limit > 0 ? limit.ToString() : "all")}");
Console.WriteLine($"  Delay:     {delayMs}ms between API calls");
Console.WriteLine();

var sqlConn = Environment.GetEnvironmentVariable("AzureSql__ConnectionString")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

if (string.IsNullOrWhiteSpace(sqlConn) || sqlConn.Contains("PLACEHOLDER"))
{
    Console.WriteLine("ERROR: SQL connection string is not configured.");
    return 1;
}

const string csjnBaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta";

if (keywordsMode)
    return await RunKeywordsBackfillAsync(sqlConn, csjnBaseUrl, limit, delayMs, dryRun);

var incompleteRulings = await LoadIncompleteRulingsAsync(sqlConn, limit);
Console.WriteLine($"Found {incompleteRulings.Count} rulings with incomplete metadata.");

var withAnalysisId = incompleteRulings.Where(r => !string.IsNullOrWhiteSpace(r.AnalysisId)).ToList();
var withoutAnalysisId = incompleteRulings.Where(r => string.IsNullOrWhiteSpace(r.AnalysisId)).ToList();

Console.WriteLine($"  With AnalysisId:    {withAnalysisId.Count} (will backfill)");
Console.WriteLine($"  Without AnalysisId: {withoutAnalysisId.Count} (skipped — need re-crawl)");
Console.WriteLine();

if (withoutAnalysisId.Count > 0)
{
    Console.WriteLine("Rulings without AnalysisId (first 20):");
    foreach (var r in withoutAnalysisId.Take(20))
        Console.WriteLine($"  Id={r.Id} ExternalId={r.ExternalId} Date={r.RulingDate}");
    Console.WriteLine();
}

if (withAnalysisId.Count == 0)
{
    Console.WriteLine("Nothing to backfill.");
    return 0;
}

using var httpClient = new HttpClient(new HttpClientHandler
{
    CookieContainer = new CookieContainer(),
    UseCookies = true,
    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
});
httpClient.DefaultRequestHeaders.Add("User-Agent", "LegalAiAr-Backfill/1.0");
httpClient.Timeout = TimeSpan.FromSeconds(30);

var updated = 0;
var errors = 0;
var skipped = 0;

foreach (var (idx, ruling) in withAnalysisId.Select((r, i) => (i, r)))
{
    var progress = $"[{idx + 1}/{withAnalysisId.Count}]";
    try
    {
        var url = $"{csjnBaseUrl}/fallos/abrirAnalisis.html?idAnalisis={Uri.EscapeDataString(ruling.AnalysisId!)}";
        var response = await httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"  {progress} SKIP {ruling.ExternalId}: HTTP {(int)response.StatusCode}");
            skipped++;
            await Task.Delay(delayMs);
            continue;
        }

        var json = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(json))
        {
            Console.WriteLine($"  {progress} SKIP {ruling.ExternalId}: empty response");
            skipped++;
            await Task.Delay(delayMs);
            continue;
        }

        using var doc = JsonDocument.Parse(json);
        var el = doc.RootElement;

        var jurisdiction = GetString(el, "jurisdiccion", "jurisdiction")
            ?? GetStringFromValorObject(el, "competencia");
        var resourceType = GetString(el, "tipoRecurso", "resourceType")
            ?? GetStringFromValorObject(el, "tipoRecurso");
        var subjectArea = StripLegacyPrefix(
            GetString(el, "materia", "subjectArea")
            ?? GetStringFromValorObject(el, "materiaSecretaria"));
        var rulingDirection = GetString(el, "sentido", "rulingDirection")
            ?? GetStringFromValorObject(el, "sentidoPronunciamiento");
        var isUnconstitutional = GetBool(el, "inconstitucional", "isUnconstitutional");

        var hasChanges = false;
        var changes = new List<string>();

        if (!string.IsNullOrEmpty(jurisdiction) && ruling.Jurisdiction == null)
        { changes.Add($"Jurisdiction={jurisdiction}"); hasChanges = true; }
        if (!string.IsNullOrEmpty(jurisdiction) && ruling.JurisdictionArea != jurisdiction)
        { changes.Add($"JurisdictionArea={jurisdiction}"); hasChanges = true; }
        if (!string.IsNullOrEmpty(resourceType) && ruling.ResourceType == null)
        { changes.Add($"ResourceType={resourceType}"); hasChanges = true; }
        if (!string.IsNullOrEmpty(subjectArea) && ruling.SubjectArea == null)
        { changes.Add($"SubjectArea={subjectArea}"); hasChanges = true; }
        if (!string.IsNullOrEmpty(rulingDirection) && ruling.RulingDirection == null)
        { changes.Add($"RulingDirection={rulingDirection}"); hasChanges = true; }
        if (isUnconstitutional && !ruling.IsUnconstitutional)
        { changes.Add("IsUnconstitutional=true"); hasChanges = true; }

        if (!hasChanges)
        {
            Console.WriteLine($"  {progress} NOOP {ruling.ExternalId}: API returned no new data");
            skipped++;
        }
        else if (dryRun)
        {
            Console.WriteLine($"  {progress} DRY-RUN {ruling.ExternalId}: {string.Join(", ", changes)}");
            updated++;
        }
        else
        {
            await UpdateRulingAsync(sqlConn, ruling.Id, jurisdiction, resourceType,
                subjectArea, rulingDirection, isUnconstitutional, jurisdiction);
            Console.WriteLine($"  {progress} UPDATED {ruling.ExternalId}: {string.Join(", ", changes)}");
            updated++;
        }

        await Task.Delay(delayMs);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"  {progress} ERROR {ruling.ExternalId}: {ex.Message}");
        errors++;
        await Task.Delay(delayMs * 2);
    }
}

Console.WriteLine();
Console.WriteLine($"Done. Updated={updated} Skipped={skipped} Errors={errors}");
return errors > 0 ? 1 : 0;

// ── Keywords backfill ───────────────────────────────────────

async Task<int> RunKeywordsBackfillAsync(string connStr, string baseUrl, int maxRows, int delay, bool dry)
{
    using var http = new HttpClient(new HttpClientHandler
    {
        CookieContainer = new CookieContainer(),
        UseCookies = true,
        AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
    });
    http.DefaultRequestHeaders.Add("User-Agent", "LegalAiAr-Backfill/1.0");
    http.Timeout = TimeSpan.FromSeconds(30);

    var rulings = await LoadRulingsWithoutKeywordsAsync(connStr, maxRows);
    Console.WriteLine($"Found {rulings.Count} rulings without keywords.");
    if (rulings.Count == 0) { Console.WriteLine("Nothing to backfill."); return 0; }

    var updatedCount = 0;
    var errCount = 0;

    foreach (var (idx, r) in rulings.Select((v, i) => (i, v)))
    {
        var progress = $"[{idx + 1}/{rulings.Count}]";
        try
        {
            var url = $"{baseUrl}/fallos/abrirAnalisis.html?idAnalisis={Uri.EscapeDataString(r.AnalysisId)}";
            var resp = await http.GetAsync(url);
            if (!resp.IsSuccessStatusCode) { Console.WriteLine($"  {progress} SKIP {r.ExternalId}: HTTP {(int)resp.StatusCode}"); await Task.Delay(delay); continue; }

            var json = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var voces = ParseVocesFromJson(doc.RootElement);

            if (voces.Count == 0)
            {
                Console.WriteLine($"  {progress} NOOP {r.ExternalId}: no voces");
                await Task.Delay(delay);
                continue;
            }

            if (dry)
            {
                Console.WriteLine($"  {progress} DRY-RUN {r.ExternalId}: {voces.Count} voces ({string.Join(", ", voces.Select(v => v.Desc).Take(5))})");
            }
            else
            {
                await PersistKeywordsAsync(connStr, r.RulingId, voces);
                Console.WriteLine($"  {progress} UPDATED {r.ExternalId}: {voces.Count} voces");
            }

            updatedCount++;
            await Task.Delay(delay);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  {progress} ERROR {r.ExternalId}: {ex.Message}");
            errCount++;
            await Task.Delay(delay * 2);
        }
    }

    Console.WriteLine();
    Console.WriteLine($"Done. Updated={updatedCount} Errors={errCount}");
    return errCount > 0 ? 1 : 0;
}

async Task<List<(Guid RulingId, string ExternalId, string AnalysisId)>> LoadRulingsWithoutKeywordsAsync(string connStr, int maxRows)
{
    var results = new List<(Guid, string, string)>();
    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = $"""
        SELECT {(maxRows > 0 ? $"TOP {maxRows}" : "")}
               r.Id, r.ExternalId, r.AnalysisId
        FROM Rulings r
        WHERE r.SourceId = 1
          AND r.AnalysisId IS NOT NULL
          AND NOT EXISTS (SELECT 1 FROM RulingKeywords rk WHERE rk.RulingId = r.Id)
        ORDER BY r.RulingDate DESC
        """;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
        results.Add((reader.GetGuid(0), reader.GetString(1), reader.GetString(2)));
    return results;
}

static List<(int Code, string Desc)> ParseVocesFromJson(JsonElement el)
{
    var list = new List<(int, string)>();
    if (el.ValueKind != JsonValueKind.Object) return list;
    if (!el.TryGetProperty("voces", out var voces) || voces.ValueKind != JsonValueKind.Array) return list;

    foreach (var item in voces.EnumerateArray())
    {
        if (item.ValueKind != JsonValueKind.Object) continue;
        if (!item.TryGetProperty("tipoVoz", out var tv) || tv.ValueKind != JsonValueKind.Object) continue;
        var code = tv.TryGetProperty("codigoValor", out var cv) && cv.TryGetInt32(out var c) ? c : 0;
        var desc = tv.TryGetProperty("valor", out var dv) ? dv.GetString()?.Trim() : null;
        if (desc != null)
            list.Add((code, desc));
    }
    return list;
}

async Task PersistKeywordsAsync(string connStr, Guid rulingId, List<(int Code, string Desc)> voces)
{
    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();

    var order = 0;
    foreach (var (code, desc) in voces)
    {
        await using var mergeCmd = conn.CreateCommand();
        mergeCmd.CommandText = """
            MERGE Keywords AS target
            USING (SELECT @code AS ExternalCode, @desc AS Description) AS source
            ON target.ExternalCode = source.ExternalCode
            WHEN NOT MATCHED THEN INSERT (ExternalCode, Description) VALUES (source.ExternalCode, source.Description)
            OUTPUT inserted.Id;
            """;
        mergeCmd.Parameters.AddWithValue("@code", code);
        mergeCmd.Parameters.AddWithValue("@desc", desc);

        int keywordId;
        var result = await mergeCmd.ExecuteScalarAsync();
        if (result != null && result != DBNull.Value)
        {
            keywordId = (int)result;
        }
        else
        {
            await using var selectCmd = conn.CreateCommand();
            selectCmd.CommandText = "SELECT Id FROM Keywords WHERE ExternalCode = @code";
            selectCmd.Parameters.AddWithValue("@code", code);
            keywordId = (int)(await selectCmd.ExecuteScalarAsync())!;
        }

        await using var linkCmd = conn.CreateCommand();
        linkCmd.CommandText = """
            IF NOT EXISTS (SELECT 1 FROM RulingKeywords WHERE RulingId = @rid AND KeywordId = @kid)
            INSERT INTO RulingKeywords (RulingId, KeywordId, SortOrder) VALUES (@rid, @kid, @order)
            """;
        linkCmd.Parameters.AddWithValue("@rid", rulingId);
        linkCmd.Parameters.AddWithValue("@kid", keywordId);
        linkCmd.Parameters.AddWithValue("@order", order++);
        await linkCmd.ExecuteNonQueryAsync();
    }
}

// ── DB queries ──────────────────────────────────────────────

async Task<List<IncompleteRuling>> LoadIncompleteRulingsAsync(string connStr, int maxRows)
{
    var results = new List<IncompleteRuling>();
    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = $"""
        SELECT {(maxRows > 0 ? $"TOP {maxRows}" : "")}
               Id, ExternalId, AnalysisId, RulingDate,
               Jurisdiction, ResourceType, SubjectArea, RulingDirection, IsUnconstitutional,
               JurisdictionArea
        FROM Rulings
        WHERE SourceId = 1
          AND (SubjectArea IS NULL OR Jurisdiction IS NULL
               OR ResourceType IS NULL OR AnalysisId IS NULL
               OR JurisdictionArea = SubjectArea)
        ORDER BY RulingDate DESC
        """;
    await using var reader = await cmd.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        results.Add(new IncompleteRuling
        {
            Id = reader.GetGuid(0),
            ExternalId = reader.GetString(1),
            AnalysisId = reader.IsDBNull(2) ? null : reader.GetString(2),
            RulingDate = DateOnly.FromDateTime(reader.GetDateTime(3)),
            Jurisdiction = reader.IsDBNull(4) ? null : reader.GetString(4),
            ResourceType = reader.IsDBNull(5) ? null : reader.GetString(5),
            SubjectArea = reader.IsDBNull(6) ? null : reader.GetString(6),
            RulingDirection = reader.IsDBNull(7) ? null : reader.GetString(7),
            IsUnconstitutional = reader.GetBoolean(8),
            JurisdictionArea = reader.IsDBNull(9) ? null : reader.GetString(9)
        });
    }
    return results;
}

async Task UpdateRulingAsync(string connStr, Guid rulingId,
    string? jurisdiction, string? resourceType, string? subjectArea,
    string? rulingDirection, bool isUnconstitutional, string? jurisdictionArea)
{
    await using var conn = new SqlConnection(connStr);
    await conn.OpenAsync();
    await using var cmd = conn.CreateCommand();
    cmd.CommandText = """
        UPDATE Rulings SET
            Jurisdiction       = COALESCE(@jurisdiction, Jurisdiction),
            JurisdictionArea   = COALESCE(@jurisdictionArea, JurisdictionArea),
            ResourceType       = COALESCE(@resourceType, ResourceType),
            SubjectArea        = COALESCE(@subjectArea, SubjectArea),
            RulingDirection    = COALESCE(@rulingDirection, RulingDirection),
            IsUnconstitutional = CASE WHEN @isUnconstitutional = 1 THEN 1
                                      ELSE IsUnconstitutional END
        WHERE Id = @id
        """;
    cmd.Parameters.AddWithValue("@id", rulingId);
    cmd.Parameters.AddWithValue("@jurisdiction", (object?)jurisdiction ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@jurisdictionArea", (object?)jurisdictionArea ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@resourceType", (object?)resourceType ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@subjectArea", (object?)subjectArea ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@rulingDirection", (object?)rulingDirection ?? DBNull.Value);
    cmd.Parameters.AddWithValue("@isUnconstitutional", isUnconstitutional ? 1 : 0);
    await cmd.ExecuteNonQueryAsync();
}

// ── JSON helpers (match CsjnApiParser patterns) ─────────────

static string? GetString(JsonElement el, params string[] keys)
{
    if (el.ValueKind == JsonValueKind.Array && el.GetArrayLength() > 0)
        return GetString(el[0], keys);
    if (el.ValueKind != JsonValueKind.Object)
        return null;
    foreach (var key in keys)
    {
        if (el.TryGetProperty(key, out var prop))
        {
            var val = GetStringFromElement(prop);
            if (!string.IsNullOrWhiteSpace(val))
                return val.Trim();
        }
    }
    return null;
}

static string? GetStringFromElement(JsonElement prop)
{
    if (prop.ValueKind == JsonValueKind.String)
        return prop.GetString();
    if (prop.ValueKind == JsonValueKind.Number)
    {
        if (prop.TryGetInt64(out var n)) return n.ToString();
        if (prop.TryGetDouble(out var d)) return ((long)d).ToString();
    }
    if (prop.ValueKind == JsonValueKind.Object)
        return GetString(prop, "valor", "value", "descripcion", "description", "codigo", "nombre", "texto");
    return null;
}

static string? GetStringFromValorObject(JsonElement el, string objectKey)
{
    if (!el.TryGetProperty(objectKey, out var obj) || obj.ValueKind != JsonValueKind.Object)
        return null;
    return GetString(obj, "valor", "value", "descripcion", "description");
}

static bool GetBool(JsonElement el, params string[] keys)
{
    foreach (var key in keys)
    {
        if (el.TryGetProperty(key, out var prop))
        {
            if (prop.ValueKind == JsonValueKind.True) return true;
            if (prop.ValueKind == JsonValueKind.False) return false;
            var s = GetStringFromElement(prop);
            if (!string.IsNullOrEmpty(s))
                return s.Equals("true", StringComparison.OrdinalIgnoreCase) || s == "1";
        }
    }
    return false;
}

static string? StripLegacyPrefix(string? value)
    => value?.TrimStart('@').Trim() is { Length: > 0 } v ? v : value;

// ── Infrastructure ──────────────────────────────────────────

static void ShowUsage()
{
    Console.WriteLine("""
        Usage: dotnet run [options]
          --keywords        Backfill keywords from abrirAnalisis.voces
          --limit <N>       Max rulings to process (default: all)
          --delay-ms <ms>   Delay between API calls (default: 1200, 500 for --keywords)
          --dry-run         Show what would be updated without writing
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

record IncompleteRuling
{
    public Guid Id { get; init; }
    public string ExternalId { get; init; } = "";
    public string? AnalysisId { get; init; }
    public DateOnly RulingDate { get; init; }
    public string? Jurisdiction { get; init; }
    public string? ResourceType { get; init; }
    public string? SubjectArea { get; init; }
    public string? RulingDirection { get; init; }
    public bool IsUnconstitutional { get; init; }
    public string? JurisdictionArea { get; init; }
}
