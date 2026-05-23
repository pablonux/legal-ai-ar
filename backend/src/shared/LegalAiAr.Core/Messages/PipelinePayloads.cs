namespace LegalAiAr.Core.Messages;

/// <summary>
/// Complete entity payload serialized to blob by the Enricher and deserialized by the Persister.
/// Contains all the data needed to persist the entity graph to SQL.
/// </summary>
public record PersisterPayload(
    RulingData? Ruling,
    IReadOnlyList<PersonData> Persons,
    IReadOnlyList<KeywordData> Keywords,
    IReadOnlyList<StatuteData> Statutes,
    IReadOnlyList<CitationData> Citations,
    IReadOnlyList<ChunkData> Chunks,
    string? AnalysisId = null,
    IReadOnlyList<CitedByData>? CitedBy = null,
    ProsecutorOpinionData? ProsecutorOpinion = null,
    IReadOnlyList<VoteData>? Votes = null,
    IReadOnlyList<SumarioData>? Sumarios = null,
    IReadOnlyList<SynthesisData>? Syntheses = null,
    IReadOnlyList<LinkData>? Links = null,
    IReadOnlyList<PartyData>? Parties = null,
    IReadOnlyList<LegalRepresentationData>? LegalRepresentations = null,
    StatutePayloadData? Statute = null);

/// <summary>
/// Statute payload for persistence. Used when EntityType is Statute.
/// </summary>
public record StatutePayloadData(
    string Number,
    string Name,
    string? NormType,
    string? NormativeLevel,
    string? IssuingBody,
    DateOnly? SanctionDate,
    DateOnly? PublicationDate,
    string? Status,
    string? FullText,
    string? SaijId,
    string? OfficialUrl);

/// <summary>
/// Ruling metadata for persistence.
/// </summary>
public record RulingData(
    string CaseTitle,
    DateOnly RulingDate,
    string? CaseNumber,
    string? JurisdictionArea,
    string? Instance,
    string? Jurisdiction,
    string? ResourceType,
    string? RulingDirection,
    string? SubjectArea,
    bool IsUnconstitutional,
    string? Summary,
    string? Holding,
    string FullText,
    string BlobPath,
    string? Court = null,
    string? LegalBranch = null,
    string? PrecedentWeight = null,
    bool IsPlenario = false,
    bool IsLeadingCase = false,
    string? ActionType = null,
    string? InternalSubject = null,
    string? OfficialReference = null,
    string? Observations = null,
    string? FederalQuestion = null,
    string? ProceduralFormula = null,
    bool HasDictamen = false);

/// <summary>
/// Person data for persistence.
/// </summary>
public record PersonData(
    string FirstName,
    string LastName,
    string RulingRole,
    int? CsjnMinistroId = null);

/// <summary>
/// Keyword data for persistence.
/// </summary>
public record KeywordData(
    int? ExternalCode,
    string Description,
    int SortOrder);

/// <summary>
/// Statute data for persistence.
/// </summary>
public record StatuteData(
    string Number,
    string Name,
    string? Articles,
    IReadOnlyList<StatuteArticleData>? StructuredArticles = null);

/// <summary>
/// Structured reference to a specific article and optional subsection of a statute.
/// </summary>
public record StatuteArticleData(string Article, string? Subsection);

/// <summary>
/// Citation data for persistence.
/// </summary>
public record CitationData(
    string ExternalAlias,
    int? CsjnSummaryId,
    string CitationType,
    int? CsjnFalloId = null,
    string? CitationText = null);

/// <summary>
/// Reverse citation: a ruling that cites this document.
/// </summary>
public record CitedByData(string AnalysisId, string CaseNumber);

/// <summary>
/// Text chunk for chunk-level embedding and indexing.
/// </summary>
public record ChunkData(int Index, string Text);

/// <summary>
/// Extracted prosecutor opinion data.
/// </summary>
public record ProsecutorOpinionData(
    string ProsecutorName,
    string? Summary,
    string? RecommendedDirection,
    bool AgreedWithCourt = false,
    string? DocumentUrl = null);

/// <summary>
/// Vote within a ruling (majority, dissent, concurrence).
/// </summary>
public record VoteData(
    string VoteType,
    string? Pages,
    string? Summary,
    IReadOnlyList<PersonData> Signatories);

/// <summary>
/// Doctrinal headnote (sumario).
/// </summary>
public record SumarioData(
    int? ExternalId,
    string Text,
    string? Volume,
    string? Page,
    int SortOrder,
    IReadOnlyList<SumarioKeywordData> Keywords);

/// <summary>
/// Keyword associated with a specific sumario.
/// </summary>
public record SumarioKeywordData(int? ExternalCode, string Description, int SortOrder);

/// <summary>
/// Ruling synthesis/summary document.
/// </summary>
public record SynthesisData(string Text, int SortOrder);

/// <summary>
/// External link related to a ruling.
/// </summary>
public record LinkData(string Url, string? Title, string? LinkType);

/// <summary>
/// Proceeding party extracted from caratula or full text.
/// </summary>
public record PartyData(
    string Name,
    string PartyRole,
    string PersonType = "Physical");

/// <summary>
/// Legal representation: a lawyer represents a party in the proceeding.
/// </summary>
public record LegalRepresentationData(
    string LawyerName,
    string PartyName);
