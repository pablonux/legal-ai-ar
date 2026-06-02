using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Pipeline.Commands.BulkRequeue;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostPipelineBulkRequeue : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/pipeline/bulk-requeue", async (
            BulkRequeueRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new BulkRequeueCommand(
                    request.Stage,
                    request.OnlyMissingOntology,
                    request.SourceId,
                    request.BatchSize),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostPipelineBulkRequeue")
        .WithTags("AdminJobs");
}
