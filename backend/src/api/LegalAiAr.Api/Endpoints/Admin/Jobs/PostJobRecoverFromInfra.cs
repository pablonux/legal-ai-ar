using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Api.Services;
using LegalAiAr.Application.Admin.Jobs.Commands.RecoverJobFromInfra;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobRecoverFromInfra : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/recover-from-infra", async (
            Guid id,
            RecoverJobFromInfraRequestDto? body,
            JobInfraRecoveryOrchestrator orchestrator,
            CancellationToken cancellationToken) =>
        {
            body ??= new RecoverJobFromInfraRequestDto();
            var command = new RecoverJobFromInfraCommand(
                id,
                body.RequireStorageProbe,
                body.ClearInfrastructureDegraded,
                body.BroadcastRecovered,
                body.ResumeDiscovery,
                body.RequeueFetcherPending,
                body.RequeueAllPipelineStages);
            var result = await orchestrator.RecoverJobAsync(command, body.ResumeAllWorkers, cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobRecoverFromInfra")
        .WithTags("AdminJobs");
}
