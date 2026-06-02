using LegalAiAr.Api.Interfaces;
using LegalAiAr.Contracts.Responses.Auth;

namespace LegalAiAr.Api.Endpoints.Auth;

public sealed class PostAuthLogout : IEndpoint
{
    public string GroupName => AuthEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("/logout", () => Results.Ok(new LogoutResponse(Success: true)))
        .WithName("PostAuthLogout")
        .WithTags("Auth")
        .Produces<LogoutResponse>(StatusCodes.Status200OK);
}
