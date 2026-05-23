using System.Text.RegularExpressions;
using LegalAiAr.Core.Models;

namespace LegalAiAr.Application.Chat.Pipeline;

/// <summary>
/// Rule-based intent classifier and entity extractor. Handles ~60-70% of queries
/// without an LLM call. Returns null confidence when inconclusive (LLM fallback trigger).
/// </summary>
public sealed class RuleBasedQueryEnricher
{
    private static readonly RegexOptions Opts =
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant;

    #region Intent Patterns

    private static readonly (Regex Pattern, string Intent, float Confidence)[] IntentRules =
    [
        (new(@"cu[áa]ntos?\b|cantidad\b|porcentaje\b|estad[ií]sticas?\b|total\s+de\b", Opts), "statistics", 0.90f),
        (new(@"(?:art(?:[ií]culo)?\.?\s*\d+)|(?:ley\s+\d[\d.]*)|(?:c[óo]digo\s+(?:penal|civil|comercial|procesal))|(?:decreto\s+\d+)|(?:constituci[óo]n\s+nacional|CN\b)", Opts), "statute_research", 0.85f),
        (new(@"cit[aáe]\w*\b|citado\s+por\b|precedente\b|cadena\b|influen\w+\b|posterior\w*\s+(?:a|al|que)\b", Opts), "precedent_exploration", 0.85f),
        (new(@"detalle\w*\b|mostr[áa]\w*\b.*fallo|informaci[óo]n\s+(?:del?|sobre)\s+(?:el\s+)?fallo|qui[ée]nes?\s+(?:firmaron|votaron)", Opts), "detail", 0.80f),
        (new(@"compar[áa]\w*\b|diferencia\w*\b|vs\.?\b|(?:en\s+)?contra(?:posici[óo]n|ste)\b", Opts), "comparison", 0.80f),
    ];

    #endregion

    #region Entity Patterns

    private static readonly Regex YearRange = new(@"entre\s+(\d{4})\s+y\s+(\d{4})", Opts);
    private static readonly Regex SinceYear = new(@"desde\s+(?:el\s+)?(\d{4})", Opts);
    private static readonly Regex UntilYear = new(@"hasta\s+(?:el\s+)?(\d{4})", Opts);
    private static readonly Regex LastNYears = new(@"[úu]ltimos?\s+(\d+)\s+a[ñn]os?", Opts);
    private static readonly Regex SpecificYear = new(@"\b((?:19|20)\d{2})\b", Opts);

    private static readonly Regex ArticleCn = new(@"art(?:[ií]culo)?\.?\s*(\d+)\s+(?:de\s+la\s+)?(?:CN|Constituci[óo]n\s+Nacional)", Opts);
    private static readonly Regex NumberedLaw = new(@"[Ll]ey\s+(\d[\d.]*)", Opts);
    private static readonly Regex NamedCode = new(@"[Cc][óo]digo\s+(Penal|Civil|Comercial|Procesal\s+\w+)", Opts);
    private static readonly Regex ArticleCode = new(@"art(?:[ií]culo)?\.?\s*(\d+)\s+(?:del?\s+)?[Cc][óo]digo\s+(\w+)", Opts);
    private static readonly Regex Decree = new(@"[Dd]ecreto\s+(\d+[/\-]?\d*)", Opts);

    private static readonly Regex CaseFormat = new(@"([A-ZÁÉÍÓÚÑ][\wáéíóúñ.]+(?:\s+[\wáéíóúñ.]+)*)\s+c[/\.]\s+([A-ZÁÉÍÓÚÑ][\wáéíóúñ.]+(?:\s+[\wáéíóúñ.]+)*)", Opts);
    private static readonly Regex FalloRef = new(@"fallo\s+[""']?([A-ZÁÉÍÓÚÑ][\wáéíóúñ]+(?:\s+[\wáéíóúñ]+)*)[""']?", Opts);
    private static readonly Regex FallosCitation = new(@"Fallos:\s*(\d+):(\d+)", Opts);

    private static readonly HashSet<string> CourtKeywords = new(StringComparer.OrdinalIgnoreCase)
    {
        "CSJN", "Corte Suprema", "Suprema Corte",
        "Cámara Federal", "C.Fed.", "Cámara Nacional",
        "Casación Penal", "Casación", "Cámara Electoral",
    };

