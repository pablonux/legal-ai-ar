using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Jobs.Commands.ReconcileJobProcessingDocuments;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobReconcileProcessingDocuments : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/reconcile-processing-documents", async (
            Guid id,
            ReconcileJobProcessingDocumentsRequestDto? body,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            body ??= new ReconcileJobProcessingDocumentsRequestDto();
            var result = await mediator.Send(
                new ReconcileJobProcessingDocumentsCommand(id, body.MinAgeMinutes, body.Stage),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobReconcileProcessingDocuments")
        .WithTags("AdminJobs");
}
