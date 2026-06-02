using System.Security.Claims;
using LegalAiAr.Api.Interfaces;
using LegalAiAr.Contracts.Responses.Auth;

namespace LegalAiAr.Api.Endpoints.Auth;

public sealed class GetAuthMe : IEndpoint
{
    public string GroupName => AuthEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/me", (ClaimsPrincipal user) =>
        {
            var email = user.FindFirstValue(ClaimTypes.Email)
                ?? user.FindFirstValue("email")
                ?? "";
            var name = user.FindFirstValue(ClaimTypes.Name)
                ?? user.FindFirstValue("name")
                ?? email;
            var role = user.FindFirstValue(ClaimTypes.Role)
                ?? user.FindFirstValue("role")
                ?? "viewer";
            var groups = user.FindFirstValue("groups") ?? "";

            return Results.Ok(new MeResponse(email, name, role, groups));
        })
        .WithName("GetAuthMe")
        .WithTags("Auth")
        .Produces<MeResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized);
}

internal static class AuthEndpointGroup
{
    public const string Name = "auth";
}
