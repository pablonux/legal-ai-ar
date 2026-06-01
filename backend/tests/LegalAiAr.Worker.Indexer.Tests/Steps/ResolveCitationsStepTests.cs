using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Enums;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Messages;
using LegalAiAr.Infrastructure.Caching;
using LegalAiAr.Infrastructure.Persistence;
using LegalAiAr.Worker.Indexer.Steps;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace LegalAiAr.Worker.Indexer.Tests.Steps;

public class ResolveCitationsStepTests
{
    private static ServiceProvider CreateServiceProvider(string dbName)
    {
        var services = new ServiceCollection();
        services.AddDbContext<AppDbContext>(opts =>
        {
            opts.UseInMemoryDatabase(dbName);
            opts.ConfigureWarnings(w => w.Ignore(InMemoryEventId.TransactionIgnoredWarning));
        });
        services.AddSingleton<EntityCacheService>();
        services.AddSingleton<ILogger<EntityCacheService>>(NullLogger<EntityCacheService>.Instance);
        services.AddScoped<IRulingRepository, LegalAiAr.Infrastructure.Persistence.Repositories.RulingRepository>();
        services.AddScoped<ICourtRepository, LegalAiAr.Infrastructure.Persistence.Repositories.CourtRepository>();
        services.AddScoped<ICitationRepository, LegalAiAr.Infrastructure.Persistence.Repositories.CitationRepository>();
        services.AddScoped<ILogger<ResolveCitationsStep>>(_ => Substitute.For<ILogger<ResolveCitationsStep>>());
        services.AddScoped<ResolveCitationsStep>();

        var sp = services.BuildServiceProvider();
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
        if (!context.Sources.Any())
        {
            context.Sources.Add(new Source { Id = 1, Name = "CSJN", FullName = "Corte Suprema", BaseUrl = "", Strategy = "api-first", IsActive = true });
            context.SaveChanges();
        }
        return sp;
    }

