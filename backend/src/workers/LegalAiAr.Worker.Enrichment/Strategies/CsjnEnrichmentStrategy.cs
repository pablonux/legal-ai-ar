using System.Text.Json;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Worker.Enrichment.Chunking;
using LegalAiAr.Worker.Enrichment.Parsing;
using LegalAiAr.Worker.Enrichment.Prompts;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Enrichment.Strategies;

/// <summary>
/// CSJN gap-filling strategy. Uses API-extracted data first, then calls GPT-4o only for gaps.
/// Dynamic missingFields: skips "judges" when API votes available, skips "cited_statutes" when
/// API referenciasNormativas available, adds "prosecutor_opinion" only when hasDictamen is true.
/// </summary>
public class CsjnEnrichmentStrategy : IEnrichmentStrategy
{
    private const string DefaultCitationType = "CITES";

    private readonly IEnrichmentService _enrichmentService;
    private readonly TextChunkingService _chunkingService;
    private readonly ILogger<CsjnEnrichmentStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public CsjnEnrichmentStrategy(
        IEnrichmentService enrichmentService,
        TextChunkingService chunkingService,
        ILogger<CsjnEnrichmentStrategy> logger)
    {
        _enrichmentService = enrichmentService;
        _chunkingService = chunkingService;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<IndexerMessage> EnrichAsync(
        EnrichmentMessage message,
        CancellationToken cancellationToken = default)
    {
        var meta = message.ExtractedMetadata;
        var persons = new List<PersonData>();
        var statutes = new List<StatuteData>();
        var citationTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        Task<List<PersonData>>? personsTask = null;
        Task<List<StatuteData>>? statutesTask = null;
        Task<ProsecutorOpinionData?>? prosecutorTask = null;

        if (meta.ApiPersons is { Count: > 0 })
        {
            persons = meta.ApiPersons.Select(p => ResolvePersonData(p)).ToList();
            _logger.LogDebug("Using {Count} persons from API votes for document {DocumentId}", persons.Count, message.DocumentId);
        }
        else if (message.MissingFields.Contains("judges"))
        {
            personsTask = ExtractPersonsAsync(message.NormalizedText, cancellationToken);
        }

        if (meta.ApiStatutes is { Count: > 0 })
        {
            statutes = meta.ApiStatutes
                .Select(s => new StatuteData(s.Number ?? "", s.Name ?? "", s.Articles, BuildStructuredArticles(s)))
                .ToList();
            _logger.LogDebug("Using {Count} statutes from API for document {DocumentId}", statutes.Count, message.DocumentId);
        }
        else if (message.MissingFields.Contains("cited_statutes"))
        {
            statutesTask = ExtractStatutesAsync(message.NormalizedText, cancellationToken);
        }

        if (message.MissingFields.Contains("prosecutor_opinion"))
        {
            prosecutorTask = ExtractProsecutorOpinionAsync(message.NormalizedText, cancellationToken);
        }

        var pendingTasks = new List<Task>();
        if (personsTask is not null) pendingTasks.Add(personsTask);
        if (statutesTask is not null) pendingTasks.Add(statutesTask);
        if (prosecutorTask is not null) pendingTasks.Add(prosecutorTask);

        if (pendingTasks.Count > 0)
            await Task.WhenAll(pendingTasks);

        if (personsTask is not null)
        {
            persons = await personsTask;
            _logger.LogDebug("Extracted {Count} persons via LLM for document {DocumentId}", persons.Count, message.DocumentId);
        }
        if (statutesTask is not null)
        {
            statutes = await statutesTask;
            _logger.LogDebug("Extracted {Count} statutes via LLM for document {DocumentId}", statutes.Count, message.DocumentId);
        }

        if (message.MissingFields.Contains("citation_types") && meta.Citations.Count > 0)
        {
            citationTypes = ClassifyCitationTypesHeuristic(meta.Citations, message.NormalizedText);
            _logger.LogDebug("Classified {Count} citation types (heuristic) for document {DocumentId}", citationTypes.Count, message.DocumentId);
        }

        var citations = meta.Citations
            .Select(c => new CitationData(
                c.Alias,
                c.SummaryId,
                citationTypes.TryGetValue(c.Alias, out var ct) ? ct : DefaultCitationType,
                c.FalloId,
                c.CitationText))
            .ToList();

        var keywords = meta.Keywords
            .Select((k, i) => new KeywordData(k.ExternalCode, k.Description, i))
            .ToList();

        var blobPath = message.BlobPath ?? GetBlobPath(message.SourceId, message.DocumentId);
        var contentHash = message.ContentHash ?? "";

        var sendTextInline = string.IsNullOrEmpty(message.TextBlobPath);

        var (legalBranch, isPlenario, isLeadingCase) = ClassifyOntologyHeuristic(meta);

        var precedentWeight = DerivePrecedentWeight(meta.Instance);

        var ruling = new RulingData(
            CaseTitle: meta.CaseTitle,
            RulingDate: meta.RulingDate,
            CaseNumber: meta.CaseNumber,
            JurisdictionArea: meta.JurisdictionArea,
            Instance: meta.Instance,
            Jurisdiction: meta.Jurisdiction,
            ResourceType: meta.ResourceType,
            RulingDirection: meta.RulingDirection,
            SubjectArea: meta.SubjectArea,
            IsUnconstitutional: meta.IsUnconstitutional,
            Summary: meta.Summary,
            Holding: meta.Holding,
            FullText: sendTextInline ? message.NormalizedText : "",
            BlobPath: blobPath,
            Court: meta.Court,
            LegalBranch: legalBranch,
            PrecedentWeight: precedentWeight,
            IsPlenario: isPlenario,
            IsLeadingCase: isLeadingCase,
            ActionType: meta.ActionType,
            InternalSubject: meta.InternalSubject,
            OfficialReference: meta.OfficialReference,
            Observations: meta.Observations,
            FederalQuestion: meta.FederalQuestion,
            ProceduralFormula: meta.ProceduralFormula,
            HasDictamen: meta.HasDictamen);

        var chunks = sendTextInline
            ? _chunkingService.Chunk(message.NormalizedText)
            : Array.Empty<ChunkData>();

        ProsecutorOpinionData? prosecutorOpinion = null;
        if (prosecutorTask is not null)
        {
            prosecutorOpinion = await prosecutorTask;
            if (prosecutorOpinion is not null)
            {
                var dictamenUrl = FindDictamenUrl(meta.Links);
                if (dictamenUrl is not null)
                    prosecutorOpinion = prosecutorOpinion with { DocumentUrl = dictamenUrl };

                if (!string.IsNullOrWhiteSpace(prosecutorOpinion.ProsecutorName))
                {
                    persons.Add(new PersonData("", prosecutorOpinion.ProsecutorName, "PROSECUTOR"));
                }
            }
            _logger.LogDebug("Prosecutor opinion extraction for {DocumentId}: {HasDictamen}",
                message.DocumentId, prosecutorOpinion is not null);
        }

        var citedBy = meta.CitedBy?
            .Select(c => new CitedByData(c.AnalysisId, c.CaseNumber))
            .ToList()
            ?? [];

        var votes = BuildVoteData(meta.ApiVotes);
        var sumarios = BuildSumarioData(meta.Sumarios);
        var syntheses = BuildSynthesisData(meta.Syntheses);
        var links = BuildLinkData(meta.Links);

        var parties = CaratulaParser.ExtractParties(meta.CaseTitle);
        if (parties.Count > 0)
            _logger.LogDebug("Extracted {Count} parties from carátula for document {DocumentId}", parties.Count, message.DocumentId);

        return new IndexerMessage(
            message.DocumentId,
            contentHash,
            message.SourceId,
            ruling,
            persons,
            keywords,
            statutes,
            citations,
            chunks,
            TextBlobPath: message.TextBlobPath,
            IngestionJobId: message.IngestionJobId,
            AnalysisId: message.AnalysisId,
            CitedBy: citedBy,
            ProsecutorOpinion: prosecutorOpinion,
            Votes: votes,
            Sumarios: sumarios,
            Syntheses: syntheses,
            Links: links,
            Parties: parties,
            Reprocess: message.Reprocess);
    }

    private static PersonData ResolvePersonData(ExtractedPersonDto p)
    {
        if (p.CsjnMinistroId.HasValue)
        {
            var (first, last) = CsjnMinisterDictionary.ResolveSplit(p.CsjnMinistroId.Value, p.Name);
            return new PersonData(first, last, p.RulingRole, p.CsjnMinistroId);
        }

        return new PersonData("", p.Name.Trim(), p.RulingRole);
    }

    private async Task<List<PersonData>> ExtractPersonsAsync(string normalizedText, CancellationToken ct)
    {
        var userContent = JudgesExtractionPrompt.BuildUserPrompt(normalizedText);
        var json = await _enrichmentService.GetStructuredOutputAsync(
            JudgesExtractionPrompt.SystemPrompt,
            userContent,
            JudgesExtractionPrompt.SchemaName,
            JudgesExtractionPrompt.JsonSchema,
            ct);

        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var persons = new List<PersonData>();

        if (root.TryGetProperty("judges", out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in arr.EnumerateArray())
            {
                var firstName = item.GetProperty("firstName").GetString() ?? "";
                var lastName = item.GetProperty("lastName").GetString() ?? "";
                var rulingRole = item.GetProperty("participationType").GetString() ?? "SIGNATORY";
                persons.Add(new PersonData(firstName, lastName, rulingRole));
            }
        }

        return persons;
    }

    private async Task<List<StatuteData>> ExtractStatutesAsync(string normalizedText, CancellationToken ct)
    {
        var userContent = StatutesExtractionPrompt.BuildUserPrompt(normalizedText);
        var json = await _enrichmentService.GetStructuredOutputAsync(
            StatutesExtractionPrompt.SystemPrompt,
            userContent,
            StatutesExtractionPrompt.SchemaName,
            StatutesExtractionPrompt.JsonSchema,
            ct);

        var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var statutes = new List<StatuteData>();

        if (root.TryGetProperty("statutes", out var arr) && arr.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in arr.EnumerateArray())
            {
                var number = item.GetProperty("number").GetString() ?? "";
                var name = item.GetProperty("name").GetString() ?? "";
                var articles = item.TryGetProperty("articles", out var a) && a.ValueKind != JsonValueKind.Null
                    ? a.GetString()
                    : null;
                statutes.Add(new StatuteData(number, name, articles));
            }
        }

        return statutes;
    }

