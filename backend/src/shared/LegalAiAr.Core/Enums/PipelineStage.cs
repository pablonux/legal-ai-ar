namespace LegalAiAr.Core.Enums;

/// <summary>
/// Stages of the ingestion pipeline, in processing order.
/// </summary>
public enum PipelineStage
{
    Discoverer = 0,
    Fetcher = 1,
    Parser = 2,
    Enricher = 3,
    Persister = 4,
    Indexer = 5,
}
