using LegalAiAr.Api.Interfaces;

namespace LegalAiAr.Api.Endpoints.Health;

public sealed class GetHealthLive : IEndpoint
{
    public string GroupName => HealthEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("/live", () => Results.Ok(new { status = "live" }))
        .AllowAnonymous()
        .WithName("GetHealthLive")
        .WithTags("Health")
        .Produces(StatusCodes.Status200OK);
}

internal static class HealthEndpointGroup
{
    public const string Name = "health";
}
