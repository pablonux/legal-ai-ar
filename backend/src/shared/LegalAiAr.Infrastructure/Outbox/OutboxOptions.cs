namespace LegalAiAr.Infrastructure.Outbox;

/// <summary>
/// Configuration for the transactional outbox dispatcher hosted in the API.
/// </summary>
public sealed class OutboxOptions
{
    public const string SectionName = "Outbox";

    public bool Enabled { get; set; } = true;

    public int PollIntervalSeconds { get; set; } = 5;

    public int BatchSize { get; set; } = 20;

    public int MaxRetries { get; set; } = 5;
}
