using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.SetDocumentFetchPdfTimeout;
using LegalAiAr.Application.Admin.Jobs.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PatchJobDocumentFetchPdfTimeout : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPatch("/jobs/{jobId:guid}/documents/{documentId:guid}/fetch-pdf-timeout", async (
            Guid jobId,
            Guid documentId,
            SetDocumentFetchPdfTimeoutRequestDto request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new SetDocumentFetchPdfTimeoutCommand(jobId, documentId, request.TimeoutSeconds),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PatchJobDocumentFetchPdfTimeout")
        .WithTags("AdminJobs");
}
