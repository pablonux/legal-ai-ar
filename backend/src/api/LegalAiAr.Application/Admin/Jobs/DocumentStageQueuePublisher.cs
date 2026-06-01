using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;

namespace LegalAiAr.Application.Admin.Jobs;

/// <summary>
/// Publishes a queue message to re-run one document at a given pipeline stage.
/// Shared by bulk and single-document admin actions.
/// </summary>
internal static class DocumentStageQueuePublisher
{
    public static async Task<bool> TryPublishForStageAsync(
        Document doc,
        PipelineStage stage,
        IQueuePublisher publisher,
        PipelineQueueNames queueNames,
        CancellationToken cancellationToken)
    {
        switch (stage)
        {
            case PipelineStage.Fetcher:
                await publisher.PublishAsync(queueNames.Fetcher, new FetcherMessage(
                    doc.Id, doc.EntityType, doc.SourceId, doc.ExternalId, doc.AnalysisId,
                    doc.IngestionJobId,
                    FetchPdfTimeoutSeconds: doc.FetchPdfTimeoutSeconds), cancellationToken);
                return true;
            case PipelineStage.Parser:
                await publisher.PublishAsync(queueNames.Parser, new ParserMessage(
                    doc.SourceId, doc.ExternalId, doc.AnalysisId, doc.BlobPath ?? "",
                    doc.ContentHash ?? "", ApiMetadata: null,
                    PipelineDocumentId: doc.Id,
                    IngestionJobId: doc.IngestionJobId), cancellationToken);
                return true;
            case PipelineStage.Enricher:
                await publisher.PublishAsync(queueNames.Enricher, new EnrichmentMessage(
                    doc.ExternalId, doc.SourceId, "",
                    new ExtractedMetadata("", default, null, null, null, null, null, null, null, [], []),
                    [],
                    doc.BlobPath, doc.ContentHash,
                    PipelineDocumentId: doc.Id,
                    IngestionJobId: doc.IngestionJobId), cancellationToken);
                return true;
            case PipelineStage.Persister:
                var persisterPayloadPath = ToPayloadPath(doc.BlobPath);
                await publisher.PublishAsync(queueNames.Persister, new PersisterMessage(
                    doc.Id, doc.EntityType, doc.SourceId, doc.ContentHash ?? "",
                    doc.IngestionJobId, persisterPayloadPath), cancellationToken);
                return true;
            case PipelineStage.Indexer:
                var indexerPayloadPath = ToPayloadPath(doc.BlobPath);
                await publisher.PublishAsync(queueNames.Indexer, new IndexerMessage(
                    doc.ExternalId, doc.ContentHash ?? "", doc.SourceId,
                    new RulingData("", default, null, null, null, null, null, null, null, false, null, null, "", ""),
                    [], [], [], [],
                    Array.Empty<ChunkData>(),
                    PayloadBlobPath: indexerPayloadPath,
                    PipelineDocumentId: doc.Id,
                    IngestionJobId: doc.IngestionJobId,
                    EntityId: doc.RulingId),
                    cancellationToken);
                return true;
            default:
                return false;
        }
    }

    private static string ToPayloadPath(string? blobPath)
    {
        if (string.IsNullOrEmpty(blobPath)) return "";
        return blobPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
            ? blobPath[..^4] + ".indexer.json"
            : blobPath + ".indexer.json";
    }
}
