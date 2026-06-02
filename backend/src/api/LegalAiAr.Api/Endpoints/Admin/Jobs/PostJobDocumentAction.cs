using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.SingleFailedDocumentAction;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobDocumentAction : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{jobId:guid}/documents/{documentId:guid}/action", async (
            Guid jobId,
            Guid documentId,
            SingleDocumentActionRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new SingleFailedDocumentActionCommand(jobId, documentId, request.Action),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobDocumentAction")
        .WithTags("AdminJobs");
}
