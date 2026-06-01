using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Indexer.Steps;

/// <summary>
/// Persists IndexerMessage to Azure SQL: Rulings, Courts, Persons, RulingParticipations,
/// Keywords, RulingKeywords, Statutes, RulingStatutes, Citations.
/// Single transaction. Per E056.
/// </summary>
public class PersistRulingStep
{
    private readonly AppDbContext _context;
    private readonly IRulingRepository _rulingRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IKeywordRepository _keywordRepository;
    private readonly IStatuteRepository _statuteRepository;
    private readonly ICitationRepository _citationRepository;
    private readonly IKeywordNormalizationService _keywordNormalizer;
    private readonly IJudicialProceedingRepository _proceedingRepository;
    private readonly IAuditService _auditService;
    private readonly ILogger<PersistRulingStep> _logger;

    public PersistRulingStep(
        AppDbContext context,
        IRulingRepository rulingRepository,
        ICourtRepository courtRepository,
        IPersonRepository personRepository,
        IKeywordRepository keywordRepository,
        IStatuteRepository statuteRepository,
        ICitationRepository citationRepository,
        IKeywordNormalizationService keywordNormalizer,
        IJudicialProceedingRepository proceedingRepository,
        IAuditService auditService,
        ILogger<PersistRulingStep> logger)
    {
        _context = context;
        _rulingRepository = rulingRepository;
        _courtRepository = courtRepository;
        _personRepository = personRepository;
        _keywordRepository = keywordRepository;
        _statuteRepository = statuteRepository;
        _citationRepository = citationRepository;
        _keywordNormalizer = keywordNormalizer;
        _proceedingRepository = proceedingRepository;
        _auditService = auditService;
        _logger = logger;
    }

