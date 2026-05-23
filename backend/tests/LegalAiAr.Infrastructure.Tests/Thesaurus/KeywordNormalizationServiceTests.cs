using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Infrastructure.Thesaurus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LegalAiAr.Infrastructure.Tests.Thesaurus;

public class KeywordNormalizationServiceTests
{
    [Fact]
    public async Task ResolveAsync_WhenEntityCacheWarm_PreferredLabel_DoesNotQueryThesaurusRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        var thesaurusRepo = Substitute.For<IThesaurusRepository>();

        using var provider = BuildProvider(dbName, thesaurusRepo);
        await SeedPreferredTermAsync(provider, "DERECHO LABORAL");

        var cache = provider.GetRequiredService<EntityCacheService>();
        await cache.WarmUpAsync(provider, CancellationToken.None);

        using var scope = provider.CreateScope();
        var normalizer = scope.ServiceProvider.GetRequiredService<IKeywordNormalizationService>();

        var id = await normalizer.ResolveAsync("DERECHO LABORAL");

        Assert.NotNull(id);
        await thesaurusRepo.DidNotReceive().ResolvePreferredTermAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsync_WhenEntityCacheWarm_SynonymLabel_DoesNotQueryThesaurusRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        var thesaurusRepo = Substitute.For<IThesaurusRepository>();

        using var provider = BuildProvider(dbName, thesaurusRepo);
        await SeedPreferredAndSynonymAsync(provider, preferredLabel: "DERECHO LABORAL", synonymLabel: "LABORAL (DERECHO)");

        var cache = provider.GetRequiredService<EntityCacheService>();
        await cache.WarmUpAsync(provider, CancellationToken.None);

        using var scope = provider.CreateScope();
        var normalizer = scope.ServiceProvider.GetRequiredService<IKeywordNormalizationService>();

        var id = await normalizer.ResolveAsync("LABORAL (DERECHO)");

        Assert.NotNull(id);
        await thesaurusRepo.DidNotReceive().ResolvePreferredTermAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsync_WhenEntityCacheWarm_AccentAndSpacingVariant_DoesNotQueryThesaurusRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        var thesaurusRepo = Substitute.For<IThesaurusRepository>();

        using var provider = BuildProvider(dbName, thesaurusRepo);
        await SeedPreferredTermAsync(provider, "PRISION PREVENTIVA");

        var cache = provider.GetRequiredService<EntityCacheService>();
        await cache.WarmUpAsync(provider, CancellationToken.None);

        using var scope = provider.CreateScope();
        var normalizer = scope.ServiceProvider.GetRequiredService<IKeywordNormalizationService>();

        var id = await normalizer.ResolveAsync("  PRISIÓN   PREVENTIVA  ");

        Assert.NotNull(id);
        await thesaurusRepo.DidNotReceive().ResolvePreferredTermAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ResolveAsync_WhenLabelNotInCacheSnapshot_DelegatesToRepository()
    {
        var dbName = Guid.NewGuid().ToString();
        var thesaurusRepo = Substitute.For<IThesaurusRepository>();
        thesaurusRepo.ResolvePreferredTermAsync("SOLO EN SQL", Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<ThesaurusTerm?>(null));

        using var provider = BuildProvider(dbName, thesaurusRepo);
        await SeedPreferredTermAsync(provider, "OTRO TEMA");

        var cache = provider.GetRequiredService<EntityCacheService>();
        await cache.WarmUpAsync(provider, CancellationToken.None);

        using var scope = provider.CreateScope();
        var normalizer = scope.ServiceProvider.GetRequiredService<IKeywordNormalizationService>();

        await normalizer.ResolveAsync("SOLO EN SQL");

        await thesaurusRepo.Received(1).ResolvePreferredTermAsync("SOLO EN SQL", Arg.Any<CancellationToken>());
    }

    private static ServiceProvider BuildProvider(string dbName, IThesaurusRepository thesaurusRepo)
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(o =>
            o.UseInMemoryDatabase(dbName)
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning)));
        services.AddSingleton<EntityCacheService>();
        services.AddSingleton<ILogger<EntityCacheService>>(NullLogger<EntityCacheService>.Instance);
        services.AddSingleton<ILogger<KeywordNormalizationService>>(NullLogger<KeywordNormalizationService>.Instance);
        services.AddScoped<IThesaurusRepository>(_ => thesaurusRepo);
        services.AddScoped<IKeywordNormalizationService, KeywordNormalizationService>();
        return services.BuildServiceProvider();
    }

    private static async Task SeedPreferredTermAsync(ServiceProvider provider, string label)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.ThesaurusTerms.Add(new ThesaurusTerm
        {
            ExternalId = Random.Shared.Next(100_000, 999_999),
            Label = label,
            IsPreferred = true,
            Depth = 0,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        });
        await db.SaveChangesAsync();
    }

    private static async Task SeedPreferredAndSynonymAsync(
        ServiceProvider provider,
        string preferredLabel,
        string synonymLabel)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var preferred = new ThesaurusTerm
        {
            ExternalId = Random.Shared.Next(100_000, 999_999),
            Label = preferredLabel,
            IsPreferred = true,
            Depth = 0,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        var synonym = new ThesaurusTerm
        {
            ExternalId = Random.Shared.Next(100_000, 999_999),
            Label = synonymLabel,
            IsPreferred = false,
            Depth = 1,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
        };
        db.ThesaurusTerms.AddRange(preferred, synonym);
        await db.SaveChangesAsync();

        db.ThesaurusRelations.Add(new ThesaurusRelation
        {
            SourceTermId = preferred.Id,
            TargetTermId = synonym.Id,
            RelationType = ThesaurusRelationType.UF,
        });
        await db.SaveChangesAsync();
    }
}
