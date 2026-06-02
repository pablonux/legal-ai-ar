using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocumentsSummary;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class GetJobDocumentsSummary : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/jobs/{id:guid}/documents/summary", async (
            Guid id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new GetJobDocumentsSummaryQuery(id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetJobDocumentsSummary")
        .WithTags("AdminJobs");
}
