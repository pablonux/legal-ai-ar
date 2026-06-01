using System.Diagnostics;
using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Models;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Persister.Strategies;

/// <summary>
/// Persists the ruling graph from <see cref="LegalAiAr.Core.Messages.PersisterPayload"/> (court, ruling, related entities, citations).
/// </summary>
/// <remarks>
/// <para><b>Order of work</b> (matches FK and SQL MERGE expectations): build dimension lookups and the ruling
/// subgraph <i>without</i> outbound citations → insert ruling (<see cref="IRulingRepository.AddAsync"/>) →
/// first <see cref="DbContext.SaveChangesAsync"/> → attach outbound citations and save again → structured
/// articles and prosecutor opinion → proceedings/parties → cited-by (reverse citations, **one** <c>SaveChanges</c> al final).</para>
/// <para>Duplicate keyword rows in the payload are skipped so <c>RulingKeywords</c> does not violate PK.</para>
/// <para>Deferring parts of persistence until after indexing is a separate pipeline design topic.</para>
/// </remarks>
public sealed class RulingPersistStrategy : IPersistStrategy
{
    private readonly AppDbContext _context;
    private readonly IRulingRepository _rulingRepository;
    private readonly ICourtRepository _courtRepository;
    private readonly IPersonRepository _personRepository;
    private readonly IKeywordRepository _keywordRepository;
    private readonly IStatuteRepository _statuteRepository;
    private readonly ICitationRepository _citationRepository;
    private readonly IJudicialProceedingRepository _proceedingRepository;
    private readonly IKeywordNormalizationService _keywordNormalizer;
    private readonly IBlobStorageService _blobStorage;
    private readonly IRulingReprocessRequestRepository _reprocessRequests;
    private readonly ILogger<RulingPersistStrategy> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RulingPersistStrategy(
        AppDbContext context,
        IRulingRepository rulingRepository,
        ICourtRepository courtRepository,
        IPersonRepository personRepository,
        IKeywordRepository keywordRepository,
        IStatuteRepository statuteRepository,
        ICitationRepository citationRepository,
        IJudicialProceedingRepository proceedingRepository,
        IKeywordNormalizationService keywordNormalizer,
        IBlobStorageService blobStorage,
        IRulingReprocessRequestRepository reprocessRequests,
        ILogger<RulingPersistStrategy> logger)
    {
        _context = context;
        _rulingRepository = rulingRepository;
        _courtRepository = courtRepository;
        _personRepository = personRepository;
        _keywordRepository = keywordRepository;
        _statuteRepository = statuteRepository;
        _citationRepository = citationRepository;
        _proceedingRepository = proceedingRepository;
        _keywordNormalizer = keywordNormalizer;
        _blobStorage = blobStorage;
        _reprocessRequests = reprocessRequests;
        _logger = logger;
    }

    private void AssignKeywordLink(RulingKeyword link, Keyword keyword)
    {
        if (_context.Entry(keyword).State == EntityState.Added)
            link.Keyword = keyword;
        else
            link.KeywordId = keyword.Id;
    }

    private void AssignSumarioKeywordLink(SumarioKeyword link, Keyword keyword)
    {
        if (_context.Entry(keyword).State == EntityState.Added)
            link.Keyword = keyword;
        else
            link.KeywordId = keyword.Id;
    }

    private void AssignStatuteLink(RulingStatute rs, Statute statute)
    {
        if (_context.Entry(statute).State == EntityState.Added)
            rs.Statute = statute;
        else
            rs.StatuteId = statute.Id;
    }

    private void AssignPersonParticipation(RulingParticipation rp, Person person)
    {
        if (_context.Entry(person).State == EntityState.Added)
            rp.Person = person;
        else
            rp.PersonId = person.Id;
    }

    private void AssignProceedingPartyPerson(ProceedingParty pp, Person person)
    {
        if (_context.Entry(person).State == EntityState.Added)
            pp.Person = person;
        else
            pp.PersonId = person.Id;
    }

    private void AssignLegalRepresentationPersons(LegalRepresentation lr, Person lawyer, Person party)
    {
        if (_context.Entry(lawyer).State == EntityState.Added)
            lr.LawyerPerson = lawyer;
        else
            lr.LawyerPersonId = lawyer.Id;

        if (_context.Entry(party).State == EntityState.Added)
            lr.PartyPerson = party;
        else
            lr.PartyPersonId = party.Id;
    }

