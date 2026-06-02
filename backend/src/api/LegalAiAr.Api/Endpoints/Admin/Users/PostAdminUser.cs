using LegalAiAr.Api.Interfaces;
using LegalAiAr.Api.Models;
using LegalAiAr.Application.Admin.Users.Commands.CreateUser;
using LegalAiAr.Application.Admin.Users.DTOs;
using LegalAiAr.Application.Mediation;

namespace LegalAiAr.Api.Endpoints.Admin.Users;

public sealed class PostAdminUser : IEndpoint
{
    public string GroupName => AdminUsersEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost(string.Empty, async (
            CreateUserRequest request,
            IMediator mediator,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
                return Results.BadRequest(new { error = "Email and Role are required." });

            var result = await mediator.Send(
                new CreateUserCommand(request.Email, request.DisplayName, request.Role),
                cancellationToken);
            return Results.Created($"/api/admin/users/{result.Id}", result);
        })
        .WithName("PostAdminUser")
        .WithTags("AdminUsers")
        .Produces<UserDto>(StatusCodes.Status201Created)
        .Produces(StatusCodes.Status400BadRequest);
}