    /// <summary>
    /// Persists the ruling and all related entities in a single transaction.
    /// </summary>
    /// <returns>Result with RulingId and resolved CourtName for downstream steps.</returns>
    public async Task<PersistRulingResult> ExecuteAsync(IndexerMessage message, CancellationToken cancellationToken = default)
    {
        var rulingData = message.Ruling;

        var courtName = !string.IsNullOrWhiteSpace(rulingData.Court)
            ? rulingData.Court
            : !string.IsNullOrWhiteSpace(rulingData.Instance)
                ? rulingData.Instance
                : "CSJN";
        var court = await _courtRepository.GetOrCreateAsync(
            name: courtName,
            jurisdictionArea: "Federal",
            territory: "Nacional",
            instance: rulingData.Instance ?? "Única",
            cancellationToken);

        ClassifyCourtIfNeeded(court, rulingData.Instance);

        var ruling = new Ruling
        {
            Id = Guid.NewGuid(),
            SourceId = message.SourceId,
            ExternalId = message.DocumentId,
            AnalysisId = message.AnalysisId,
            ContentHash = message.ContentHash,
            CaseTitle = rulingData.CaseTitle,
            CaseNumber = rulingData.CaseNumber,
            RulingDate = rulingData.RulingDate,
            Court = court,
            JurisdictionArea = rulingData.JurisdictionArea,
            Instance = rulingData.Instance,
            Jurisdiction = rulingData.Jurisdiction,
            ResourceType = rulingData.ResourceType,
            RulingDirection = rulingData.RulingDirection,
            SubjectArea = rulingData.SubjectArea,
            IsUnconstitutional = rulingData.IsUnconstitutional,
            Summary = rulingData.Summary,
            Holding = rulingData.Holding,
            FullText = rulingData.FullText,
            BlobPath = rulingData.BlobPath,
            ActionType = rulingData.ActionType,
            InternalSubject = rulingData.InternalSubject,
            OfficialReference = rulingData.OfficialReference,
            Observations = rulingData.Observations,
            FederalQuestion = rulingData.FederalQuestion,
            ProceduralFormula = rulingData.ProceduralFormula,
            HasDictamen = rulingData.HasDictamen,
            IndexedAt = DateTime.UtcNow,
            Status = RulingStatus.Indexed,
            IngestionJobId = message.IngestionJobId,
            LegalBranch = ParseEnum<LegalBranch>(rulingData.LegalBranch),
            PrecedentWeight = ParseEnum<PrecedentWeight>(rulingData.PrecedentWeight),
            IsPlenario = rulingData.IsPlenario,
            IsLeadingCase = rulingData.IsLeadingCase
        };

        var keywordByLookup = await PrefetchKeywordsForMessageAsync(message, cancellationToken);

        var seenPersonIds = new HashSet<int>();
        foreach (var p in message.Persons)
        {
            var displayName = !string.IsNullOrEmpty(p.LastName) && !string.IsNullOrEmpty(p.FirstName)
                ? $"{p.LastName}, {p.FirstName}"
                : !string.IsNullOrEmpty(p.LastName) ? p.LastName : p.FirstName;
            var person = await _personRepository.GetOrCreateAsync(displayName, p.FirstName, p.LastName, p.CsjnMinistroId, cancellationToken);
            if (!seenPersonIds.Add(person.Id))
            {
                _logger.LogDebug("Skipping duplicate person {DisplayName} for document {DocumentId}",
                    displayName, message.DocumentId);
                continue;
            }

            var role = Enum.TryParse<RulingRole>(p.RulingRole, true, out var rr) ? rr : RulingRole.SIGNATORY;
            ruling.RulingParticipations.Add(new RulingParticipation
            {
                Ruling = ruling,
                Person = person,
                Role = role
            });
        }

        foreach (var k in message.Keywords)
        {
            if (IsJunkKeyword(k.Description))
            {
                _logger.LogDebug("Filtering junk keyword '{Description}' for document {DocumentId}",
                    k.Description, message.DocumentId);
                continue;
            }

            var keyword = keywordByLookup[new KeywordLookupKey(k.ExternalCode, k.Description)];
            if (keyword.ThesaurusTermId == null)
            {
                keyword.ThesaurusTermId = await _keywordNormalizer.ResolveAsync(k.Description, cancellationToken);
            }
            ruling.RulingKeywords.Add(new RulingKeyword
            {
                Ruling = ruling,
                Keyword = keyword,
                SortOrder = k.SortOrder
            });
        }

        var seenStatuteIds = new HashSet<int>();
        var statutePairs = message.Statutes.Select(s => (s.Number, s.Name)).ToList();
        var statuteByNumber = statutePairs.Count == 0
            ? new Dictionary<string, Statute>(StringComparer.Ordinal)
            : (Dictionary<string, Statute>)(await _statuteRepository.GetOrCreateBatchAsync(statutePairs, cancellationToken));

        foreach (var s in message.Statutes)
        {
            if (!statuteByNumber.TryGetValue(s.Number, out var statute))
                continue;

            if (statute.Name != s.Name)
                statute.Name = s.Name;

            if (!seenStatuteIds.Add(statute.Id))
            {
                _logger.LogDebug("Skipping duplicate statute {Number} ({Name}) for document {DocumentId}",
                    s.Number, s.Name, message.DocumentId);
                continue;
            }

            if (statute.LegalBranch == null && ruling.LegalBranch != null)
                statute.LegalBranch = ruling.LegalBranch;

            StatuteClassifier.ClassifyIfNeeded(statute);

            ruling.RulingStatutes.Add(new RulingStatute
            {
                Ruling = ruling,
                Statute = statute,
                Articles = s.Articles
            });
        }

        foreach (var c in message.Citations)
        {
            var citationType = Enum.TryParse<CitationType>(c.CitationType, true, out var ct) ? ct : CitationType.CITES;
            var citation = new Citation
            {
                SourceRulingId = ruling.Id,
                ExternalAlias = c.ExternalAlias,
                CsjnSummaryId = c.CsjnSummaryId,
                CsjnFalloId = c.CsjnFalloId,
                CitationText = c.CitationText,
                CitationType = citationType,
                TargetRulingId = null
            };
            await _citationRepository.AddAsync(citation, cancellationToken);
        }

        if (message.CitedBy is { Count: > 0 })
        {
            var sourceIdsByAnalysis =
                await _rulingRepository.FindRulingIdsByAnalysisIdsAsync(
                    message.CitedBy.Select(cb => cb.AnalysisId),
                    cancellationToken);

            foreach (var cb in message.CitedBy)
            {
                if (!sourceIdsByAnalysis.TryGetValue(cb.AnalysisId, out var sourceRulingId))
                    continue;

                var reverseCitation = new Citation
                {
                    SourceRulingId = sourceRulingId,
                    TargetRulingId = ruling.Id,
                    ExternalAlias = cb.CaseNumber,
                    CitationType = CitationType.CITES
                };
                await _citationRepository.AddAsync(reverseCitation, cancellationToken);
                _logger.LogDebug("Created reverse citation from {SourceId} to {TargetId}",
                    sourceRulingId, ruling.Id);
            }
        }

        await _rulingRepository.AddAsync(ruling, cancellationToken);

        if (message.ProsecutorOpinion is { } po)
        {
            var opinion = new ProsecutorOpinion
            {
                RulingId = ruling.Id,
                ProsecutorName = po.ProsecutorName,
                Summary = po.Summary,
                RecommendedDirection = po.RecommendedDirection,
                AgreedWithCourt = po.AgreedWithCourt,
                DocumentBlobPath = po.DocumentUrl,
                ExtractedAt = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(po.ProsecutorName))
            {
                var prosecutorPerson = await _personRepository.GetOrCreateAsync(
                    po.ProsecutorName, "", po.ProsecutorName, null, cancellationToken);
                opinion.PersonId = prosecutorPerson.Id;
                opinion.Person = prosecutorPerson;

                ruling.RulingParticipations.Add(new RulingParticipation
                {
                    Ruling = ruling,
                    Person = prosecutorPerson,
                    Role = RulingRole.PROSECUTOR
                });
            }

            _context.ProsecutorOpinions.Add(opinion);
        }

        // Votes: create Vote entities and link RulingParticipations
        if (message.Votes is { Count: > 0 })
        {
            foreach (var vd in message.Votes)
            {
                var voteType = Enum.TryParse<VoteType>(vd.VoteType, true, out var vt) ? vt : VoteType.Majority;
                var vote = new Vote
                {
                    RulingId = ruling.Id,
                    VoteType = voteType,
                    Pages = vd.Pages,
                    Summary = vd.Summary,
                    Ruling = ruling
                };
                ruling.Votes.Add(vote);

                foreach (var sig in vd.Signatories)
                {
                    var participation = ruling.RulingParticipations
                        .FirstOrDefault(rp => rp.Person.CsjnMinistroId == sig.CsjnMinistroId && sig.CsjnMinistroId.HasValue)
                        ?? ruling.RulingParticipations
                            .FirstOrDefault(rp => rp.Person.LastName == sig.LastName);

                    if (participation != null)
                        participation.Vote = vote;
                }
            }
        }

        // Sumarios: create Sumario + SumarioKeyword
        if (message.Sumarios is { Count: > 0 })
        {
            foreach (var sd in message.Sumarios)
            {
                var sumario = new Sumario
                {
                    RulingId = ruling.Id,
                    ExternalId = sd.ExternalId,
                    Text = sd.Text,
                    Volume = sd.Volume,
                    Page = sd.Page,
                    SortOrder = sd.SortOrder,
                    Ruling = ruling
                };
                ruling.Sumarios.Add(sumario);

                foreach (var kd in sd.Keywords)
                {
                    var keyword = keywordByLookup[new KeywordLookupKey(kd.ExternalCode, kd.Description)];
                    sumario.SumarioKeywords.Add(new SumarioKeyword
                    {
                        Sumario = sumario,
                        Keyword = keyword,
                        SortOrder = kd.SortOrder
                    });
                }
            }
        }

        // Syntheses
        if (message.Syntheses is { Count: > 0 })
        {
            foreach (var sd in message.Syntheses)
            {
                ruling.Syntheses.Add(new RulingSynthesis
                {
                    RulingId = ruling.Id,
                    Text = sd.Text,
                    SortOrder = sd.SortOrder,
                    Ruling = ruling
                });
            }
        }

        // Links
        if (message.Links is { Count: > 0 })
        {
            foreach (var ld in message.Links)
            {
                ruling.Links.Add(new RulingLink
                {
                    RulingId = ruling.Id,
                    Url = ld.Url,
                    Title = ld.Title,
                    LinkType = ld.LinkType,
                    Ruling = ruling
                });
            }
        }

        // Structured statute articles
        foreach (var rs in ruling.RulingStatutes)
        {
            var statuteData = message.Statutes.FirstOrDefault(s =>
                s.Number == rs.Statute.Number && s.Name == rs.Statute.Name);
            if (statuteData?.StructuredArticles is { Count: > 0 })
            {
                foreach (var sa in statuteData.StructuredArticles)
                {
                    rs.StructuredArticles.Add(new RulingStatuteArticle
                    {
                        RulingStatute = rs,
                        Article = sa.Article,
                        Subsection = sa.Subsection
                    });
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        await LinkToProceedingAsync(ruling, message.Parties, message.LegalRepresentations, cancellationToken);
        await RecordAuditTrailAsync(ruling, message, cancellationToken);

        _logger.LogInformation(
            "Persisted ruling {RulingId} for document {DocumentId}",
            ruling.Id,
            message.DocumentId);

        var courtMeta = new CourtMetadata(
            court.CourtCategory?.ToString(),
            court.Fuero?.ToString(),
            court.InstanceLevel);

        return new PersistRulingResult(ruling.Id, court.Name, courtMeta);
    }

    private async Task LinkToProceedingAsync(
        Ruling ruling,
        IReadOnlyList<PartyData>? parties,
        IReadOnlyList<LegalRepresentationData>? legalRepresentations,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ruling.CaseNumber))
            return;

        try
        {
            var existing = await _proceedingRepository.FindByCaseNumberAsync(
                ruling.CaseNumber, ruling.JurisdictionArea, cancellationToken);

            JudicialProceeding proceeding;
            if (existing is not null)
            {
                proceeding = existing;
                ruling.JudicialProceedingId = existing.Id;
                existing.RulingCount++;
                if (existing.FirstRulingDate is null || ruling.RulingDate < existing.FirstRulingDate)
                    existing.FirstRulingDate = ruling.RulingDate;
                if (existing.LastRulingDate is null || ruling.RulingDate > existing.LastRulingDate)
                    existing.LastRulingDate = ruling.RulingDate;
            }
            else
            {
                proceeding = new JudicialProceeding
                {
                    CaseNumber = ruling.CaseNumber,
                    DisplayName = ruling.CaseTitle,
                    JurisdictionArea = ruling.JurisdictionArea,
                    RulingCount = 1,
                    FirstRulingDate = ruling.RulingDate,
                    LastRulingDate = ruling.RulingDate,
                    CreatedAt = DateTime.UtcNow
                };
                await _proceedingRepository.AddAsync(proceeding, cancellationToken);
                await _proceedingRepository.SaveChangesAsync(cancellationToken);
                ruling.JudicialProceedingId = proceeding.Id;
            }

            // Persist proceeding parties
            if (parties is { Count: > 0 })
            {
                var partyPersonMap = new Dictionary<string, Person>(StringComparer.OrdinalIgnoreCase);
                foreach (var pd in parties)
                {
                    var role = Enum.TryParse<PartyRole>(pd.PartyRole, true, out var pr) ? pr : PartyRole.PLAINTIFF;
                    var personType = pd.PersonType.Equals("Legal", StringComparison.OrdinalIgnoreCase)
                        ? PersonType.LegalPublic : PersonType.Physical;

                    var person = await _personRepository.GetOrCreateAsync(pd.Name, null, pd.Name, null, cancellationToken);
                    if (person.PersonType == PersonType.Physical && personType != PersonType.Physical)
                        person.PersonType = personType;

                    partyPersonMap[pd.Name] = person;

                    _context.Set<ProceedingParty>().Add(new ProceedingParty
                    {
                        JudicialProceedingId = proceeding.Id,
                        Person = person,
                        Role = role
                    });
                }

                // Persist legal representations
                if (legalRepresentations is { Count: > 0 })
                {
                    foreach (var lr in legalRepresentations)
                    {
                        var lawyer = await _personRepository.GetOrCreateAsync(lr.LawyerName, null, lr.LawyerName, null, cancellationToken);
                        if (partyPersonMap.TryGetValue(lr.PartyName, out var partyPerson))
                        {
                            _context.Set<LegalRepresentation>().Add(new LegalRepresentation
                            {
                                LawyerPerson = lawyer,
                                PartyPerson = partyPerson,
                                JudicialProceedingId = proceeding.Id
                            });
                        }
                    }
                }

                _logger.LogDebug("Persisted {PartyCount} parties for proceeding {ProceedingId}",
                    parties.Count, proceeding.Id);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogDebug("Linked ruling {RulingId} to proceeding {ProceedingId} (CaseNumber={CaseNumber})",
                ruling.Id, ruling.JudicialProceedingId, ruling.CaseNumber);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to link ruling {RulingId} to proceeding (non-fatal)", ruling.Id);
        }
    }

    private async Task RecordAuditTrailAsync(
        Ruling ruling, IndexerMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var changes = BuildRulingFieldChanges(message);
            var jobId = message.IngestionJobId ?? Guid.Empty;

            var personCount = message.Persons.Count;
            var keywordCount = message.Keywords.Count;
            var statuteCount = message.Statutes.Count;
            var citationCount = message.Citations.Count;

            await _auditService.RecordFullAuditAsync(
                entityType: nameof(Ruling),
                entityId: ruling.Id.ToString(),
                changes: changes,
                sourceId: message.SourceId,
                ingestionJobId: jobId,
                operation: AuditOperation.Created,
                performedBy: "IndexerWorker",
                fieldsChanged: changes.Select(c => c.FieldName).ToList(),
                changeSummary: $"Indexed ruling with {personCount} persons, {keywordCount} keywords, {statuteCount} statutes, {citationCount} citations",
                entitiesCreated: 1,
                fieldsUpdated: changes.Count,
                cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to record audit trail for ruling {RulingId} (non-fatal)", ruling.Id);
        }
    }

    public static List<FieldChange> BuildRulingFieldChanges(IndexerMessage message)
    {
        var r = message.Ruling;
        var changes = new List<FieldChange>();

        AddIfPresent(changes, nameof(r.CaseTitle), r.CaseTitle, "abrirAnalisis", "caratula");
        AddIfPresent(changes, nameof(r.CaseNumber), r.CaseNumber, "abrirAnalisis", "expediente");
        AddIfPresent(changes, nameof(r.RulingDate), r.RulingDate.ToString("o"), "abrirAnalisis", "fecha");
        AddIfPresent(changes, nameof(r.Summary), r.Summary, "abrirAnalisis", "sumario");
        AddIfPresent(changes, nameof(r.Holding), r.Holding, "abrirAnalisis", "resolutivo");
        AddIfPresent(changes, nameof(r.JurisdictionArea), r.JurisdictionArea, "abrirAnalisis", "jurisdiccion");
        AddIfPresent(changes, nameof(r.Instance), r.Instance, "abrirAnalisis", "instancia");
        AddIfPresent(changes, nameof(r.RulingDirection), r.RulingDirection, "abrirAnalisis", "sentido");
        AddIfPresent(changes, nameof(r.SubjectArea), r.SubjectArea, "abrirAnalisis", "materia");
        AddIfPresent(changes, nameof(r.ActionType), r.ActionType, "abrirAnalisis", "tipoAccion");
        AddIfPresent(changes, nameof(r.LegalBranch), r.LegalBranch, "abrirAnalisis", "rama");
        AddIfPresent(changes, nameof(r.OfficialReference), r.OfficialReference, "abrirAnalisis", "referencia");

        return changes;

        static void AddIfPresent(
            List<FieldChange> list, string fieldName, string? value,
            string endpoint, string sourceField)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                list.Add(new FieldChange(
                    FieldName: fieldName,
                    Value: value,
                    PreviousValue: null,
                    InferenceMethod: InferenceMethod.SourceApi,
                    ChangeType: ChangeType.Create,
                    SourceEndpoint: endpoint,
                    SourceField: sourceField));
            }
        }
    }

