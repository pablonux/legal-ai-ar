using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class GetJobs : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/jobs", async (IMediator mediator, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetJobsQuery(), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetJobs")
        .WithTags("AdminJobs");
}
