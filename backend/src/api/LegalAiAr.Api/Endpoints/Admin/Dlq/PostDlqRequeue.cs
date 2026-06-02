using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Dlq.Commands.RequeueMessage;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Api.Endpoints.Admin.Dlq;

public sealed class PostDlqRequeue : IEndpoint
{
    public string GroupName => AdminDlqEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/{queue}/{id}/requeue", async (
            string queue,
            string id,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(new RequeueMessageCommand(queue, id), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostDlqRequeue")
        .WithTags("AdminDlq")
        .Produces<RequeueResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
}
