using LegalAiAr.Api.Interfaces;

namespace LegalAiAr.Api.Extensions;

public static class EndpointDiscoveryExtensions
{
    public static IServiceCollection AddLegalAiArEndpoints(this IServiceCollection services)
    {
        var endpointTypes = typeof(IEndpoint).Assembly
            .GetExportedTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false } && typeof(IEndpoint).IsAssignableFrom(t))
            .ToList();

        foreach (var type in endpointTypes)
            services.AddSingleton(type);

        services.AddSingleton<IReadOnlyList<Type>>(endpointTypes);
        return services;
    }
}
