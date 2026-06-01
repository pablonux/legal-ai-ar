using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for the discoverer queue. Triggers discovery of documents from a source.
/// Replaces the legacy <c>CrawlerMessage</c> with entity-first design.
/// </summary>
/// <param name="EntityType">Type of entity to discover (Ruling, Statute).</param>
/// <param name="SourceId">Source to discover from (CSJN=1, SAIJ=2, PJN=3, SCBA=4).</param>
/// <param name="Type">Ingestion type: "incremental" or "by-range".</param>
/// <param name="Since">For incremental: discover documents since this date.</param>
/// <param name="DateFrom">For by-range: start of date range.</param>
/// <param name="DateTo">For by-range: end of date range.</param>
/// <param name="IngestionJobId">Pre-created job ID.</param>
/// <param name="UseCache">When true, check download cache before HTTP calls.</param>
/// <param name="Reprocess">When true, skip deduplication checks.</param>
/// <param name="MaxDocuments">Optional cap on documents to discover.</param>
/// <param name="PreserveProgressCounters">When true with <see cref="IngestionJobId"/>, do not reset <c>DocumentsDiscovered</c>; keep <c>DocumentsSkipped</c> baseline for resume.</param>
public record DiscovererMessage(
    EntityType EntityType,
    int SourceId,
    string Type = "incremental",
    DateOnly? Since = null,
    DateOnly? DateFrom = null,
    DateOnly? DateTo = null,
    Guid? IngestionJobId = null,
    bool UseCache = false,
    bool Reprocess = false,
    int? MaxDocuments = null,
    int? SkipDocuments = null,
    bool PreserveProgressCounters = false);
