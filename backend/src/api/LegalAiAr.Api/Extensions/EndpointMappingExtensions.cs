using LegalAiAr.Api.Filters;
using LegalAiAr.Api.Interfaces;

namespace LegalAiAr.Api.Extensions;

public static class EndpointMappingExtensions
{
    public static WebApplication MapLegalAiArEndpoints(this WebApplication app)
    {
        var endpointTypes = app.Services.GetRequiredService<IReadOnlyList<Type>>();
        var endpoints = endpointTypes
            .Select(t => (IEndpoint)app.Services.GetRequiredService(t))
            .ToList();

        foreach (var group in endpoints.GroupBy(e => e.GroupName, StringComparer.OrdinalIgnoreCase))
        {
            var routeGroup = app.MapGroup($"api/{group.Key.ToLowerInvariant()}");
            routeGroup.AddEndpointFilter<ValidationEndpointFilter>();

            if (!string.Equals(group.Key, "health", StringComparison.OrdinalIgnoreCase))
            {
                var policy = group.Select(e => e.AuthorizationPolicy).FirstOrDefault(p => p is not null);
                if (policy is not null)
                    routeGroup.RequireAuthorization(policy);
                else
                    routeGroup.RequireAuthorization();
            }

            foreach (var endpoint in group)
                endpoint.MapEndpoint(routeGroup);
        }

        return app;
    }
}
