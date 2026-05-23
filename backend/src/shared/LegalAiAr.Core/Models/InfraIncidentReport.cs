namespace LegalAiAr.Core.Models;

/// <summary>
/// Worker-reported storage or network failure (serialized over SignalR to the API hub).
/// </summary>
public record InfraIncidentReport(
    string Category,
    string ErrorCode,
    string QueueName,
    string WorkerType,
    string? Detail,
    Guid? IngestionJobId = null);
