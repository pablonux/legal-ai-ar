using LegalAiAr.Core.Enums;

namespace LegalAiAr.Core.Pipeline;

/// <summary>
/// Derives queue names from a single configurable prefix:
///   {prefix}-{stage}       e.g. "pipeline-fetcher"
///   {prefix}-{stage}-dlq   e.g. "pipeline-fetcher-dlq"
/// Messages carry EntityType + SourceId, so one set of queues serves all sources.
/// </summary>
public sealed class PipelineQueueNames
{
    public string Prefix { get; }

    public string Discoverer => $"{Prefix}-discoverer";
    public string Fetcher => $"{Prefix}-fetcher";
    public string Parser => $"{Prefix}-parser";
    public string Enricher => $"{Prefix}-enricher";
    public string Persister => $"{Prefix}-persister";
    public string Indexer => $"{Prefix}-indexer";

    public string DiscovererDlq => $"{Prefix}-discoverer-dlq";
    public string FetcherDlq => $"{Prefix}-fetcher-dlq";
    public string ParserDlq => $"{Prefix}-parser-dlq";
    public string EnricherDlq => $"{Prefix}-enricher-dlq";
    public string PersisterDlq => $"{Prefix}-persister-dlq";
    public string IndexerDlq => $"{Prefix}-indexer-dlq";

    public IReadOnlyList<string> AllMain => [Discoverer, Fetcher, Parser, Enricher, Persister, Indexer];
    public IReadOnlyList<string> AllDlq => [DiscovererDlq, FetcherDlq, ParserDlq, EnricherDlq, PersisterDlq, IndexerDlq];
    public IReadOnlyList<string> All => [.. AllMain, .. AllDlq];

    public string DlqFor(string stage) => $"{Prefix}-{stage}-dlq";
    public string QueueFor(string stage) => $"{Prefix}-{stage}";
    public string QueueFor(PipelineStage stage) => $"{Prefix}-{StageToString(stage)}";
    public string DlqFor(PipelineStage stage) => $"{Prefix}-{StageToString(stage)}-dlq";

    public PipelineQueueNames(string prefix)
    {
        if (string.IsNullOrWhiteSpace(prefix))
            throw new ArgumentException("Pipeline queue prefix cannot be empty.", nameof(prefix));
        Prefix = prefix.Trim().ToLowerInvariant();
    }

    private static string StageToString(PipelineStage stage) => stage switch
    {
        PipelineStage.Discoverer => "discoverer",
        PipelineStage.Fetcher => "fetcher",
        PipelineStage.Parser => "parser",
        PipelineStage.Enricher => "enricher",
        PipelineStage.Persister => "persister",
        PipelineStage.Indexer => "indexer",
        _ => throw new ArgumentOutOfRangeException(nameof(stage))
    };
}
