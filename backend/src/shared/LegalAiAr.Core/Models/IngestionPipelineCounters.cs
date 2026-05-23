namespace LegalAiAr.Core.Models;

/// <summary>
/// Denormalized pipeline counters on <see cref="Entities.IngestionJob"/> that workers increment.
/// Used when reconciling job rows from <c>Documents</c> truth.
/// </summary>
public readonly record struct IngestionPipelineCounters(
    int DocumentsCrawled,
    int DocumentsParsed,
    int DocumentsEnriched,
    int DocumentsPersisted,
    int DocumentsIndexed,
    int DocumentsFailed);
