using System.Security.Claims;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Application.Admin.RulingReprocess.Commands.RetryRulingReprocess;
using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.RulingReprocess;

public sealed class PostRulingReprocessRetry : IEndpoint
{
    public string GroupName => AdminRulingReprocessEndpointGroup.Name;

    public string? AuthorizationPolicy => "AdminOnly";

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/{requestId:guid}/retry", async (
            Guid requestId,
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new RetryRulingReprocessCommand(requestId, AdminRequester.GetEmail(user)),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostRulingReprocessRetry")
        .WithTags("AdminRulingReprocess")
        .Produces<EnqueueRulingReprocessResult>(StatusCodes.Status200OK);
}
