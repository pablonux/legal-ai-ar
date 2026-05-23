using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Worker.Parser.Parsers;

/// <summary>
/// Maps CsjnApiMetadata to ExtractedMetadata and computes dynamic missingFields for EnrichmentMessage.
/// When the API provides structured data (votes, statutes), the corresponding LLM call is skipped.
/// </summary>
public static class CsjnMetadataMapper
{
    private const int CsjnSourceId = 1;
    private const string CsjnCourtName = "Corte Suprema de Justicia de la Nación";
    private const string CsjnInstanceLevel = "CSJN";

    internal static bool IsKnownMinister(int ministroId)
        => CsjnMinisterDictionary.Ministers.ContainsKey(ministroId);

    public static ExtractedMetadata ToExtractedMetadata(CsjnApiMetadata api)
    {
        var keywords = api.Keywords
            .Select(k => new ExtractedKeywordDto(k.ExternalCode, k.Description))
            .ToList();

        var citations = api.Citations
            .Select(c => new ExtractedCitationDto(c.Alias, c.SummaryId, c.FalloId, c.CitationText))
            .ToList();

        var citedBy = api.CitedBy
            .Select(c => new ExtractedCitedByDto(c.AnalysisId, c.CaseNumber))
            .ToList();

        var apiPersons = ConvertVotesToPersons(api.Votes);
        var apiStatutes = ConvertApiStatutes(api.ApiStatutes);
        var sumarios = ConvertSumarios(api.Sumarios);
        var syntheses = ConvertSyntheses(api.Syntheses);
        var links = ConvertLinks(api.Links);
        var dictamenes = ConvertDictamenes(api.Dictamenes);

        return new ExtractedMetadata(
            CaseTitle: api.CaseTitle,
            RulingDate: api.RulingDate,
            CaseNumber: api.CaseNumber,
            Court: CsjnCourtName,
            JurisdictionArea: api.Jurisdiction,
            Instance: CsjnInstanceLevel,
            RulingDirection: api.RulingDirection,
            Summary: api.Summary,
            Holding: api.Holding,
            Keywords: keywords,
            Citations: citations,
            Jurisdiction: api.Jurisdiction,
            ResourceType: api.ResourceType,
            SubjectArea: StripLegacyPrefix(api.SubjectArea),
            IsUnconstitutional: api.IsUnconstitutional,
            CitedBy: citedBy,
            ActionType: api.ActionType,
            InternalSubject: StripLegacyPrefix(api.InternalSubject),
            OfficialReference: api.OfficialReference,
            Observations: api.Observations,
            FederalQuestion: api.FederalQuestion,
            ProceduralFormula: api.ProceduralFormula,
            HasDictamen: api.HasDictamen,
            ApiPersons: apiPersons,
            ApiStatutes: apiStatutes,
            ApiVotes: api.Votes,
            Sumarios: sumarios,
            Syntheses: syntheses,
            Links: links,
            Dictamenes: dictamenes);
    }

