using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Infrastructure.Outbox;

public static class OutboxServiceCollectionExtensions
{
    /// <summary>
    /// Registers transactional outbox: dispatcher, processor, and background worker.
    /// The <see cref="DispatchDomainEventsInterceptor"/> is registered with DbContext in Infrastructure setup.
    /// </summary>
    public static IServiceCollection AddLegalAiArOutbox(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection(OutboxOptions.SectionName));

        services.AddScoped<IDomainEventsDispatcher, DomainEventsDispatcher>();
        services.AddScoped<IOutboxMessageProcessor, OutboxMessageProcessor>();
        services.AddHostedService<OutboxDispatcherWorker>();

        return services;
    }
}