    private static readonly Regex CourtPattern = new(
        @"(?:CSJN|Corte\s+Suprema|Suprema\s+Corte|Cámara\s+(?:Federal|Nacional|Electoral)|C\.Fed\.|Casación(?:\s+Penal)?)",
        Opts);

    #endregion

    public (string Intent, float? Confidence) ClassifyIntent(string query)
    {
        foreach (var (pattern, intent, confidence) in IntentRules)
        {
            if (pattern.IsMatch(query))
                return (intent, confidence);
        }

        return ("general", null);
    }

    public QueryEnrichment? ExtractEntities(string query, string intent, float? confidence)
    {
        var temporal = ExtractTemporal(query);
        var courts = ExtractCourts(query);
        var statutes = ExtractStatutes(query);
        var cases = ExtractCases(query);

        var hasEntities = temporal.Count > 0 || courts.Count > 0
            || statutes.Count > 0 || cases.Count > 0;

        if (!hasEntities && confidence is null)
            return null;

        return new QueryEnrichment
        {
            Intent = intent,
            Source = EnrichmentSource.RuleBased,
            Confidence = confidence,
            Temporal = temporal,
            Courts = courts,
            Statutes = statutes,
            Cases = cases,
        };
    }

    private static List<TemporalEntity> ExtractTemporal(string query)
    {
        var results = new List<TemporalEntity>();

        var rangeMatch = YearRange.Match(query);
        if (rangeMatch.Success)
        {
            results.Add(new TemporalEntity(int.Parse(rangeMatch.Groups[1].Value), int.Parse(rangeMatch.Groups[2].Value)));
            return results;
        }

        var sinceMatch = SinceYear.Match(query);
        if (sinceMatch.Success)
        {
            results.Add(new TemporalEntity(int.Parse(sinceMatch.Groups[1].Value), null));
            return results;
        }

        var untilMatch = UntilYear.Match(query);
        if (untilMatch.Success)
        {
            results.Add(new TemporalEntity(0, int.Parse(untilMatch.Groups[1].Value)));
            return results;
        }

        var lastNMatch = LastNYears.Match(query);
        if (lastNMatch.Success)
        {
            var years = int.Parse(lastNMatch.Groups[1].Value);
            results.Add(new TemporalEntity(DateTime.UtcNow.Year - years, null));
            return results;
        }

        return results;
    }

    private static List<string> ExtractCourts(string query)
    {
        var courts = new List<string>();
        foreach (Match match in CourtPattern.Matches(query))
            courts.Add(match.Value);
        return courts;
    }

    private static List<StatuteEntity> ExtractStatutes(string query)
    {
        var statutes = new List<StatuteEntity>();

        foreach (Match match in ArticleCn.Matches(query))
            statutes.Add(new StatuteEntity("Constitución Nacional", [match.Groups[1].Value]));

        foreach (Match match in ArticleCode.Matches(query))
            statutes.Add(new StatuteEntity($"Código {match.Groups[2].Value}", [match.Groups[1].Value]));

        foreach (Match match in NumberedLaw.Matches(query))
            statutes.Add(new StatuteEntity($"Ley {match.Groups[1].Value}", []));

        foreach (Match match in NamedCode.Matches(query))
        {
            var name = $"Código {match.Groups[1].Value}";
            if (!statutes.Any(s => s.Reference == name))
                statutes.Add(new StatuteEntity(name, []));
        }

        foreach (Match match in Decree.Matches(query))
            statutes.Add(new StatuteEntity($"Decreto {match.Groups[1].Value}", []));

        return statutes;
    }

    private static List<string> ExtractCases(string query)
    {
        var cases = new List<string>();

        foreach (Match match in CaseFormat.Matches(query))
            cases.Add($"{match.Groups[1].Value} c/ {match.Groups[2].Value}");

        foreach (Match match in FalloRef.Matches(query))
            cases.Add(match.Groups[1].Value);

        foreach (Match match in FallosCitation.Matches(query))
            cases.Add($"Fallos: {match.Groups[1].Value}:{match.Groups[2].Value}");

        return cases;
    }
}