    /// <summary>
    /// Keyword-based heuristic for citation type classification — replaces LLM call.
    /// Searches for directional phrases near each citation alias in the ruling text.
    /// Defaults to CITES when no strong signal is found.
    /// </summary>
    private static Dictionary<string, string> ClassifyCitationTypesHeuristic(
        IReadOnlyList<ExtractedCitationDto> citations,
        string normalizedText)
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var textLower = normalizedText.ToLowerInvariant();

        foreach (var citation in citations)
        {
            if (string.IsNullOrWhiteSpace(citation.Alias))
                continue;

            var idx = textLower.IndexOf(citation.Alias.ToLowerInvariant(), StringComparison.Ordinal);
            if (idx < 0)
            {
                result[citation.Alias] = DefaultCitationType;
                continue;
            }

            var windowStart = Math.Max(0, idx - 300);
            var windowEnd = Math.Min(textLower.Length, idx + citation.Alias.Length + 300);
            var window = textLower[windowStart..windowEnd];

            if (ContainsAny(window, OverruleSignals))
                result[citation.Alias] = "OVERRULES";
            else if (ContainsAny(window, DistinguishSignals))
                result[citation.Alias] = "DISTINGUISHES";
            else if (ContainsAny(window, UpholdsSignals))
                result[citation.Alias] = "UPHOLDS";
            else
                result[citation.Alias] = DefaultCitationType;
        }

