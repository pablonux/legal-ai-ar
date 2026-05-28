using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for the fetcher queue. Instructs the Fetcher to download content for a single document.
/// Published by the Discoverer for each discovered document.
/// </summary>
/// <param name="DocumentId">Document.Id (pipeline correlation ID).</param>
/// <param name="EntityType">Type of entity being fetched.</param>
/// <param name="SourceId">Source the document belongs to.</param>
/// <param name="ExternalId">External document ID from the source.</param>
/// <param name="AnalysisId">Source-specific analysis ID. Null for non-CSJN sources.</param>
/// <param name="IngestionJobId">Job that originated this document.</param>
/// <param name="UseCache">When true, check download cache before HTTP calls.</param>
/// <param name="Reprocess">When true, skip deduplication checks.</param>
/// <param name="AcuerdoDate">CSJN acuerdo date hint for the ruling date.</param>
/// <param name="CaseNumber">Case number hint from discovery.</param>
/// <param name="FetchPdfTimeoutSeconds">Optional HTTP timeout for this PDF fetch only (seconds). Null uses the worker default.</param>
public record FetcherMessage(
    Guid DocumentId,
    EntityType EntityType,
    int SourceId,
    string ExternalId,
    string? AnalysisId = null,
    Guid? IngestionJobId = null,
    bool UseCache = false,
    bool Reprocess = false,
    DateOnly? AcuerdoDate = null,
    string? CaseNumber = null,
    int? FetchPdfTimeoutSeconds = null);
