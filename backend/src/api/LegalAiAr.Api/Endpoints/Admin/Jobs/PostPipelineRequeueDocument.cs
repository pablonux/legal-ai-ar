using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Pipeline.Commands.RequeueDocument;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostPipelineRequeueDocument : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/pipeline/requeue-document", async (
            RequeueDocumentRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new RequeueDocumentCommand(request.Stage, request.Message, request.RulingId),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostPipelineRequeueDocument")
        .WithTags("AdminJobs");
}
