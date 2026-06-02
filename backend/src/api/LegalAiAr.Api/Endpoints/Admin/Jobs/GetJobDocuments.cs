using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Queries.GetJobDocuments;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class GetJobDocuments : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/jobs/{id:guid}/documents", async (
            Guid id,
            PipelineStage? stage,
            DocumentStatus? status,
            int skip,
            int take,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            take = take == 0 ? 50 : take;
            var result = await mediator.Send(new GetJobDocumentsQuery(id, stage, status, skip, take), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetJobDocuments")
        .WithTags("AdminJobs");
}