        return result;
    }

    private static bool ContainsAny(string text, string[] signals)
    {
        foreach (var signal in signals)
            if (text.Contains(signal, StringComparison.Ordinal))
                return true;
        return false;
    }

    private static readonly string[] OverruleSignals =
    [
        "se aparta de", "apartándose de", "apartandose de",
        "deja sin efecto", "revoca", "revocó",
        "modifica el criterio", "abandona la doctrina",
        "contrariamente a lo resuelto en"
    ];

    private static readonly string[] DistinguishSignals =
    [
        "se distingue de", "a diferencia de", "no resulta aplicable",
        "no es asimilable", "caso distinto", "circunstancias diferentes",
        "no guarda analogía"
    ];

    private static readonly string[] UpholdsSignals =
    [
        "conforme lo resuelto en", "en concordancia con",
        "ratifica", "ratificó", "reafirma",
        "sigue el criterio", "remisión a", "remite a",
        "tal como se decidió en", "de conformidad con"
    ];

    /// <summary>
    /// Heuristic ontology classification — replaces LLM call with SubjectArea mapping + regex.
    /// </summary>
    private (string? LegalBranch, bool IsPlenario, bool IsLeadingCase) ClassifyOntologyHeuristic(
        ExtractedMetadata meta)
    {
        var legalBranch = MapSubjectAreaToLegalBranch(meta.SubjectArea);
        var isPlenario = DetectPlenario(meta.CaseTitle, meta.Summary);
        return (legalBranch, isPlenario, false);
    }

    private static readonly Dictionary<string, string> SubjectAreaMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["DERECHO CONSTITUCIONAL"] = "PUB_CONST",
        ["CONSTITUCIONAL"] = "PUB_CONST",
        ["DERECHO ADMINISTRATIVO"] = "PUB_ADMIN",
        ["ADMINISTRATIVO"] = "PUB_ADMIN",
        ["EMPLEO PUBLICO"] = "PUB_ADMIN",
        ["DERECHO PENAL"] = "PUB_PENAL",
        ["PENAL"] = "PUB_PENAL",
        ["DERECHO PROCESAL CIVIL"] = "PUB_PROC_CIV",
        ["PROCESAL CIVIL"] = "PUB_PROC_CIV",
        ["DERECHO PROCESAL"] = "PUB_PROC_CIV",
        ["DERECHO PROCESAL PENAL"] = "PUB_PROC_PEN",
        ["PROCESAL PENAL"] = "PUB_PROC_PEN",
        ["DERECHO TRIBUTARIO"] = "PUB_TRIB",
        ["TRIBUTARIO"] = "PUB_TRIB",
        ["IMPUESTOS"] = "PUB_TRIB",
        ["DERECHO INTERNACIONAL PUBLICO"] = "PUB_INT",
        ["DERECHO INTERNACIONAL"] = "PUB_INT",
        ["DERECHO CIVIL"] = "PRIV_CIVIL",
        ["CIVIL"] = "PRIV_CIVIL",
        ["DAÑOS Y PERJUICIOS"] = "PRIV_CIVIL",
        ["DERECHO COMERCIAL"] = "PRIV_COM",
        ["COMERCIAL"] = "PRIV_COM",
        ["SOCIEDADES"] = "PRIV_COM",
        ["CONCURSOS Y QUIEBRAS"] = "PRIV_COM",
        ["DERECHO LABORAL"] = "PRIV_LAB",
        ["LABORAL"] = "PRIV_LAB",
        ["TRABAJO"] = "PRIV_LAB",
        ["DERECHO COLECTIVO DEL TRABAJO"] = "PRIV_LAB_COL",
        ["SEGURO"] = "PRIV_SEG",
        ["SEGUROS"] = "PRIV_SEG",
        ["PROPIEDAD INTELECTUAL"] = "PRIV_PI",
        ["MARCAS Y PATENTES"] = "PRIV_PI",
        ["DERECHO DE FAMILIA"] = "SOC_FAM",
        ["FAMILIA"] = "SOC_FAM",
        ["SEGURIDAD SOCIAL"] = "SOC_PREV",
        ["JUBILACIONES Y PENSIONES"] = "SOC_PREV",
        ["PREVISIONAL"] = "SOC_PREV",
        ["NIÑEZ Y ADOLESCENCIA"] = "SOC_NINEZ",
        ["MENORES"] = "SOC_NINEZ",
        ["DERECHO AMBIENTAL"] = "SOC_AMB",
        ["AMBIENTE"] = "SOC_AMB",
        ["MEDIO AMBIENTE"] = "SOC_AMB",
        ["DEFENSA DEL CONSUMIDOR"] = "SOC_CONS",
        ["CONSUMIDOR"] = "SOC_CONS",
        ["PROTECCION DE DATOS"] = "DIG_DATOS",
        ["HABEAS DATA"] = "DIG_DATOS",
    };

    private static string? MapSubjectAreaToLegalBranch(string? subjectArea)
    {
        if (string.IsNullOrWhiteSpace(subjectArea))
            return null;

        if (SubjectAreaMap.TryGetValue(subjectArea.Trim(), out var branch))
            return branch;

        foreach (var kvp in SubjectAreaMap)
        {
            if (subjectArea.Contains(kvp.Key, StringComparison.OrdinalIgnoreCase))
                return kvp.Value;
        }

        return null;
    }

    private static bool DetectPlenario(string? caseTitle, string? summary)
    {
        var sources = new[] { caseTitle, summary };
        foreach (var text in sources)
        {
            if (string.IsNullOrWhiteSpace(text)) continue;
            if (text.Contains("plenario", StringComparison.OrdinalIgnoreCase)
                || text.Contains("en pleno", StringComparison.OrdinalIgnoreCase)
                || text.Contains("acuerdo plenario", StringComparison.OrdinalIgnoreCase))
                return true;
        }
        return false;
    }

    private static string? DerivePrecedentWeight(string? instance)
    {
        if (string.IsNullOrWhiteSpace(instance))
            return null;

        return instance.ToUpperInvariant() switch
        {
            "CSJN" => "HIGHLY_PERSUASIVE",
            _ when instance.Contains("Cámara", StringComparison.OrdinalIgnoreCase) => "PERSUASIVE",
            _ when instance.Contains("Juzgado", StringComparison.OrdinalIgnoreCase) => "REFERENTIAL",
            _ => "REFERENTIAL"
        };
    }

    private async Task<ProsecutorOpinionData?> ExtractProsecutorOpinionAsync(string normalizedText, CancellationToken ct)
    {
        try
        {
            var userContent = ProsecutorOpinionPrompt.BuildUserPrompt(normalizedText);
            var json = await _enrichmentService.GetStructuredOutputAsync(
                ProsecutorOpinionPrompt.SystemPrompt,
                userContent,
                ProsecutorOpinionPrompt.SchemaName,
                ProsecutorOpinionPrompt.JsonSchema,
                ct);

            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("hasDictamen", out var hd) || hd.ValueKind != JsonValueKind.True)
                return null;

            var name = root.TryGetProperty("prosecutorName", out var pn) ? pn.GetString() ?? "" : "";
            var summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;
            var direction = root.TryGetProperty("recommendedDirection", out var rd) ? rd.GetString() : null;
            var agreed = root.TryGetProperty("agreedWithCourt", out var ac) && ac.ValueKind == JsonValueKind.True;

            return new ProsecutorOpinionData(name, summary, direction, agreed);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Prosecutor opinion extraction failed, skipping");
            return null;
        }
    }

    private static IReadOnlyList<StatuteArticleData>? BuildStructuredArticles(ExtractedStatuteDto s)
    {
        if (string.IsNullOrWhiteSpace(s.Article))
            return null;
        return [new StatuteArticleData(s.Article, s.Subsection)];
    }

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

    private static List<VoteData> BuildVoteData(IReadOnlyList<CsjnVoteDto>? apiVotes)
    {
        if (apiVotes is null || apiVotes.Count == 0) return [];

        return apiVotes.Select(v =>
        {
            var signatories = new List<PersonData>();
            if (v.Ministers is { Count: > 0 })
            {
                foreach (var m in v.Ministers)
                {
                    var (first, last) = CsjnMinisterDictionary.ResolveSplit(m.MinistroId, m.Surname);
                    signatories.Add(new PersonData(first, last, MapVoteType(v.VoteType), m.MinistroId));
                }
            }
            else if (!string.IsNullOrWhiteSpace(v.Judges))
            {
                var names = v.Judges.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                foreach (var name in names)
                    signatories.Add(new PersonData("", name.Trim(), MapVoteType(v.VoteType)));
            }

            return new VoteData(v.VoteType, v.Pages, null, signatories);
        }).ToList();
    }

    private static List<SumarioData> BuildSumarioData(IReadOnlyList<ExtractedSumarioDto>? sumarios)
    {
        if (sumarios is null || sumarios.Count == 0) return [];
        return sumarios.Select(s => new SumarioData(
            s.ExternalId,
            s.Text,
            s.Volume,
            s.Page,
            s.SortOrder,
            s.Keywords.Select((k, i) => new SumarioKeywordData(k.ExternalCode, k.Description, i)).ToList()
        )).ToList();
    }

    private static List<SynthesisData> BuildSynthesisData(IReadOnlyList<ExtractedSynthesisDto>? syntheses)
    {
        if (syntheses is null || syntheses.Count == 0) return [];
        return syntheses.Select(s => new SynthesisData(s.Text, s.SortOrder)).ToList();
    }

    private static List<LinkData> BuildLinkData(IReadOnlyList<ExtractedLinkDto>? links)
    {
        if (links is null || links.Count == 0) return [];
        return links.Select(l => new LinkData(l.Url, l.Title, l.LinkType)).ToList();
    }

    private static string? FindDictamenUrl(IReadOnlyList<ExtractedLinkDto>? links)
    {
        if (links is null || links.Count == 0) return null;
        return links
            .FirstOrDefault(l => l.Title != null &&
                l.Title.Contains("DICTAMEN", StringComparison.OrdinalIgnoreCase))
            ?.Url;
    }

    private static string GetBlobPath(int sourceId, string documentId)
    {
        return LegalAiAr.Infrastructure.Blob.BlobPathHelper.BuildPdfPath("ruling", sourceId, documentId, yearMonth: null);
    }
}
