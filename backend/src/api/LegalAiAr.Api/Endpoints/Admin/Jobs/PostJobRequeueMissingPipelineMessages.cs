using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Jobs.Commands.RequeueMissingPipelineMessages;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Enums;

namespace LegalAiAr.Api.Endpoints.Admin.Jobs;

public sealed class PostJobRequeueMissingPipelineMessages : IEndpoint
{
    public string GroupName => AdminJobsEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/jobs/{id:guid}/requeue-missing-pipeline-messages", async (
            Guid id,
            PipelineStage? stage,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RequeueMissingPipelineMessagesCommand(id, stage), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostJobRequeueMissingPipelineMessages")
        .WithTags("AdminJobs");
}
