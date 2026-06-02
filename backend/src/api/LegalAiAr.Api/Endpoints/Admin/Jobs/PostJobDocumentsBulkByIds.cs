using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkFailedDocumentsByIds;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobDocumentsBulkByIds : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{jobId:guid}/documents/bulk-by-ids", async (
            Guid jobId,
            BulkFailedDocumentsByIdsRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new BulkFailedDocumentsByIdsCommand(jobId, request.DocumentIds, request.Action),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobDocumentsBulkByIds")
        .WithTags("AdminJobs");
}
