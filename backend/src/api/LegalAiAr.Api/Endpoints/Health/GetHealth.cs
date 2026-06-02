using LegalAiAr.Api.Interfaces;
using LegalAiAr.Core.Interfaces.Services;

namespace LegalAiAr.Api.Endpoints.Health;

public sealed class GetHealth : IEndpoint
{
    public string GroupName => HealthEndpointGroup.Name;

    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet(string.Empty, async (IHealthCheckService healthCheckService, CancellationToken cancellationToken) =>
        {
            var result = await healthCheckService.CheckAsync(cancellationToken);
            return result.Status == "Unhealthy"
                ? Results.Json(result, statusCode: StatusCodes.Status503ServiceUnavailable)
                : Results.Ok(result);
        })
        .WithName("GetHealth")
        .WithTags("Health")
        .Produces<HealthCheckResult>(StatusCodes.Status200OK)
        .Produces<HealthCheckResult>(StatusCodes.Status503ServiceUnavailable);
}
