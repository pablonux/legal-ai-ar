using System.Text.RegularExpressions;

namespace LegalAiAr.Application.Chat.Utilities;

/// <summary>
/// Parses, normalizes, and validates <c>{caso: "...", id: "..."}</c> citation references
/// in model responses. Regex aligned with frontend <c>CITATION_REGEX</c>.
/// </summary>
public static class CitationParser
{
    private static readonly Regex CitationRegex = new(
        @"\{caso:\s*""([^""]+)""\s*,\s*id:\s*""([a-f0-9-]+)""\}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private static readonly Regex MalformedCitationRegex = new(
        @"\{caso:\s*'([^']+)'\s*,\s*id:\s*'([a-f0-9-]+)'\}",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static IReadOnlyList<Citation> Parse(string text)
    {
        var results = new List<Citation>();
        foreach (Match match in CitationRegex.Matches(text))
        {
            if (Guid.TryParse(match.Groups[2].Value, out var id))
                results.Add(new Citation(match.Groups[1].Value, id, match.Index, match.Length));
        }
        return results;
    }

    /// <summary>
    /// Normalizes citation format: single quotes to double, extra whitespace removal.
    /// </summary>
    public static string Normalize(string text)
    {
        return MalformedCitationRegex.Replace(text, m =>
            $"{{caso: \"{m.Groups[1].Value}\", id: \"{m.Groups[2].Value}\"}}");
    }

    public static CitationValidationResult Validate(
        Citation citation,
        IReadOnlyDictionary<Guid, string> knownRulings,
        IReadOnlySet<Guid> resolvedIds)
    {
        var issues = new List<string>();

        if (!knownRulings.TryGetValue(citation.RulingId, out var dbTitle))
        {
            issues.Add($"Ruling ID {citation.RulingId} not found in database");
            return new CitationValidationResult(citation, CitationSeverity.Critical, issues);
        }

        if (!resolvedIds.Contains(citation.RulingId))
            issues.Add($"Ruling {citation.RulingId} was not returned by any tool call");

        if (!string.IsNullOrEmpty(dbTitle) && FuzzyMatchRatio(citation.CaseTitle, dbTitle) < 0.85)
            issues.Add($"Case title mismatch: response=\"{citation.CaseTitle}\", db=\"{dbTitle}\"");

        var severity = issues.Count > 0 ? CitationSeverity.Warning : CitationSeverity.None;
        return new CitationValidationResult(citation, severity, issues);
    }

    internal static double FuzzyMatchRatio(string a, string b)
    {
        if (string.IsNullOrEmpty(a) || string.IsNullOrEmpty(b)) return 0;
        var normalized1 = a.Trim().ToLowerInvariant();
        var normalized2 = b.Trim().ToLowerInvariant();
        if (normalized1 == normalized2) return 1.0;

        var distance = LevenshteinDistance(normalized1, normalized2);
        var maxLen = Math.Max(normalized1.Length, normalized2.Length);
        return 1.0 - (double)distance / maxLen;
    }

    private static int LevenshteinDistance(string s, string t)
    {
        var n = s.Length;
        var m = t.Length;
        var d = new int[n + 1, m + 1];

        for (var i = 0; i <= n; i++) d[i, 0] = i;
        for (var j = 0; j <= m; j++) d[0, j] = j;

        for (var i = 1; i <= n; i++)
            for (var j = 1; j <= m; j++)
            {
                var cost = s[i - 1] == t[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        return d[n, m];
    }
}

public sealed record Citation(string CaseTitle, Guid RulingId, int Offset, int Length);

public sealed record CitationValidationResult(
    Citation Citation,
    CitationSeverity Severity,
    IReadOnlyList<string> Issues);

public enum CitationSeverity
{
    None,
    Info,
    Warning,
    Critical
}
