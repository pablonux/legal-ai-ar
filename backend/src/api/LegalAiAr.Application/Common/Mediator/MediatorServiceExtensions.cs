using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Extension methods for registering the custom mediator and its handlers in DI.
/// </summary>
public static class MediatorServiceExtensions
{
    /// <summary>
    /// Registers <see cref="IMediator"/>, all <see cref="IRequestHandler{TRequest,TResponse}"/> and
    /// <see cref="IStreamRequestHandler{TRequest,TResponse}"/> implementations found in the given assembly,
    /// and the specified open-generic pipeline behaviors.
    /// </summary>
    public static IServiceCollection AddMediator(
        this IServiceCollection services,
        Assembly assembly,
        params Type[] openBehaviors)
    {
        services.AddScoped<IMediator, Mediator>();

        RegisterHandlers(services, assembly, typeof(IRequestHandler<,>));
        RegisterHandlers(services, assembly, typeof(IStreamRequestHandler<,>));

        foreach (var behavior in openBehaviors)
        {
            if (!behavior.IsGenericTypeDefinition)
                throw new ArgumentException($"{behavior.Name} must be an open generic type.", nameof(openBehaviors));

            services.AddTransient(typeof(IPipelineBehavior<,>), behavior);
        }

        return services;
    }

    private static void RegisterHandlers(IServiceCollection services, Assembly assembly, Type openInterface)
    {
        var handlerTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == openInterface)
                .Select(i => new { Interface = i, Implementation = t }));

        foreach (var pair in handlerTypes)
            services.AddTransient(pair.Interface, pair.Implementation);
    }
}
