using System.Text.Json;
using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Infrastructure.Audit;

public class AuditService : IAuditService
{
    private readonly AppDbContext _context;
    private readonly ILogger<AuditService> _logger;

    public AuditService(AppDbContext context, ILogger<AuditService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task RecordFieldProvenanceAsync(
        string entityType, string entityId, string fieldName,
        string? value, string? previousValue,
        int sourceId, InferenceMethod inferenceMethod, Guid ingestionJobId,
        ChangeType changeType,
        string? sourceEndpoint, string? sourceField,
        string? aiModel, string? aiPrompt, float? aiConfidence,
        CancellationToken cancellationToken)
    {
        // Mark previous record as not current
        var existing = await _context.FieldProvenance
            .Where(fp => fp.EntityType == entityType && fp.EntityId == entityId
                         && fp.FieldName == fieldName && fp.IsCurrent)
            .FirstOrDefaultAsync(cancellationToken);

        if (existing != null)
            existing.IsCurrent = false;

        _context.FieldProvenance.Add(new FieldProvenance
        {
            EntityType = entityType,
            EntityId = entityId,
            FieldName = fieldName,
            Value = value,
            PreviousValue = previousValue,
            SourceId = sourceId,
            SourceEndpoint = sourceEndpoint,
            SourceField = sourceField,
            InferenceMethod = inferenceMethod,
            AiModel = aiModel,
            AiPrompt = aiPrompt,
            AiConfidence = aiConfidence,
            IngestionJobId = ingestionJobId,
            ChangeType = changeType,
            IsCurrent = true
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordFieldProvenanceBatchAsync(
        string entityType, string entityId,
        IEnumerable<FieldChange> changes,
        int sourceId, Guid ingestionJobId,
        CancellationToken cancellationToken)
    {
        var changeList = changes.ToList();
        if (changeList.Count == 0) return;

        var fieldNames = changeList.Select(c => c.FieldName).ToHashSet();

        var existingRecords = await _context.FieldProvenance
            .Where(fp => fp.EntityType == entityType && fp.EntityId == entityId
                         && fp.IsCurrent && fieldNames.Contains(fp.FieldName))
            .ToListAsync(cancellationToken);

        foreach (var existing in existingRecords)
            existing.IsCurrent = false;

        foreach (var change in changeList)
        {
            _context.FieldProvenance.Add(new FieldProvenance
            {
                EntityType = entityType,
                EntityId = entityId,
                FieldName = change.FieldName,
                Value = change.Value,
                PreviousValue = change.PreviousValue,
                SourceId = sourceId,
                SourceEndpoint = change.SourceEndpoint,
                SourceField = change.SourceField,
                InferenceMethod = change.InferenceMethod,
                AiModel = change.AiModel,
                AiPrompt = change.AiPrompt,
                AiConfidence = change.AiConfidence,
                IngestionJobId = ingestionJobId,
                ChangeType = change.ChangeType,
                IsCurrent = true
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogDebug("Recorded {Count} field provenance records for {EntityType} {EntityId}",
            changeList.Count, entityType, entityId);
    }

    public async Task RecordAuditLogAsync(
        string entityType, string entityId,
        AuditOperation operation, Guid? ingestionJobId,
        string performedBy, IReadOnlyList<string>? fieldsChanged,
        string? changeSummary, CancellationToken cancellationToken)
    {
        _context.EntityAuditLogs.Add(new EntityAuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Operation = operation,
            IngestionJobId = ingestionJobId,
            PerformedBy = performedBy,
            ChangeSummary = changeSummary,
            FieldsChanged = fieldsChanged is { Count: > 0 }
                ? JsonSerializer.Serialize(fieldsChanged)
                : null
        });

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordJobDetailAsync(
        Guid ingestionJobId, string entityType,
        int entitiesCreated, int entitiesUpdated, int entitiesDeleted,
        int fieldsUpdated, CancellationToken cancellationToken)
    {
        var existing = await _context.IngestionJobDetails
            .FirstOrDefaultAsync(d => d.IngestionJobId == ingestionJobId && d.EntityType == entityType,
                cancellationToken);

        if (existing != null)
        {
            existing.EntitiesCreated += entitiesCreated;
            existing.EntitiesUpdated += entitiesUpdated;
            existing.EntitiesDeleted += entitiesDeleted;
            existing.FieldsUpdated += fieldsUpdated;
        }
        else
        {
            _context.IngestionJobDetails.Add(new IngestionJobDetail
            {
                IngestionJobId = ingestionJobId,
                EntityType = entityType,
                EntitiesCreated = entitiesCreated,
                EntitiesUpdated = entitiesUpdated,
                EntitiesDeleted = entitiesDeleted,
                FieldsUpdated = fieldsUpdated
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordFullAuditAsync(
        string entityType, string entityId,
        IEnumerable<FieldChange> changes,
        int sourceId, Guid ingestionJobId,
        AuditOperation operation, string performedBy,
        IReadOnlyList<string>? fieldsChanged,
        string? changeSummary,
        int entitiesCreated, int fieldsUpdated,
        CancellationToken cancellationToken)
    {
        var changeList = changes.ToList();

        if (changeList.Count > 0)
        {
            var fieldNames = changeList.Select(c => c.FieldName).ToHashSet();

            var existingRecords = await _context.FieldProvenance
                .Where(fp => fp.EntityType == entityType && fp.EntityId == entityId
                             && fp.IsCurrent && fieldNames.Contains(fp.FieldName))
                .ToListAsync(cancellationToken);

            foreach (var existing in existingRecords)
                existing.IsCurrent = false;

            foreach (var change in changeList)
            {
                _context.FieldProvenance.Add(new FieldProvenance
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    FieldName = change.FieldName,
                    Value = change.Value,
                    PreviousValue = change.PreviousValue,
                    SourceId = sourceId,
                    SourceEndpoint = change.SourceEndpoint,
                    SourceField = change.SourceField,
                    InferenceMethod = change.InferenceMethod,
                    AiModel = change.AiModel,
                    AiPrompt = change.AiPrompt,
                    AiConfidence = change.AiConfidence,
                    IngestionJobId = ingestionJobId,
                    ChangeType = change.ChangeType,
                    IsCurrent = true
                });
            }
        }

        _context.EntityAuditLogs.Add(new EntityAuditLog
        {
            EntityType = entityType,
            EntityId = entityId,
            Operation = operation,
            IngestionJobId = ingestionJobId,
            PerformedBy = performedBy,
            ChangeSummary = changeSummary,
            FieldsChanged = fieldsChanged is { Count: > 0 }
                ? JsonSerializer.Serialize(fieldsChanged)
                : null
        });

        var existingJobDetail = await _context.IngestionJobDetails
            .FirstOrDefaultAsync(d => d.IngestionJobId == ingestionJobId && d.EntityType == entityType,
                cancellationToken);

        if (existingJobDetail != null)
        {
            existingJobDetail.EntitiesCreated += entitiesCreated;
            existingJobDetail.FieldsUpdated += fieldsUpdated;
        }
        else
        {
            _context.IngestionJobDetails.Add(new IngestionJobDetail
            {
                IngestionJobId = ingestionJobId,
                EntityType = entityType,
                EntitiesCreated = entitiesCreated,
                EntitiesUpdated = 0,
                EntitiesDeleted = 0,
                FieldsUpdated = fieldsUpdated
            });
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (changeList.Count > 0)
        {
            _logger.LogDebug("Recorded {Count} field provenance records for {EntityType} {EntityId}",
                changeList.Count, entityType, entityId);
        }
    }
}
