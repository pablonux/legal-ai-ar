using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Messages;

/// <summary>
/// Message for the persister queue. Contains the complete entity payload for SQL persistence.
/// Replaces the role of IndexerMessage as the carrier of the full entity graph.
/// The actual payload is offloaded to Blob when it exceeds queue size limits.
/// </summary>
/// <param name="DocumentId">Document.Id (pipeline correlation ID).</param>
/// <param name="EntityType">Type of entity to persist.</param>
/// <param name="SourceId">Source of the document.</param>
/// <param name="ContentHash">SHA-256 hash for idempotency.</param>
/// <param name="IngestionJobId">Job that originated this document.</param>
/// <param name="PayloadBlobPath">Blob path where the full entity payload JSON is stored.</param>
/// <param name="Reprocess">When true, delete existing entity before re-persisting.</param>
public record PersisterMessage(
    Guid DocumentId,
    EntityType EntityType,
    int SourceId,
    string ContentHash,
    Guid? IngestionJobId = null,
    string? PayloadBlobPath = null,
    bool Reprocess = false);
