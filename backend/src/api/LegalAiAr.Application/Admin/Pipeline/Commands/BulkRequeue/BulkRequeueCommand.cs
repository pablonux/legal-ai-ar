using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Application.Admin.Pipeline.Commands.BulkRequeue;

/// <summary>
/// Re-queues all eligible rulings to a pipeline stage in batches.
/// </summary>
/// <param name="Stage">enrichment or indexer.</param>
/// <param name="OnlyMissingOntology">When true, only requeue rulings where LegalBranch is null (not yet ontology-classified).</param>
/// <param name="SourceId">Optional source filter.</param>
/// <param name="BatchSize">Number of rulings per batch. Defaults to 50.</param>
public record BulkRequeueCommand(
    string Stage,
    bool OnlyMissingOntology = true,
    int? SourceId = null,
    int BatchSize = 50) : IRequest<BulkRequeueResult>;
