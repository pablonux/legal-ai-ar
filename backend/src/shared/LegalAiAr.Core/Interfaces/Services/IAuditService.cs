using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Interfaces.Services;

/// <summary>
/// Records field-level provenance and entity audit logs during ingestion.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Records the provenance of a single field value.
    /// </summary>
    Task RecordFieldProvenanceAsync(
        string entityType,
        string entityId,
        string fieldName,
        string? value,
        string? previousValue,
        int sourceId,
        InferenceMethod inferenceMethod,
        Guid ingestionJobId,
        ChangeType changeType,
        string? sourceEndpoint = null,
        string? sourceField = null,
        string? aiModel = null,
        string? aiPrompt = null,
        float? aiConfidence = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records provenance for multiple fields of the same entity in batch.
    /// </summary>
    Task RecordFieldProvenanceBatchAsync(
        string entityType,
        string entityId,
        IEnumerable<FieldChange> changes,
        int sourceId,
        Guid ingestionJobId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records an entity-level audit log entry.
    /// </summary>
    Task RecordAuditLogAsync(
        string entityType,
        string entityId,
        AuditOperation operation,
        Guid? ingestionJobId,
        string performedBy,
        IReadOnlyList<string>? fieldsChanged = null,
        string? changeSummary = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records or updates ingestion job detail metrics for an entity type.
    /// </summary>
    Task RecordJobDetailAsync(
        Guid ingestionJobId,
        string entityType,
        int entitiesCreated = 0,
        int entitiesUpdated = 0,
        int entitiesDeleted = 0,
        int fieldsUpdated = 0,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Records field provenance batch, entity audit log, and ingestion job detail in one unit of work.
    /// </summary>
    Task RecordFullAuditAsync(
        string entityType,
        string entityId,
        IEnumerable<FieldChange> changes,
        int sourceId,
        Guid ingestionJobId,
        AuditOperation operation,
        string performedBy,
        IReadOnlyList<string>? fieldsChanged,
        string? changeSummary,
        int entitiesCreated,
        int fieldsUpdated,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a single field change for batch provenance recording.
/// </summary>
public record FieldChange(
    string FieldName,
    string? Value,
    string? PreviousValue,
    InferenceMethod InferenceMethod,
    ChangeType ChangeType = ChangeType.Create,
    string? SourceEndpoint = null,
    string? SourceField = null,
    string? AiModel = null,
    string? AiPrompt = null,
    float? AiConfidence = null);
