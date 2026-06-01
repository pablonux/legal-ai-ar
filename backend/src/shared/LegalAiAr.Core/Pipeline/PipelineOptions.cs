namespace LegalAiAr.Core.Pipeline;

/// <summary>
/// Pipeline configuration. Bound from the "Pipeline" config section.
/// </summary>
public class PipelineOptions
{
    public const string SectionName = "Pipeline";

    /// <summary>
    /// Single prefix shared by all pipeline queues: {prefix}-fetcher, {prefix}-parser, etc.
    /// </summary>
    public string QueuePrefix { get; set; } = "pipeline";
}
