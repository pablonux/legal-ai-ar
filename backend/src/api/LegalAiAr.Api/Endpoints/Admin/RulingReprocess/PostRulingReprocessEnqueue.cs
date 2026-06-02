using System.Security.Claims;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models.Admin;
using LegalAiAr.Application.Admin.RulingReprocess.Commands.EnqueueRulingReprocess;
using LegalAiAr.Application.Admin.RulingReprocess.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.RulingReprocess;

public sealed class PostRulingReprocessEnqueue : IEndpoint
{
    public string GroupName => AdminRulingReprocessEndpointGroup.Name;

    public string? AuthorizationPolicy => "AdminOnly";

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/rulings/{rulingId:guid}", async (
            Guid rulingId,
            EnqueueRulingReprocessRequest? body,
            ClaimsPrincipal user,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(
                new EnqueueRulingReprocessCommand(
                    rulingId,
                    AdminRequester.GetEmail(user),
                    body?.UseCache ?? false),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PostRulingReprocessEnqueue")
        .WithTags("AdminRulingReprocess")
        .Produces<EnqueueRulingReprocessResult>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
}
