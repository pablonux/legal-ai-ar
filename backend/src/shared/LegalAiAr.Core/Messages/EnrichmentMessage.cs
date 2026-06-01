using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for queue-enrichment. Contains extracted metadata and fields that need GPT-5 extraction.
/// </summary>
public record EnrichmentMessage(
    string DocumentId,
    int SourceId,
    string NormalizedText,
    ExtractedMetadata ExtractedMetadata,
    IReadOnlyList<string> MissingFields,
    string? BlobPath = null,
    string? ContentHash = null,
    string? TextBlobPath = null,
    string? MetadataBlobPath = null,
    Guid? IngestionJobId = null,
    string? AnalysisId = null,
    bool Reprocess = false,
    bool UseCache = false,
    Guid? PipelineDocumentId = null,
    EntityType EntityType = EntityType.Ruling);

/// <summary>
/// Metadata extracted before enrichment (from API or HTML scraping).
/// </summary>
public record ExtractedMetadata(
    string CaseTitle,
    DateOnly RulingDate,
    string? CaseNumber,
    string? Court,
    string? JurisdictionArea,
    string? Instance,
    string? RulingDirection,
    string? Summary,
    string? Holding,
    IReadOnlyList<ExtractedKeywordDto> Keywords,
    IReadOnlyList<ExtractedCitationDto> Citations,
    string? Jurisdiction = null,
    string? ResourceType = null,
    string? SubjectArea = null,
    bool IsUnconstitutional = false,
    IReadOnlyList<ExtractedCitedByDto>? CitedBy = null,
    string? ActionType = null,
    string? InternalSubject = null,
    string? OfficialReference = null,
    string? Observations = null,
    string? FederalQuestion = null,
    string? ProceduralFormula = null,
    bool HasDictamen = false,
    IReadOnlyList<ExtractedPersonDto>? ApiPersons = null,
    IReadOnlyList<ExtractedStatuteDto>? ApiStatutes = null,
    IReadOnlyList<CsjnVoteDto>? ApiVotes = null,
    IReadOnlyList<ExtractedSumarioDto>? Sumarios = null,
    IReadOnlyList<ExtractedSynthesisDto>? Syntheses = null,
    IReadOnlyList<ExtractedLinkDto>? Links = null,
    IReadOnlyList<ExtractedDictamenDto>? Dictamenes = null);

/// <summary>Keyword with optional external code. ExternalCode null for Phase 2 sources.</summary>
public record ExtractedKeywordDto(int? ExternalCode, string Description);

/// <summary>Citation with alias. SummaryId for CSJN linking.</summary>
public record ExtractedCitationDto(string Alias, int? SummaryId = null, int? FalloId = null, string? CitationText = null);

/// <summary>Reverse citation: a ruling that cites the current document.</summary>
public record ExtractedCitedByDto(string AnalysisId, string CaseNumber);

/// <summary>Person extracted from CSJN API structured votes.</summary>
public record ExtractedPersonDto(string Name, string RulingRole, int? CsjnMinistroId = null);

/// <summary>Statute extracted from CSJN API referenciasNormativas.</summary>
public record ExtractedStatuteDto(
    string? Number,
    string? Name,
    string? Articles,
    string? Article = null,
    string? Subsection = null);

/// <summary>Doctrinal headnote extracted from CSJN getSumariosAnalisis.</summary>
public record ExtractedSumarioDto(
    int? ExternalId,
    string Text,
    string? Volume,
    string? Page,
    int SortOrder,
    IReadOnlyList<ExtractedKeywordDto> Keywords);

/// <summary>Synthesis document extracted from CSJN getSintesisAnalisis.</summary>
public record ExtractedSynthesisDto(string Text, int SortOrder);

/// <summary>External link extracted from CSJN getEnlacesAnalisis.</summary>
public record ExtractedLinkDto(string Url, string? Title, string? LinkType);

/// <summary>Dictamen from CSJN getDictamenesAnalisis.</summary>
public record ExtractedDictamenDto(string? Title, string? DocumentUrl, string? DocumentType);
