using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.BulkDocumentAction;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobDocumentsAction : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/documents/action", async (
            Guid id,
            BulkDocumentActionRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new BulkDocumentActionCommand(id, request.Stage, request.Action),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobDocumentsAction")
        .WithTags("AdminJobs");
}