    internal static void ClassifyCourtIfNeeded(Court court, string? instance)
    {
        if (court.CourtCategory is not null)
            return;

        var name = court.Name;
        if (name.Contains("Corte Suprema", StringComparison.OrdinalIgnoreCase) || instance == "CSJN")
        {
            court.CourtCategory = CourtType.CSJN;
            court.InstanceLevel = 3;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
            court.Fuero = Fuero.FEDERAL;
        }
        else if (name.Contains("Cámara Nacional", StringComparison.OrdinalIgnoreCase))
        {
            court.CourtCategory = CourtType.CAM_NAC;
            court.InstanceLevel = 2;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
        }
        else if (name.Contains("Cámara Federal", StringComparison.OrdinalIgnoreCase))
        {
            court.CourtCategory = CourtType.CAM_FED;
            court.InstanceLevel = 2;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
            court.Fuero = Fuero.FEDERAL;
        }
        else if (name.Contains("Casación", StringComparison.OrdinalIgnoreCase))
        {
            court.CourtCategory = CourtType.CAM_CAS;
            court.InstanceLevel = 2;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
        }
        else if (name.Contains("Tribunal Oral", StringComparison.OrdinalIgnoreCase))
        {
            court.CourtCategory = CourtType.TOF;
            court.InstanceLevel = 1;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
            court.Fuero = Fuero.CRIMINAL;
        }
        else if (name.Contains("Juzgado Nacional", StringComparison.OrdinalIgnoreCase))
        {
            court.CourtCategory = CourtType.JUZ_NAC;
            court.InstanceLevel = 1;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
        }
        else if (name.Contains("Juzgado Federal", StringComparison.OrdinalIgnoreCase))
        {
            court.CourtCategory = CourtType.JUZ_FED;
            court.InstanceLevel = 1;
            court.GovernmentLevel = GovernmentLevel.NATIONAL;
            court.Fuero = Fuero.FEDERAL;
        }
    }

