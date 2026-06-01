using LegalAiAr.Core.Models;

namespace LegalAiAr.Api.Hubs;

public interface IWorkerControlClient
{
    Task PauseAsync();
    Task ResumeAsync();
    Task InfraDegradedAsync(InfraIncidentReport report);
    Task InfraRecoveredAsync(Guid? ingestionJobId, string? detail);
}
