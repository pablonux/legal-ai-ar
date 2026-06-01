namespace LegalAiAr.Core.Pipeline;

/// <summary>
/// Shared configuration for all pipeline worker services.
/// Bound from the "Worker" config section.
/// </summary>
public class WorkerOptions
{
    public const string SectionName = "Worker";

    public int BatchSize { get; set; } = 1;
    public int MaxConcurrency { get; set; } = 1;
    public double PollIntervalSeconds { get; set; } = 2;
    public double EmptyPollIntervalSeconds { get; set; } = 10;
    public int VisibilityTimeoutMinutes { get; set; } = 10;
    public int MaxDequeueCount { get; set; } = 3;
}