    private static readonly HashSet<string> JunkKeywordPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        "(.)", "(*)", "(**)", "(***)", "(****)",
        ".", "*", "**", "***", "****"
    };

    /// <summary>
    /// CSJN uses asterisk/dot markers as relevance indicators in summaries, not real keywords.
    /// </summary>
    private static bool IsJunkKeyword(string? description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return true;
        var trimmed = description.Trim();
        return trimmed.Length <= 6 && JunkKeywordPatterns.Contains(trimmed);
    }

    private static T? ParseEnum<T>(string? value) where T : struct, Enum
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;
        return Enum.TryParse<T>(value, ignoreCase: true, out var result) ? result : null;
    }

    private async Task<IReadOnlyDictionary<KeywordLookupKey, Keyword>> PrefetchKeywordsForMessageAsync(
        IndexerMessage message,
        CancellationToken cancellationToken)
    {
        var keys = new List<KeywordLookupKey>();
        foreach (var k in message.Keywords)
        {
            if (IsJunkKeyword(k.Description))
                continue;
            keys.Add(new KeywordLookupKey(k.ExternalCode, k.Description));
        }

        if (message.Sumarios is { Count: > 0 })
        {
            foreach (var sd in message.Sumarios)
            {
                foreach (var kd in sd.Keywords)
                {
                    if (IsJunkKeyword(kd.Description))
                        continue;
                    keys.Add(new KeywordLookupKey(kd.ExternalCode, kd.Description));
                }
            }
        }

        if (keys.Count == 0)
            return new Dictionary<KeywordLookupKey, Keyword>();

        return await _keywordRepository.GetOrCreateBatchAsync(keys, cancellationToken);
    }
}

public record PersistRulingResult(Guid RulingId, string CourtName, CourtMetadata? CourtMetadata);
