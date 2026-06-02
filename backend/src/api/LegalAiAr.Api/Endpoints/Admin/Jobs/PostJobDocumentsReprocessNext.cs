using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.ReprocessNextFailedDocuments;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobDocumentsReprocessNext : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/documents/reprocess-next", async (
            Guid id,
            ReprocessNextFailedRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var take = request.Take ?? 10;
            var result = await mediator.Send(
                new ReprocessNextFailedDocumentsCommand(id, request.Stage, take),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobDocumentsReprocessNext")
        .WithTags("AdminJobs");
}
