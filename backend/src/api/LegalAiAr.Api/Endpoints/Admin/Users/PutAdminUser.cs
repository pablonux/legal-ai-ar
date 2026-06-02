using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Users.Commands.UpdateUser;
using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Users;

public sealed class PutAdminUser : IEndpoint
{
    public string GroupName => AdminUsersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPut("/{id:guid}", async (
            Guid id,
            UpdateUserRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Role))
                return Results.BadRequest(new { error = "Role is required." });

            var result = await mediator.Send(
                new UpdateUserCommand(id, request.DisplayName, request.Role),
                cancellationToken);
            return Results.Ok(result);
        })
        .WithName("PutAdminUser")
        .WithTags("AdminUsers")
        .Produces<UserDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status404NotFound);
}
