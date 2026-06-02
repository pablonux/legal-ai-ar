using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.ResumeDiscovery;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobResumeDiscovery : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/resume-discovery", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new ResumeDiscoveryCommand(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobResumeDiscovery")
        .WithTags("AdminJobs");
}