    public async Task<PersistResult> PersistAsync(
        PersisterMessage message,
        CancellationToken cancellationToken = default)
    {
        var hashPrefix = message.ContentHash is { Length: > 0 }
            ? message.ContentHash[..Math.Min(8, message.ContentHash.Length)]
            : "";

        var sw = Stopwatch.StartNew();
        var payload = await DownloadPayloadAsync(message.PayloadBlobPath!, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix}",
            "BlobJsonPayload", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix);

        if (payload.Ruling is null)
            throw new InvalidOperationException("PersisterPayload.Ruling is null for a Ruling entity");

        var rulingData = payload.Ruling;

        sw.Restart();
        var existingRuling = await _rulingRepository.GetByContentHashAsync(message.ContentHash, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix}",
            "ContentHashLookup", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix);

        var activeReprocess = message.Reprocess
            ? await _reprocessRequests.GetActiveByDocumentIdAsync(message.DocumentId, cancellationToken)
            : null;
        var inPlaceRulingId = activeReprocess?.RulingId;

        if (existingRuling is not null)
        {
            if (!message.Reprocess)
            {
                _logger.LogInformation("Ruling with ContentHash {Hash} already exists, skipping", message.ContentHash);
                return new PersistResult(existingRuling.Id, IsNew: false);
            }

            if (!inPlaceRulingId.HasValue)
            {
                sw.Restart();
                await DeleteExistingRulingAsync(existingRuling.Id, cancellationToken);
                sw.Stop();
                _logger.LogDebug(
                    "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} ExistingRulingId={ExistingRulingId}",
                    "DeleteExistingForReprocess", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, existingRuling.Id);

                _logger.LogInformation("Deleted existing Ruling {RulingId} for reprocessing (ContentHash={Hash})",
                    existingRuling.Id, message.ContentHash);
            }
        }

        sw.Restart();
        var court = await ResolveCourtAsync(rulingData, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix}",
            "CourtResolve", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix);

        if (inPlaceRulingId.HasValue)
        {
            sw.Restart();
            await ClearRulingDependentsAsync(inPlaceRulingId.Value, cancellationToken);
            sw.Stop();
            _logger.LogInformation(
                "Cleared dependents for in-place reprocess of Ruling {RulingId} (DocId={DocumentId})",
                inPlaceRulingId.Value, message.DocumentId);
        }

        var ruling = BuildRuling(message, rulingData, court, inPlaceRulingId);
        ruling.AnalysisId = payload.AnalysisId;

        sw.Restart();
        var participationPeople = await PersistPersonsAsync(ruling, payload.Persons, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} PersonCount={PersonCount}",
            "Persons", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, payload.Persons.Count);

        sw.Restart();
        var thesaurusResolveCount = await PersistKeywordsAsync(ruling, payload.Keywords, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} KeywordCount={KeywordCount} ThesaurusResolveCount={ThesaurusResolveCount}",
            "Keywords", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, payload.Keywords.Count, thesaurusResolveCount);

        sw.Restart();
        var statuteStructuredPairs = await PersistStatutesAsync(ruling, payload.Statutes, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} StatuteCount={StatuteCount}",
            "Statutes", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, payload.Statutes.Count);

        sw.Restart();
        PersistVotes(ruling, payload.Votes, participationPeople);
        await PersistSumarios(ruling, payload.Sumarios, cancellationToken);
        PersistSyntheses(ruling, payload.Syntheses);
        PersistLinks(ruling, payload.Links);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} VoteCount={VoteCount} SumarioCount={SumarioCount}",
            "RelatedGraph", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, payload.Votes?.Count ?? 0, payload.Sumarios?.Count ?? 0);

        sw.Restart();
        if (inPlaceRulingId.HasValue)
        {
            _context.Rulings.Update(ruling);
            await _context.SaveChangesAsync(cancellationToken);
        }
        else
        {
            await _rulingRepository.AddAsync(ruling, cancellationToken);
        }
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix}",
            inPlaceRulingId.HasValue ? "RulingRepositoryUpdate" : "RulingRepositoryAdd",
            sw.ElapsedMilliseconds, message.DocumentId, hashPrefix);

        sw.Restart();
        PersistCitations(ruling, payload.Citations);
        await _context.SaveChangesAsync(cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} OutboundCitationCount={OutboundCitationCount}",
            "CitationsFirstSave", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, payload.Citations.Count);

        sw.Restart();
        PersistStructuredArticles(statuteStructuredPairs);
        await PersistProsecutorOpinionAsync(ruling, payload.ProsecutorOpinion, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} StructuredStatutePairs={StructuredStatutePairs}",
            "ArticlesOpinionSave", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, statuteStructuredPairs.Count);

        sw.Restart();
        await LinkToProceedingAsync(ruling, payload.Parties, payload.LegalRepresentations, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix}",
            "LinkProceeding", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix);

        sw.Restart();
        await PersistCitedByAsync(ruling, payload.CitedBy, cancellationToken);
        sw.Stop();
        _logger.LogDebug(
            "Persister ruling phase {Phase} {ElapsedMs}ms DocId={DocumentId} HashPrefix={HashPrefix} CitedByCount={CitedByCount}",
            "CitedBy", sw.ElapsedMilliseconds, message.DocumentId, hashPrefix, payload.CitedBy?.Count ?? 0);

        _logger.LogInformation(
            "Persisted Ruling Id={RulingId} ExternalId={ExternalId} Title={Title} " +
            "(persons={Persons}, keywords={Kw}, statutes={St}, citations={Ct}, votes={Vt}, sumarios={Su})",
            ruling.Id, ruling.ExternalId, ruling.CaseTitle,
            payload.Persons.Count, payload.Keywords.Count, payload.Statutes.Count,
            payload.Citations.Count, payload.Votes?.Count ?? 0, payload.Sumarios?.Count ?? 0);

        return new PersistResult(ruling.Id, IsNew: !inPlaceRulingId.HasValue);
    }

    private async Task<Court> ResolveCourtAsync(RulingData rulingData, CancellationToken ct)
    {
        var courtName = !string.IsNullOrWhiteSpace(rulingData.Court)
            ? rulingData.Court
            : !string.IsNullOrWhiteSpace(rulingData.Instance) ? rulingData.Instance : "CSJN";

        var court = await _courtRepository.GetOrCreateAsync(
            name: courtName,
            jurisdictionArea: "Federal",
            territory: "Nacional",
            instance: rulingData.Instance ?? "Única",
            ct);

        ClassifyCourtIfNeeded(court, rulingData.Instance);
        return court;
    }

    private static Ruling BuildRuling(PersisterMessage message, RulingData rulingData, Court court, Guid? forceRulingId = null)
    {
        // Link via Court navigation: new courts have Id=0 until SaveChanges; setting CourtId=court.Id would send FK 0 and break FK_Rulings_Courts_CourtId.
        return new Ruling
        {
            Id = forceRulingId ?? Guid.NewGuid(),
            SourceId = message.SourceId,
            ExternalId = message.DocumentId.ToString(),
            ContentHash = message.ContentHash,
            IngestionJobId = message.IngestionJobId,
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
            LegalBranch = ParseEnum<LegalBranch>(rulingData.LegalBranch),
            PrecedentWeight = ParseEnum<PrecedentWeight>(rulingData.PrecedentWeight),
            IsPlenario = rulingData.IsPlenario,
            IsLeadingCase = rulingData.IsLeadingCase,
            ActionType = rulingData.ActionType,
            InternalSubject = rulingData.InternalSubject,
            OfficialReference = rulingData.OfficialReference,
            Observations = rulingData.Observations,
            FederalQuestion = rulingData.FederalQuestion,
            ProceduralFormula = rulingData.ProceduralFormula,
            HasDictamen = rulingData.HasDictamen,
            Status = RulingStatus.Pending,
            IndexedAt = DateTime.UtcNow,
        };
    }

    private async Task<List<(RulingParticipation Rp, Person Person)>> PersistPersonsAsync(
        Ruling ruling, IReadOnlyList<PersonData> persons, CancellationToken ct)
    {
        var participationPeople = new List<(RulingParticipation Rp, Person Person)>();
        var seenKeys = new HashSet<(string? First, string? Last, int? Ministro)>();
        foreach (var p in persons)
        {
            var displayName = !string.IsNullOrEmpty(p.LastName) && !string.IsNullOrEmpty(p.FirstName)
                ? $"{p.LastName}, {p.FirstName}"
                : !string.IsNullOrEmpty(p.LastName) ? p.LastName : p.FirstName;
            var person = await _personRepository.GetOrCreateAsync(
                displayName, p.FirstName, p.LastName, p.CsjnMinistroId, ct);

            var dedupeKey = (p.FirstName, p.LastName, p.CsjnMinistroId);
            if (!seenKeys.Add(dedupeKey))
                continue;

            var role = Enum.TryParse<RulingRole>(p.RulingRole, true, out var r)
                ? r
                : RulingRole.SIGNATORY;

            var rp = new RulingParticipation
            {
                RulingId = ruling.Id,
                Role = role,
            };
            AssignPersonParticipation(rp, person);
            ruling.RulingParticipations.Add(rp);
            participationPeople.Add((rp, person));
        }

        return participationPeople;
    }

    private static readonly HashSet<string> JunkKeywordPatterns = new(StringComparer.OrdinalIgnoreCase)
    {
        "(.)", "(*)", "(**)", "(***)", "(****)", ".", "*", "**", "***", "****"
    };

    /// <summary>Skips duplicate keywords in the payload (same Id or same pending instance) to satisfy PK on <c>RulingKeywords</c>.</summary>
    private async Task<int> PersistKeywordsAsync(
        Ruling ruling, IReadOnlyList<KeywordData> keywords, CancellationToken ct)
    {
        var thesaurusResolveCount = 0;
        var seenKeywordIds = new HashSet<int>();
        var seenNewKeywordRefs = new HashSet<Keyword>();

        var batchKeys = new List<KeywordLookupKey>();
        foreach (var k in keywords)
        {
            if (string.IsNullOrWhiteSpace(k.Description))
                continue;
            var trimmed = k.Description.Trim();
            if (trimmed.Length <= 6 && JunkKeywordPatterns.Contains(trimmed))
                continue;
            batchKeys.Add(new KeywordLookupKey(k.ExternalCode, k.Description));
        }

        var keywordByKey = batchKeys.Count == 0
            ? new Dictionary<KeywordLookupKey, Keyword>()
            : await _keywordRepository.GetOrCreateBatchAsync(batchKeys, ct);

        foreach (var k in keywords)
        {
            if (string.IsNullOrWhiteSpace(k.Description))
                continue;
            var trimmed = k.Description.Trim();
            if (trimmed.Length <= 6 && JunkKeywordPatterns.Contains(trimmed))
                continue;

            var lookupKey = new KeywordLookupKey(k.ExternalCode, k.Description);
            var keyword = keywordByKey[lookupKey];
            if (keyword.Id != 0)
            {
                if (!seenKeywordIds.Add(keyword.Id))
                    continue;
            }
            else if (!seenNewKeywordRefs.Add(keyword))
            {
                continue;
            }

            if (keyword.ThesaurusTermId == null)
            {
                thesaurusResolveCount++;
                keyword.ThesaurusTermId = await _keywordNormalizer.ResolveAsync(k.Description, ct);
            }

            var rk = new RulingKeyword { SortOrder = k.SortOrder };
            AssignKeywordLink(rk, keyword);
            ruling.RulingKeywords.Add(rk);
        }

        return thesaurusResolveCount;
    }

    private async Task<List<(RulingStatute Rs, StatuteData Data)>> PersistStatutesAsync(
        Ruling ruling, IReadOnlyList<StatuteData> statutes, CancellationToken ct)
    {
        var structuredPairs = new List<(RulingStatute Rs, StatuteData Data)>();
        var seenStatuteIds = new HashSet<int>();
        var pendingNewStatuteRefs = new List<Statute>();

        var pairs = statutes.Select(s => (s.Number, s.Name)).ToList();
        var statuteByNumber = pairs.Count == 0
            ? new Dictionary<string, Statute>(StringComparer.Ordinal)
            : (Dictionary<string, Statute>)(await _statuteRepository.GetOrCreateBatchAsync(pairs, ct));

        foreach (var s in statutes)
        {
            if (!statuteByNumber.TryGetValue(s.Number, out var statute))
                continue;

            if (statute.Name != s.Name)
                statute.Name = s.Name;

            if (statute.Id != 0)
            {
                if (!seenStatuteIds.Add(statute.Id))
                    continue;
            }
            else if (pendingNewStatuteRefs.Contains(statute))
            {
                continue;
            }
            else
            {
                pendingNewStatuteRefs.Add(statute);
            }

            if (statute.LegalBranch == null && ruling.LegalBranch != null)
                statute.LegalBranch = ruling.LegalBranch;

            StatuteClassifier.ClassifyIfNeeded(statute);

            var rs = new RulingStatute { Articles = s.Articles };
            AssignStatuteLink(rs, statute);
            ruling.RulingStatutes.Add(rs);

            if (s.StructuredArticles is { Count: > 0 })
                structuredPairs.Add((rs, s));
        }

        return structuredPairs;
    }

    private static void PersistCitations(Ruling ruling, IReadOnlyList<CitationData> citations)
    {
        foreach (var c in citations)
        {
            var citationType = Enum.TryParse<CitationType>(c.CitationType, true, out var ct)
                ? ct
                : CitationType.CITES;

            // Link via collection only — setting SourceRulingId explicitly can confuse EF's insert order
            // with temporal tables / large graphs and surface FK errors on SourceRulingId.
            ruling.OutboundCitations.Add(new Citation
            {
                ExternalAlias = c.ExternalAlias,
                CsjnSummaryId = c.CsjnSummaryId,
                CsjnFalloId = c.CsjnFalloId,
                CitationText = c.CitationText,
                CitationType = citationType,
            });
        }
    }

    private static void PersistVotes(
        Ruling ruling,
        IReadOnlyList<VoteData>? votes,
        IReadOnlyList<(RulingParticipation Rp, Person Person)> participationPeople)
    {
        if (votes is not { Count: > 0 }) return;

        foreach (var vd in votes)
        {
            var voteType = Enum.TryParse<VoteType>(vd.VoteType, true, out var vt)
                ? vt : VoteType.Majority;
            var vote = new Vote
            {
                RulingId = ruling.Id,
                VoteType = voteType,
                Pages = vd.Pages,
                Summary = vd.Summary,
            };
            ruling.Votes.Add(vote);

            foreach (var sig in vd.Signatories)
            {
                RulingParticipation? participation = null;
                foreach (var (rp, person) in participationPeople)
                {
                    if (sig.CsjnMinistroId.HasValue && person.CsjnMinistroId == sig.CsjnMinistroId)
                    {
                        participation = rp;
                        break;
                    }
                }

                participation ??= participationPeople
                    .FirstOrDefault(t => string.Equals(t.Person.LastName, sig.LastName,
                        StringComparison.OrdinalIgnoreCase)).Rp;

                if (participation != null)
                    participation.Vote = vote;
            }
        }
    }

    private async Task PersistSumarios(
        Ruling ruling, IReadOnlyList<SumarioData>? sumarios, CancellationToken ct)
    {
        if (sumarios is not { Count: > 0 }) return;

        var sumarioKeys = new List<KeywordLookupKey>();
        foreach (var sd in sumarios)
        {
            foreach (var kd in sd.Keywords)
            {
                if (string.IsNullOrWhiteSpace(kd.Description))
                    continue;
                var trimmed = kd.Description.Trim();
                if (trimmed.Length <= 6 && JunkKeywordPatterns.Contains(trimmed))
                    continue;
                sumarioKeys.Add(new KeywordLookupKey(kd.ExternalCode, kd.Description));
            }
        }

        var sumarioKeywordByKey = sumarioKeys.Count == 0
            ? new Dictionary<KeywordLookupKey, Keyword>()
            : (Dictionary<KeywordLookupKey, Keyword>)(await _keywordRepository.GetOrCreateBatchAsync(sumarioKeys, ct));

        foreach (var sd in sumarios)
        {
            var sumario = new Sumario
            {
                RulingId = ruling.Id,
                ExternalId = sd.ExternalId,
                Text = sd.Text,
                Volume = sd.Volume,
                Page = sd.Page,
                SortOrder = sd.SortOrder,
            };
            ruling.Sumarios.Add(sumario);

            foreach (var kd in sd.Keywords)
            {
                if (string.IsNullOrWhiteSpace(kd.Description))
                    continue;
                var trimmed = kd.Description.Trim();
                if (trimmed.Length <= 6 && JunkKeywordPatterns.Contains(trimmed))
                    continue;

                var lookupKey = new KeywordLookupKey(kd.ExternalCode, kd.Description);
                var keyword = sumarioKeywordByKey[lookupKey];
                if (keyword.ThesaurusTermId == null)
                    keyword.ThesaurusTermId = await _keywordNormalizer.ResolveAsync(kd.Description, ct);

                var sk = new SumarioKeyword { SortOrder = kd.SortOrder };
                AssignSumarioKeywordLink(sk, keyword);
                sumario.SumarioKeywords.Add(sk);
            }
        }
    }

    private static void PersistSyntheses(Ruling ruling, IReadOnlyList<SynthesisData>? syntheses)
    {
        if (syntheses is not { Count: > 0 }) return;

        foreach (var sd in syntheses)
        {
            ruling.Syntheses.Add(new RulingSynthesis
            {
                RulingId = ruling.Id,
                Text = sd.Text,
                SortOrder = sd.SortOrder,
            });
        }
    }

    private static void PersistLinks(Ruling ruling, IReadOnlyList<LinkData>? links)
    {
        if (links is not { Count: > 0 }) return;

        foreach (var ld in links)
        {
            ruling.Links.Add(new RulingLink
            {
                RulingId = ruling.Id,
                Url = ld.Url,
                Title = ld.Title,
                LinkType = ld.LinkType,
            });
        }
    }

    private static void PersistStructuredArticles(IReadOnlyList<(RulingStatute Rs, StatuteData Data)> pairs)
    {
        foreach (var (rs, statuteData) in pairs)
        {
            foreach (var sa in statuteData.StructuredArticles!)
            {
                rs.StructuredArticles.Add(new RulingStatuteArticle
                {
                    Article = sa.Article,
                    Subsection = sa.Subsection,
                });
            }
        }
    }

    private async Task PersistProsecutorOpinionAsync(
        Ruling ruling, ProsecutorOpinionData? po, CancellationToken ct)
    {
        if (po is null) return;

        var opinion = new ProsecutorOpinion
        {
            RulingId = ruling.Id,
            ProsecutorName = po.ProsecutorName,
            Summary = po.Summary,
            RecommendedDirection = po.RecommendedDirection,
            AgreedWithCourt = po.AgreedWithCourt,
            DocumentBlobPath = po.DocumentUrl,
            ExtractedAt = DateTime.UtcNow,
        };

        if (!string.IsNullOrWhiteSpace(po.ProsecutorName))
        {
            var prosecutorPerson = await _personRepository.GetOrCreateAsync(
                po.ProsecutorName, "", po.ProsecutorName, null, ct);
            if (_context.Entry(prosecutorPerson).State == EntityState.Added)
                opinion.Person = prosecutorPerson;
            else
                opinion.PersonId = prosecutorPerson.Id;

            var rp = new RulingParticipation
            {
                RulingId = ruling.Id,
                Role = RulingRole.PROSECUTOR,
            };
            AssignPersonParticipation(rp, prosecutorPerson);
            ruling.RulingParticipations.Add(rp);
        }

        _context.ProsecutorOpinions.Add(opinion);
    }

    private async Task PersistCitedByAsync(
        Ruling ruling, IReadOnlyList<CitedByData>? citedBy, CancellationToken ct)
    {
        if (citedBy is not { Count: > 0 }) return;

        var analysisIds = citedBy.Select(cb => cb.AnalysisId).ToList();
        var sourceIdsByAnalysis =
            await _rulingRepository.FindRulingIdsByAnalysisIdsAsync(analysisIds, ct);

        foreach (var cb in citedBy)
        {
            if (!sourceIdsByAnalysis.TryGetValue(cb.AnalysisId, out var sourceRulingId))
                continue;

            if (!_context.Rulings.Local.Any(r => r.Id == sourceRulingId))
            {
                var sourceStub = new Ruling { Id = sourceRulingId };
                _context.Attach(sourceStub);
                _context.Entry(sourceStub).State = EntityState.Unchanged;
            }

            var reverseCitation = new Citation
            {
                SourceRulingId = sourceRulingId,
                TargetRulingId = ruling.Id,
                ExternalAlias = cb.CaseNumber,
                CitationType = CitationType.CITES,
            };
            await _citationRepository.AddAsync(reverseCitation, ct);
        }

        try
        {
            await _context.SaveChangesAsync(ct);
            _logger.LogDebug("Persisted reverse citations (cited-by) for ruling {RulingId}", ruling.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(
                ex,
                "Batch save of reverse citations failed for Ruling {RulingId}: {Error}",
                ruling.Id, ex.InnerException?.Message ?? ex.Message);
            DetachAddedCitationsOnly();
        }
    }

    private void DetachAddedCitationsOnly()
    {
        foreach (var entry in _context.ChangeTracker.Entries<Citation>()
                     .Where(e => e.State == EntityState.Added)
                     .ToList())
        {
            entry.State = EntityState.Detached;
        }
    }

    private async Task LinkToProceedingAsync(
        Ruling ruling,
        IReadOnlyList<PartyData>? parties,
        IReadOnlyList<LegalRepresentationData>? legalRepresentations,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(ruling.CaseNumber))
            return;

        try
        {
            var existing = await _proceedingRepository.FindByCaseNumberAsync(
                ruling.CaseNumber, ruling.JurisdictionArea, ct);

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
                    CreatedAt = DateTime.UtcNow,
                };
                await _proceedingRepository.AddAsync(proceeding, ct);
                await _proceedingRepository.SaveChangesAsync(ct);
                ruling.JudicialProceedingId = proceeding.Id;
            }

            if (parties is { Count: > 0 })
            {
                var partyPersonMap = new Dictionary<string, Person>(StringComparer.OrdinalIgnoreCase);
                foreach (var pd in parties)
                {
                    var role = Enum.TryParse<PartyRole>(pd.PartyRole, true, out var pr)
                        ? pr : PartyRole.PLAINTIFF;
                    var personType = pd.PersonType.Equals("Legal", StringComparison.OrdinalIgnoreCase)
                        ? PersonType.LegalPublic : PersonType.Physical;

                    var person = await _personRepository.GetOrCreateAsync(
                        pd.Name, null, pd.Name, null, ct);
                    if (person.PersonType == PersonType.Physical && personType != PersonType.Physical)
                        person.PersonType = personType;

                    partyPersonMap[pd.Name] = person;

                    var pp = new ProceedingParty
                    {
                        JudicialProceedingId = proceeding.Id,
                        Role = role,
                    };
                    AssignProceedingPartyPerson(pp, person);
                    _context.Set<ProceedingParty>().Add(pp);
                }

                if (legalRepresentations is { Count: > 0 })
                {
                    foreach (var lr in legalRepresentations)
                    {
                        var lawyer = await _personRepository.GetOrCreateAsync(
                            lr.LawyerName, null, lr.LawyerName, null, ct);
                        if (partyPersonMap.TryGetValue(lr.PartyName, out var partyPerson))
                        {
                            var ent = new LegalRepresentation
                            {
                                JudicialProceedingId = proceeding.Id,
                            };
                            AssignLegalRepresentationPersons(ent, lawyer, partyPerson);
                            _context.Set<LegalRepresentation>().Add(ent);
                        }
                    }
                }
            }

            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to link ruling {RulingId} to proceeding (non-fatal)", ruling.Id);
        }
    }

    private async Task DeleteExistingRulingAsync(Guid rulingId, CancellationToken ct)
    {
        await ClearRulingDependentsAsync(rulingId, ct);

        await _context.Rulings
            .Where(r => r.Id == rulingId)
            .ExecuteDeleteAsync(ct);
    }

    private async Task ClearRulingDependentsAsync(Guid rulingId, CancellationToken ct)
    {
        await _context.Citations
            .Where(c => c.SourceRulingId == rulingId || c.TargetRulingId == rulingId)
            .ExecuteDeleteAsync(ct);

        await _context.RulingParticipations.Where(rp => rp.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.RulingKeywords.Where(rk => rk.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.RulingStatutes.Where(rs => rs.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.Votes.Where(v => v.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.Sumarios.Where(s => s.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.RulingSyntheses.Where(s => s.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.Set<RulingLink>().Where(l => l.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.Set<ProsecutorOpinion>().Where(p => p.RulingId == rulingId).ExecuteDeleteAsync(ct);
        await _context.RulingEmbeddingStates.Where(s => s.RulingId == rulingId).ExecuteDeleteAsync(ct);
    }

    private async Task<PersisterPayload> DownloadPayloadAsync(string blobPath, CancellationToken ct)
    {
        await using var stream = await _blobStorage.DownloadAsync(blobPath, ct);
        var payload = await JsonSerializer.DeserializeAsync<PersisterPayload>(stream, JsonOptions, ct);
        return payload ?? throw new InvalidOperationException($"Failed to deserialize PersisterPayload from {blobPath}");
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

    private static T? ParseEnum<T>(string? value) where T : struct, Enum =>
        !string.IsNullOrWhiteSpace(value) && Enum.TryParse<T>(value, true, out var result)
            ? result : null;
}
