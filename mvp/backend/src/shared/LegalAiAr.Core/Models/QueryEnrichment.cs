namespace LegalAiAr.Core.Models;

/// <summary>
/// Structured enrichment extracted from a user query before the agentic executor runs.
/// </summary>
public sealed class QueryEnrichment
{
    public required string Intent { get; init; }
    public EnrichmentSource Source { get; init; }
    public float? Confidence { get; init; }
    public IReadOnlyList<TemporalEntity> Temporal { get; init; } = [];
    public IReadOnlyList<string> Courts { get; init; } = [];
    public IReadOnlyList<StatuteEntity> Statutes { get; init; } = [];
    public IReadOnlyList<string> Cases { get; init; } = [];
    public IReadOnlyList<string> Topics { get; init; } = [];

    /// <summary>
    /// Formats the enrichment as a system message for the agentic executor.
    /// </summary>
    public string ToSystemMessage()
    {
        var parts = new List<string> { $"[Query Analysis]\nIntent: {Intent}" };

        var entities = new List<string>();
        foreach (var court in Courts) entities.Add($"- Court: {court}");
        foreach (var statute in Statutes)
        {
            var articles = statute.Articles.Count > 0 ? $" ({string.Join(", ", statute.Articles)})" : "";
            entities.Add($"- Statute: {statute.Reference}{articles}");
        }
        foreach (var temporal in Temporal)
        {
            var range = temporal.To.HasValue
                ? $"{temporal.From}-{temporal.To}"
                : $"desde {temporal.From}";
            entities.Add($"- Period: {range}");
        }
        foreach (var c in Cases) entities.Add($"- Case: {c}");
        foreach (var topic in Topics) entities.Add($"- Topic: {topic}");

        if (entities.Count > 0)
        {
            parts.Add("Entities detected:");
            parts.AddRange(entities);
        }

        return string.Join("\n", parts);
    }
}

public sealed record TemporalEntity(int From, int? To);
public sealed record StatuteEntity(string Reference, IReadOnlyList<string> Articles);

public enum EnrichmentSource
{
    RuleBased,
    LlmFallback
}
