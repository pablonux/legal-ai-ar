namespace LegalAiAr.Api.Services;

/// <summary>
/// Worker pipeline clients (SignalR to worker-control hub) authenticate with this shared secret header.
/// Must match across API and all worker appsettings.
/// </summary>
public class WorkerControlOptions
{
    public const string SectionName = "WorkerControl";

    /// <summary>
    /// Sent as <c>X-Worker-Hub-Key</c> on SignalR negotiate and WebSocket connections.
    /// </summary>
    public string HubAccessKey { get; set; } = string.Empty;
}
