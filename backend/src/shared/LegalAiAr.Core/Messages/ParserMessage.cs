using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for queue-parser. Contains document metadata and blob path for PDF.
/// Fetcher uploads the PDF to Blob and provides BlobPathPdf.
/// </summary>
public record ParserMessage(
    int SourceId,
    string DocumentId,
    string? AnalysisId,
    string BlobPathPdf,
    string ContentHash,
    CsjnApiMetadata? ApiMetadata,
    Guid? IngestionJobId = null,
    bool UseCache = false,
    bool Reprocess = false,
    DateOnly? RulingDateHint = null,
    string? CaseNumberHint = null,
    Guid? PipelineDocumentId = null,
    EntityType EntityType = EntityType.Ruling);

/// <summary>
/// Metadata from CSJN API endpoints (8 endpoints).
/// </summary>
public record CsjnApiMetadata(
    string CaseTitle,
    DateOnly RulingDate,
    string? CaseNumber,
    string? Jurisdiction,
    string? ResourceType,
    string? RulingDirection,
    string? SubjectArea,
    bool IsUnconstitutional,
    string? Summary,
    string? Holding,
    IReadOnlyList<CsjnKeywordDto> Keywords,
    IReadOnlyList<CsjnCitationDto> Citations,
    IReadOnlyList<CsjnCitedByDto> CitedBy,
    string? ActionType = null,
    string? InternalSubject = null,
    string? OfficialReference = null,
    string? Observations = null,
    string? FederalQuestion = null,
    string? ProceduralFormula = null,
    bool HasDictamen = false,
    IReadOnlyList<CsjnVoteDto>? Votes = null,
    IReadOnlyList<CsjnApiStatuteDto>? ApiStatutes = null,
    IReadOnlyList<CsjnSumarioDto>? Sumarios = null,
    IReadOnlyList<CsjnSintesisDto>? Syntheses = null,
    IReadOnlyList<CsjnEnlaceDto>? Links = null,
    IReadOnlyList<CsjnDictamenDto>? Dictamenes = null);

/// <summary>Keyword from CSJN API.</summary>
public record CsjnKeywordDto(int ExternalCode, string Description);

/// <summary>Citation to another ruling from CSJN API.</summary>
public record CsjnCitationDto(string Alias, int? SummaryId, int? FalloId = null, string? CitationText = null);

/// <summary>Ruling that cites this document (from getCitantes HTML).</summary>
public record CsjnCitedByDto(string AnalysisId, string CaseNumber);

/// <summary>Structured vote from abrirAnalisis.votosAnalisisDocumental.</summary>
public record CsjnVoteDto(
    string VoteType,
    string Judges,
    string? Pages = null,
    IReadOnlyList<CsjnMinistroDto>? Ministers = null);

/// <summary>Individual minister from votosAnalisisDocumental.ministros[].</summary>
public record CsjnMinistroDto(int MinistroId, string Surname);

/// <summary>Normative reference from abrirAnalisis.referenciasNormativas.</summary>
public record CsjnApiStatuteDto(string? NormType, string? Number, string? Description, string? Article, string? Subsection);

/// <summary>Doctrinal headnote from CSJN getSumariosAnalisis.</summary>
public record CsjnSumarioDto(
    int? Id,
    string Text,
    string? Volume,
    string? Page,
    int SortOrder,
    IReadOnlyList<CsjnKeywordDto> Keywords);

/// <summary>Synthesis/summary document from CSJN getSintesisAnalisis.</summary>
public record CsjnSintesisDto(string Text, int SortOrder);

/// <summary>External link from CSJN getEnlacesAnalisis.</summary>
public record CsjnEnlaceDto(string Url, string? Description, bool IsInternal);

/// <summary>Prosecutor opinion document from CSJN getDictamenesAnalisis.</summary>
public record CsjnDictamenDto(string? Title, string? DocumentUrl, string? DocumentType);
