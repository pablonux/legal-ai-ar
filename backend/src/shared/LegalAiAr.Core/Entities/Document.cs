using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Entities;

/// <summary>
/// Central pipeline entity tracking a document's journey through all 6 stages.
/// Created by the Discoverer, advanced by each subsequent stage.
/// <see cref="Id"/> serves as the CorrelationId across all queue messages and logs.
/// </summary>
public class Document
{
    public Guid Id { get; set; }
    public Guid IngestionJobId { get; set; }
    public EntityType EntityType { get; set; }
    public int SourceId { get; set; }

    /// <summary>External document ID from the source (e.g. CSJN Codigo, SAIJ doc ID).</summary>
    public string ExternalId { get; set; } = string.Empty;
    /// <summary>Source-specific analysis ID. For CSJN: idAnalisis used for API calls.</summary>
    public string? AnalysisId { get; set; }

    public PipelineStage CurrentStage { get; set; } = PipelineStage.Discoverer;
    public DocumentStatus Status { get; set; } = DocumentStatus.Pending;

    /// <summary>Blob path where the content (PDF/HTML) was uploaded by the Fetcher.</summary>
    public string? BlobPath { get; set; }
    /// <summary>SHA-256 hash of the downloaded content, set by the Fetcher.</summary>
    public string? ContentHash { get; set; }

    /// <summary>Error message from the last failure at <see cref="CurrentStage"/>.</summary>
    public string? ErrorMessage { get; set; }
    /// <summary>Exception type name from the last failure.</summary>
    public string? ErrorType { get; set; }
    /// <summary>Number of processing attempts at the current stage.</summary>
    public int RetryCount { get; set; }

    /// <summary>
    /// When set, the Fetcher uses this HTTP timeout (seconds) only for this document's PDF download,
    /// via a dedicated HTTP client (the worker default timeout is unchanged for other documents).
    /// </summary>
    public int? FetchPdfTimeoutSeconds { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }

    /// <summary>FK to persisted Ruling entity. Set by the Persister stage.</summary>
    public Guid? RulingId { get; set; }
    /// <summary>FK to persisted Statute entity. Set by the Persister stage.</summary>
    public int? StatuteId { get; set; }

    public IngestionJob IngestionJob { get; set; } = null!;
}