    /// <summary>
    /// Converts structured vote data from API into person entries.
    /// Prefers ministros[] with stable IDs; falls back to comma-split vocales.
    /// </summary>
    private static List<ExtractedPersonDto> ConvertVotesToPersons(IReadOnlyList<CsjnVoteDto>? votes)
    {
        if (votes is null || votes.Count == 0)
            return [];

        var persons = new List<ExtractedPersonDto>();
        var seenMinistroIds = new HashSet<int>();

        foreach (var vote in votes)
        {
            var rulingRole = MapVoteType(vote.VoteType);

            if (vote.Ministers is { Count: > 0 })
            {
                foreach (var m in vote.Ministers)
                {
                    if (!seenMinistroIds.Add(m.MinistroId))
                        continue;

                    var canonicalName = ResolveMinisterName(m.MinistroId, m.Surname);
                    persons.Add(new ExtractedPersonDto(canonicalName, rulingRole, m.MinistroId));
                }
            }
            else
            {
                var names = vote.Judges
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var name in names)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                        persons.Add(new ExtractedPersonDto(name.Trim(), rulingRole));
                }
            }
        }

        return persons;
    }

    internal static string ResolveMinisterName(int ministroId, string apiSurname)
        => CsjnMinisterDictionary.ResolveFullName(ministroId, apiSurname);

    private static string MapVoteType(string csjnVoteType)
    {
        var upper = csjnVoteType.ToUpperInvariant();
        if (upper.Contains("DISIDENCIA"))
            return "DISSENT";
        if (upper.Contains("MAYORIA") || upper.Contains("ADHIERE"))
            return "MAJORITY";
        if (upper.Contains("CONCURRENCIA") || upper.Contains("VOTO PROPIO"))
            return "CONCURRENCE";
        if (upper.Contains("ABSTENCION"))
            return "ABSTENTION";
        return "SIGNATORY";
    }

    /// <summary>
    /// Converts API normative references to statute DTOs.
    /// </summary>
    private static List<ExtractedStatuteDto> ConvertApiStatutes(IReadOnlyList<CsjnApiStatuteDto>? apiStatutes)
    {
        if (apiStatutes is null || apiStatutes.Count == 0)
            return [];

        return apiStatutes
            .Select(s =>
            {
                var number = s.Number ?? "";
                var name = s.Description ?? (s.NormType != null ? $"{s.NormType} {number}".Trim() : number);
                var articles = s.Article;
                if (!string.IsNullOrEmpty(s.Subsection) && !string.IsNullOrEmpty(articles))
                    articles = $"{articles} inc. {s.Subsection}";
                return new ExtractedStatuteDto(number, name, articles, s.Article, s.Subsection);
            })
            .Where(s => !string.IsNullOrWhiteSpace(s.Number) || !string.IsNullOrWhiteSpace(s.Name))
            .ToList();
    }

    /// <summary>
    /// Returns dynamic missingFields based on what the API actually provided.
    /// For CSJN: skips "judges" if votosAnalisisDocumental had data, skips "cited_statutes" if
    /// referenciasNormativas had data, and conditionally includes "prosecutor_opinion" if hasDictamen.
    /// </summary>
    public static IReadOnlyList<string> GetMissingFields(int sourceId, CsjnApiMetadata? apiMetadata = null)
    {
        if (sourceId != CsjnSourceId)
            return [];

        var missing = new List<string>();

        var hasApiPersons = apiMetadata?.Votes is { Count: > 0 };
        if (!hasApiPersons)
            missing.Add("judges");

        var hasApiStatutes = apiMetadata?.ApiStatutes is { Count: > 0 };
        if (!hasApiStatutes)
            missing.Add("cited_statutes");

        missing.Add("citation_types");

        if (apiMetadata?.HasDictamen == true)
            missing.Add("prosecutor_opinion");

        return missing;
    }

    private static List<ExtractedSumarioDto> ConvertSumarios(IReadOnlyList<CsjnSumarioDto>? src)
    {
        if (src is null || src.Count == 0) return [];
        return src.Select(s => new ExtractedSumarioDto(
            s.Id,
            s.Text,
            s.Volume,
            s.Page,
            s.SortOrder,
            s.Keywords.Select(k => new ExtractedKeywordDto(k.ExternalCode, k.Description)).ToList()
        )).ToList();
    }

    private static List<ExtractedSynthesisDto> ConvertSyntheses(IReadOnlyList<CsjnSintesisDto>? src)
    {
        if (src is null || src.Count == 0) return [];
        return src.Select(s => new ExtractedSynthesisDto(s.Text, s.SortOrder)).ToList();
    }

    private static List<ExtractedLinkDto> ConvertLinks(IReadOnlyList<CsjnEnlaceDto>? src)
    {
        if (src is null || src.Count == 0) return [];
        return src.Select(s => new ExtractedLinkDto(
            s.Url,
            s.Description,
            s.IsInternal ? "INTERNAL" : "EXTERNAL"
        )).ToList();
    }

    private static List<ExtractedDictamenDto> ConvertDictamenes(IReadOnlyList<CsjnDictamenDto>? src)
    {
        if (src is null || src.Count == 0) return [];
        return src.Select(s => new ExtractedDictamenDto(s.Title, s.DocumentUrl, s.DocumentType)).ToList();
    }

    private static string? StripLegacyPrefix(string? value)
        => value?.TrimStart('@').Trim() is { Length: > 0 } v ? v : value;
}
