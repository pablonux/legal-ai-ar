using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.Dlq.Queries.GetDlqMessages;
using LegalAiAr.Application.Mediation;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Api.Endpoints.Admin.Dlq;

public sealed class GetDlqMessagesEndpoint : IEndpoint
{
    public string GroupName => AdminDlqEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (
            string queue,
            int maxMessages,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(queue))
                return Results.BadRequest(new { error = "Queue is required." });

            var take = maxMessages == 0 ? 32 : maxMessages;
            var result = await mediator.Send(new GetDlqMessagesQuery(queue.Trim(), take), cancellationToken);
            return Results.Ok(result);
        })
        .WithName("GetDlqMessages")
        .WithTags("AdminDlq")
        .Produces<DlqPeekResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);
}

internal static class AdminDlqEndpointGroup
{
    public const string Name = "admin/dlq";
}