    private static IndexerMessage CreateIndexerMessage(string? caseNumber = "CAF 9548/2021/CA1-CS1") =>
        new(
            DocumentId: "8048522",
            ContentHash: "abc123hash",
            SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test Case",
                RulingDate: new DateOnly(2024, 3, 15),
                CaseNumber: caseNumber,
                JurisdictionArea: "Tributario",
                Instance: "CSJN",
                Jurisdiction: null,
                ResourceType: null,
                RulingDirection: "UPHOLDS",
                SubjectArea: null,
                IsUnconstitutional: false,
                Summary: "Summary",
                Holding: "Holding",
                FullText: "Full text of the ruling.",
                BlobPath: "csjn/2024/8048522.pdf"),
            Persons: [new PersonData("Ricardo", "Lorenzetti", "SIGNATORY")],
            Keywords: [new KeywordData(2093, "IMPUESTO A LAS GANANCIAS", 0)],
            Statutes: [new StatuteData("24.767", "Ley de Concursos", "art. 14")],
            Citations: [new CitationData("Fallos: 328:1883", 56748, "CITES")],
            Chunks: [new ChunkData(0, "Chunk text")]);

    [Fact]
    public async Task ExecuteAsync_InboundCitationMatch_UpdatesTargetRulingId()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);

        using (var scope = sp.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var courtRepo = scope.ServiceProvider.GetRequiredService<ICourtRepository>();
            var court = await courtRepo.GetOrCreateAsync("CSJN", "Nacional", "Argentina", "CSJN", cancellationToken: default);

            var rulingA = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "111",
                ContentHash = "hashA",
                CaseTitle = "Ruling A",
                CaseNumber = "CAF 111/2020",
                RulingDate = new DateOnly(2024, 1, 1),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingA);

            var rulingB = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "8048522",
                ContentHash = "abc123hash",
                CaseTitle = "Test Case",
                CaseNumber = "CAF 9548/2021/CA1-CS1",
                RulingDate = new DateOnly(2024, 3, 15),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingB);

            var citation = new Citation
            {
                SourceRulingId = rulingA.Id,
                ExternalAlias = "CAF 9548/2021/CA1-CS1",
                TargetRulingId = null,
                CitationType = Core.Enums.CitationType.CITES
            };
            context.Citations.Add(citation);
            await context.SaveChangesAsync();
        }

        using (var scope = sp.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var rulingB = await context.Rulings.FirstAsync(r => r.ExternalId == "8048522");
            var sut = scope.ServiceProvider.GetRequiredService<ResolveCitationsStep>();
            var message = CreateIndexerMessage();

            await sut.ExecuteAsync(rulingB.Id, message);

            var citation = await context.Citations.FirstAsync(c => c.SourceRulingId != rulingB.Id);
            Assert.Equal(rulingB.Id, citation.TargetRulingId);
        }
    }

    [Fact]
    public async Task ExecuteAsync_OutboundTargetExists_UpdatesTargetRulingId()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);

        Guid rulingAId;
        Guid rulingBId;
        using (var scope = sp.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var courtRepo = scope.ServiceProvider.GetRequiredService<ICourtRepository>();
            var court = await courtRepo.GetOrCreateAsync("CSJN", "Nacional", "Argentina", "CSJN", cancellationToken: default);

            var rulingA = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "8048522",
                ContentHash = "abc123hash",
                CaseTitle = "New Ruling",
                CaseNumber = "CAF 9548/2021/CA1-CS1",
                RulingDate = new DateOnly(2024, 3, 15),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingA);
            rulingAId = rulingA.Id;

            var rulingB = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "999",
                ContentHash = "hashB",
                CaseTitle = "Cited Ruling",
                CaseNumber = "328:1883",
                RulingDate = new DateOnly(1883, 1, 1),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingB);
            rulingBId = rulingB.Id;

            var citation = new Citation
            {
                SourceRulingId = rulingAId,
                ExternalAlias = "328:1883",
                TargetRulingId = null,
                CitationType = Core.Enums.CitationType.CITES
            };
            context.Citations.Add(citation);
            await context.SaveChangesAsync();
        }

        using (var scope = sp.CreateScope())
        {
            var sut = scope.ServiceProvider.GetRequiredService<ResolveCitationsStep>();
            var message = CreateIndexerMessage() with
            {
                Citations = [new CitationData("328:1883", null, "CITES")]
            };

            await sut.ExecuteAsync(rulingAId, message);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var citation = await context.Citations.FirstAsync(c => c.SourceRulingId == rulingAId);
            Assert.Equal(rulingBId, citation.TargetRulingId);
        }
    }

    [Fact]
    public async Task ExecuteAsync_AmbiguousOutboundMatch_LeavesTargetRulingIdNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);

        Guid rulingAId;
        using (var scope = sp.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var courtRepo = scope.ServiceProvider.GetRequiredService<ICourtRepository>();
            var court = await courtRepo.GetOrCreateAsync("CSJN", "Nacional", "Argentina", "CSJN", cancellationToken: default);

            var rulingA = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "8048522",
                ContentHash = "abc123hash",
                CaseTitle = "New Ruling",
                CaseNumber = "CAF 9548/2021",
                RulingDate = new DateOnly(2024, 3, 15),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingA);
            rulingAId = rulingA.Id;

            var rulingB1 = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "b1",
                ContentHash = "hashB1",
                CaseTitle = "Ruling B1",
                CaseNumber = "328:1883",
                RulingDate = new DateOnly(1883, 1, 1),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingB1);

            var rulingB2 = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "b2",
                ContentHash = "hashB2",
                CaseTitle = "Ruling B2",
                CaseNumber = "328:1883",
                RulingDate = new DateOnly(1883, 1, 1),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingB2);

            var citation = new Citation
            {
                SourceRulingId = rulingAId,
                ExternalAlias = "328:1883",
                TargetRulingId = null,
                CitationType = Core.Enums.CitationType.CITES
            };
            context.Citations.Add(citation);
            await context.SaveChangesAsync();
        }

        using (var scope = sp.CreateScope())
        {
            var sut = scope.ServiceProvider.GetRequiredService<ResolveCitationsStep>();
            var message = CreateIndexerMessage() with
            {
                Citations = [new CitationData("328:1883", null, "CITES")]
            };

            await sut.ExecuteAsync(rulingAId, message);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var citation = await context.Citations.FirstAsync(c => c.SourceRulingId == rulingAId);
            Assert.Null(citation.TargetRulingId);
        }
    }

    [Fact]
    public async Task ExecuteAsync_OutboundTargetNotIndexed_LeavesTargetRulingIdNull()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);

        Guid rulingAId;
        using (var scope = sp.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var courtRepo = scope.ServiceProvider.GetRequiredService<ICourtRepository>();
            var court = await courtRepo.GetOrCreateAsync("CSJN", "Nacional", "Argentina", "CSJN", cancellationToken: default);

            var rulingA = new Ruling
            {
                Id = Guid.NewGuid(),
                SourceId = 1,
                ExternalId = "8048522",
                ContentHash = "abc123hash",
                CaseTitle = "New Ruling",
                CaseNumber = "CAF 9548/2021",
                RulingDate = new DateOnly(2024, 3, 15),
                CourtId = court.Id,
                IndexedAt = DateTime.UtcNow,
                Status = RulingStatus.Indexed
            };
            context.Rulings.Add(rulingA);
            rulingAId = rulingA.Id;

            var citation = new Citation
            {
                SourceRulingId = rulingAId,
                ExternalAlias = "Fallos: 999:9999",
                TargetRulingId = null,
                CitationType = Core.Enums.CitationType.CITES
            };
            context.Citations.Add(citation);
            await context.SaveChangesAsync();
        }

        using (var scope = sp.CreateScope())
        {
            var sut = scope.ServiceProvider.GetRequiredService<ResolveCitationsStep>();
            var message = CreateIndexerMessage() with
            {
                Citations = [new CitationData("Fallos: 999:9999", null, "CITES")]
            };

            await sut.ExecuteAsync(rulingAId, message);

            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var citation = await context.Citations.FirstAsync(c => c.SourceRulingId == rulingAId);
            Assert.Null(citation.TargetRulingId);
        }
    }
}
