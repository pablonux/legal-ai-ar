using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Queries.GetPipelineStatus;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class GetPipelineStatus : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/pipeline/status", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetPipelineStatusQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetPipelineStatus")
        .WithTags("AdminJobs");
}
