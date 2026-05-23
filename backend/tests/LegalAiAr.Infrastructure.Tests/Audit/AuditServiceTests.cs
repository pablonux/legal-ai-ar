using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Audit;
using LegalAiAr.Infrastructure.Tests.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace LegalAiAr.Infrastructure.Tests.Audit;

public class AuditServiceTests
{
    private static AuditService CreateSut(string dbName, out Infrastructure.Persistence.AppDbContext context)
    {
        context = TestDbContextFactory.Create(dbName);
        return new AuditService(context, Substitute.For<ILogger<AuditService>>());
    }

    [Fact]
    public async Task RecordFieldProvenanceAsync_CreatesRecord()
    {
        var dbName = Guid.NewGuid().ToString();
        var sut = CreateSut(dbName, out var context);
        var jobId = Guid.NewGuid();

        await sut.RecordFieldProvenanceAsync(
            entityType: "Ruling", entityId: "abc-123", fieldName: "CaseTitle",
            value: "Gomez c/ Estado Nacional", previousValue: null,
            sourceId: 1, inferenceMethod: InferenceMethod.SourceApi, ingestionJobId: jobId,
            changeType: ChangeType.Create,
            sourceEndpoint: "abrirAnalisis", sourceField: "caratula",
            aiModel: null, aiPrompt: null, aiConfidence: null,
            cancellationToken: CancellationToken.None);

        var record = await context.FieldProvenance.SingleAsync();
        Assert.Equal("Ruling", record.EntityType);
        Assert.Equal("abc-123", record.EntityId);
        Assert.Equal("CaseTitle", record.FieldName);
        Assert.Equal("Gomez c/ Estado Nacional", record.Value);
        Assert.True(record.IsCurrent);
        Assert.Equal(InferenceMethod.SourceApi, record.InferenceMethod);
        Assert.Equal(jobId, record.IngestionJobId);
    }

    [Fact]
    public async Task RecordFieldProvenanceAsync_MarksPreviousAsNotCurrent()
    {
        var dbName = Guid.NewGuid().ToString();
        var sut = CreateSut(dbName, out var context);
        var jobId = Guid.NewGuid();

        await sut.RecordFieldProvenanceAsync(
            entityType: "Ruling", entityId: "abc-123", fieldName: "Summary",
            value: "V1", previousValue: null, sourceId: 1,
            inferenceMethod: InferenceMethod.SourceApi, ingestionJobId: jobId,
            changeType: ChangeType.Create,
            sourceEndpoint: null, sourceField: null,
            aiModel: null, aiPrompt: null, aiConfidence: null,
            cancellationToken: CancellationToken.None);

        await sut.RecordFieldProvenanceAsync(
            entityType: "Ruling", entityId: "abc-123", fieldName: "Summary",
            value: "V2", previousValue: "V1", sourceId: 1,
            inferenceMethod: InferenceMethod.AiPrimary, ingestionJobId: jobId,
            changeType: ChangeType.Update,
            sourceEndpoint: null, sourceField: null,
            aiModel: null, aiPrompt: null, aiConfidence: null,
            cancellationToken: CancellationToken.None);

        var records = await context.FieldProvenance
            .Where(fp => fp.EntityType == "Ruling" && fp.EntityId == "abc-123" && fp.FieldName == "Summary")
            .OrderBy(fp => fp.Id)
            .ToListAsync();

        Assert.Equal(2, records.Count);
        Assert.False(records[0].IsCurrent);
        Assert.True(records[1].IsCurrent);
        Assert.Equal("V2", records[1].Value);
        Assert.Equal("V1", records[1].PreviousValue);
    }

    [Fact]
    public async Task RecordFieldProvenanceBatchAsync_CreatesMultipleRecords()
    {
        var dbName = Guid.NewGuid().ToString();
        var sut = CreateSut(dbName, out var context);
        var jobId = Guid.NewGuid();

        var changes = new List<FieldChange>
        {
            new("CaseTitle", "Title", null, InferenceMethod.SourceApi),
            new("CaseNumber", "12345", null, InferenceMethod.SourceApi),
            new("Summary", "AI Summary", null, InferenceMethod.AiPrimary, AiModel: "gpt-5-nano", AiConfidence: 0.95f)
        };

        await sut.RecordFieldProvenanceBatchAsync(
            "Ruling", "r-001", changes, sourceId: 1, ingestionJobId: jobId,
            cancellationToken: CancellationToken.None);

        var records = await context.FieldProvenance.Where(fp => fp.EntityId == "r-001").ToListAsync();
        Assert.Equal(3, records.Count);
        Assert.All(records, r => Assert.True(r.IsCurrent));

        var aiRecord = records.Single(r => r.FieldName == "Summary");
        Assert.Equal("gpt-5-nano", aiRecord.AiModel);
        Assert.Equal(0.95f, aiRecord.AiConfidence);
    }

    [Fact]
    public async Task RecordAuditLogAsync_CreatesLogEntry()
    {
        var dbName = Guid.NewGuid().ToString();
        var sut = CreateSut(dbName, out var context);
        var jobId = Guid.NewGuid();

        await sut.RecordAuditLogAsync(
            entityType: "Ruling", entityId: "r-002",
            operation: AuditOperation.Created, ingestionJobId: jobId,
            performedBy: "IndexerWorker",
            fieldsChanged: new[] { "CaseTitle", "Summary" },
            changeSummary: "Indexed ruling with 3 persons",
            cancellationToken: CancellationToken.None);

        var log = await context.EntityAuditLogs.SingleAsync();
        Assert.Equal("Ruling", log.EntityType);
        Assert.Equal("r-002", log.EntityId);
        Assert.Equal(AuditOperation.Created, log.Operation);
        Assert.Equal("IndexerWorker", log.PerformedBy);
        Assert.Contains("CaseTitle", log.FieldsChanged!);
        Assert.Contains("Summary", log.FieldsChanged!);
    }

    [Fact]
    public async Task RecordJobDetailAsync_CreatesAndAccumulatesMetrics()
    {
        var dbName = Guid.NewGuid().ToString();
        var sut = CreateSut(dbName, out var context);
        var jobId = Guid.NewGuid();

        await sut.RecordJobDetailAsync(jobId, "Ruling", 1, 0, 0, 8, CancellationToken.None);
        await sut.RecordJobDetailAsync(jobId, "Ruling", 1, 0, 0, 5, CancellationToken.None);

        var detail = await context.IngestionJobDetails
            .SingleAsync(d => d.IngestionJobId == jobId && d.EntityType == "Ruling");
        Assert.Equal(2, detail.EntitiesCreated);
        Assert.Equal(13, detail.FieldsUpdated);
    }
}
