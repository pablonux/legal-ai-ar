using System.Text.RegularExpressions;
using LegalAiAr.Core.Messages;

namespace LegalAiAr.Worker.Enrichment.Parsing;

/// <summary>
/// Extracts proceeding parties (plaintiff, defendant) from Argentine case titles (carátulas).
/// Handles standard patterns like "Actor c/ Demandado s/ materia" and labeled patterns
/// like "ACTOR: X - DEMANDADO: Y".
/// </summary>
public static partial class CaratulaParser
{
    /// <summary>
    /// Extracts parties from a case title. Returns empty list if no parties detected.
    /// </summary>
    public static List<PartyData> ExtractParties(string? caseTitle)
    {
        if (string.IsNullOrWhiteSpace(caseTitle))
            return [];

        var parties = TryExtractFromLabeled(caseTitle);
        if (parties.Count > 0)
            return parties;

        parties = TryExtractFromCvPattern(caseTitle);
        return parties;
    }

    /// <summary>
    /// Detects if a party name looks like a legal entity (S.A., S.R.L., Estado, etc.).
    /// </summary>
    public static string ClassifyPersonType(string name)
    {
        return LegalEntityPattern().IsMatch(name) ? "Legal" : "Physical";
    }

    /// <summary>
    /// Tries labeled patterns: "ACTOR: X", "DEMANDADO: Y", "RECURRENTE: Z", etc.
    /// Common in CSJN fallos destacados.
    /// </summary>
    private static List<PartyData> TryExtractFromLabeled(string title)
    {
        var parties = new List<PartyData>();

        var plaintiffLabels = new[] { "ACTOR", "ACTORA", "ACTORES", "RECURRENTE", "QUERELLANTE", "DEMANDANTE", "INCIDENTISTA" };
        var defendantLabels = new[] { "DEMANDADO", "DEMANDADA", "DEMANDADOS", "RECURRIDO", "RECURRIDA", "IMPUTADO", "IMPUTADA" };

        foreach (var label in plaintiffLabels)
        {
            var match = Regex.Match(title, $@"{label}\s*:\s*(.+?)(?:\s*[-–—]\s*(?:DEMANDAD|RECURRID|IMPUTAD)|$)",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var name = CleanName(match.Groups[1].Value);
                if (name.Length >= 2)
                    parties.Add(new PartyData(name, "PLAINTIFF", ClassifyPersonType(name)));
            }
        }

        foreach (var label in defendantLabels)
        {
            var match = Regex.Match(title, $@"{label}\s*:\s*(.+?)(?:\s*[-–—]\s*|$)",
                RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var name = CleanName(match.Groups[1].Value);
                if (name.Length >= 2)
                    parties.Add(new PartyData(name, "DEFENDANT", ClassifyPersonType(name)));
            }
        }

        return parties;
    }

    /// <summary>
    /// Standard Argentine carátula: "Plaintiff c/ Defendant s/ subject matter".
    /// Variations: "c.", "c/", "contra", "C/".
    /// </summary>
    private static List<PartyData> TryExtractFromCvPattern(string title)
    {
        var match = CvPattern().Match(title);
        if (!match.Success)
            return [];

        var parties = new List<PartyData>();
        var plaintiff = CleanName(match.Groups["plaintiff"].Value);
        var defendant = CleanName(match.Groups["defendant"].Value);

        if (plaintiff.Length >= 2)
            parties.Add(new PartyData(plaintiff, "PLAINTIFF", ClassifyPersonType(plaintiff)));
        if (defendant.Length >= 2)
            parties.Add(new PartyData(defendant, "DEFENDANT", ClassifyPersonType(defendant)));

        return parties;
    }

    private static string CleanName(string raw)
    {
        var cleaned = raw.Trim().TrimEnd('.', ',', ';', '-', '–', '—');
        cleaned = ExtraWhitespace().Replace(cleaned, " ");
        return cleaned.Trim();
    }

    [GeneratedRegex(
        @"^(?<plaintiff>.+?)\s+(?:c[/.]|contra)\s+(?<defendant>.+?)(?:\s+s[/.]|$)",
        RegexOptions.IgnoreCase | RegexOptions.Singleline)]
    private static partial Regex CvPattern();

    [GeneratedRegex(
        @"(?:S\.?A\.?|S\.?R\.?L\.?|S\.?C\.?A\.?|Estado\s+Nacional|Estado\s+Provincial|Gobierno|Municipalidad|Provincia|Nación|AFIP|ANSES|BCRA|YPF|Ministerio)",
        RegexOptions.IgnoreCase)]
    private static partial Regex LegalEntityPattern();

    [GeneratedRegex(@"\s{2,}")]
    private static partial Regex ExtraWhitespace();
}
