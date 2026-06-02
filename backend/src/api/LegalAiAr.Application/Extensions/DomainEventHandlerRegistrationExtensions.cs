using System.Reflection;
using LegalAiAr.Core.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Extensions;

public static class DomainEventHandlerRegistrationExtensions
{
    /// <summary>
    /// Registers all concrete <see cref="IDomainEventHandler{TEvent}"/> implementations in the given assembly.
    /// </summary>
    public static IServiceCollection AddDomainEventHandlersFromAssembly(
        this IServiceCollection services,
        Assembly assembly)
    {
        var handlerOpenGeneric = typeof(IDomainEventHandler<>);

        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
                continue;

            foreach (var implementedInterface in type.GetInterfaces())
            {
                if (!implementedInterface.IsGenericType)
                    continue;

                if (implementedInterface.GetGenericTypeDefinition() != handlerOpenGeneric)
                    continue;

                services.AddScoped(implementedInterface, type);
            }
        }

        return services;
    }
}
