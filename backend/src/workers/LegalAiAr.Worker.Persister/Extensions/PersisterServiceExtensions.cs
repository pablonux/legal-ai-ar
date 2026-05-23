using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Pipeline.Strategies;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Pipeline;
using LegalAiAr.Worker.Persister.Strategies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LegalAiAr.Worker.Persister.Extensions;

public static class PersisterServiceExtensions
{
    public static IServiceCollection AddPersisterServices(this IServiceCollection services)
    {
        services.AddScoped<IStrategyResolver<IPersistStrategy>>(sp =>
        {
            var blobStorage = sp.GetRequiredService<IBlobStorageService>();
            var builder = new StrategyResolverBuilder<IPersistStrategy>();

            var rulingStrategy = new RulingPersistStrategy(
                sp.GetRequiredService<AppDbContext>(),
                sp.GetRequiredService<IRulingRepository>(),
                sp.GetRequiredService<ICourtRepository>(),
                sp.GetRequiredService<IPersonRepository>(),
                sp.GetRequiredService<IKeywordRepository>(),
                sp.GetRequiredService<IStatuteRepository>(),
                sp.GetRequiredService<ICitationRepository>(),
                sp.GetRequiredService<IJudicialProceedingRepository>(),
                sp.GetRequiredService<IKeywordNormalizationService>(),
                blobStorage,
                sp.GetRequiredService<IRulingReprocessRequestRepository>(),
                sp.GetRequiredService<ILogger<RulingPersistStrategy>>());

            // CSJN rulings
            builder.Register(EntityType.Ruling, 1, rulingStrategy);
            // SAIJ rulings (same strategy, different source)
            builder.Register(EntityType.Ruling, 3, rulingStrategy);

            // SAIJ legislation
            builder.Register(EntityType.Statute, 2,
                new StatutePersistStrategy(
                    sp.GetRequiredService<IStatuteRepository>(),
                    blobStorage,
                    sp.GetRequiredService<ILogger<StatutePersistStrategy>>()));

            return builder.Build();
        });

        return services;
    }
}
