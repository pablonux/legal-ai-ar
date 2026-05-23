namespace LegalAiAr.Api.Models;

/// <summary>
/// Body for <c>POST /api/admin/jobs/{{id}}/recover-from-infra</c>.
/// </summary>
public sealed class RecoverJobFromInfraRequestDto
{
    public bool RequireStorageProbe { get; set; } = true;
    public bool ClearInfrastructureDegraded { get; set; } = true;
    public bool BroadcastRecovered { get; set; } = true;
    public bool ResumeDiscovery { get; set; }
    public bool RequeueFetcherPending { get; set; } = true;
    public bool RequeueAllPipelineStages { get; set; }
    public bool ResumeAllWorkers { get; set; } = true;
}
