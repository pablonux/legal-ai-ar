using LegalAiAr.Core.Entities;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
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

public class PersistRulingStepTests
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
        services.AddScoped<IPersonRepository, LegalAiAr.Infrastructure.Persistence.Repositories.PersonRepository>();
        services.AddScoped<IKeywordRepository, LegalAiAr.Infrastructure.Persistence.Repositories.KeywordRepository>();
        services.AddScoped<IStatuteRepository, LegalAiAr.Infrastructure.Persistence.Repositories.StatuteRepository>();
        services.AddScoped<ICitationRepository, LegalAiAr.Infrastructure.Persistence.Repositories.CitationRepository>();
        services.AddScoped<IJudicialProceedingRepository>(_ => Substitute.For<IJudicialProceedingRepository>());
        services.AddScoped<ILogger<PersistRulingStep>>(_ => Substitute.For<ILogger<PersistRulingStep>>());
        services.AddScoped<IKeywordNormalizationService>(_ =>
        {
            var mock = Substitute.For<IKeywordNormalizationService>();
            mock.ResolveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns((int?)null);
            return mock;
        });
        services.AddScoped<IAuditService>(_ => Substitute.For<IAuditService>());
        services.AddScoped<PersistRulingStep>();

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

    private static IndexerMessage CreateIndexerMessage() =>
        new(
            DocumentId: "8048522",
            ContentHash: "abc123hash",
            SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test Case",
                RulingDate: new DateOnly(2024, 3, 15),
                CaseNumber: "CAF 9548/2021/CA1-CS1",
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
    public async Task ExecuteAsync_PersistsRulingAndRelatedEntities()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);

        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage();
        var result = await sut.ExecuteAsync(message);

        Assert.NotEqual(Guid.Empty, result.RulingId);

        var ruling = await context.Rulings
            .Include(r => r.RulingParticipations).ThenInclude(rp => rp.Person)
            .Include(r => r.RulingKeywords).ThenInclude(rk => rk.Keyword)
            .Include(r => r.RulingStatutes).ThenInclude(rs => rs.Statute)
            .FirstOrDefaultAsync(r => r.Id == result.RulingId);

        Assert.NotNull(ruling);
        Assert.Equal("8048522", ruling.ExternalId);
        Assert.Equal("abc123hash", ruling.ContentHash);
        Assert.Equal("Test Case", ruling.CaseTitle);
        Assert.Equal("CAF 9548/2021/CA1-CS1", ruling.CaseNumber);
        Assert.Single(ruling.RulingParticipations);
        Assert.Contains("Ricardo", ruling.RulingParticipations.First().Person.DisplayName);
        Assert.Single(ruling.RulingKeywords);
        Assert.Equal(2093, ruling.RulingKeywords.First().Keyword.ExternalCode);
        Assert.Single(ruling.RulingStatutes);
        Assert.Equal("24.767", ruling.RulingStatutes.First().Statute.Number);

        var citations = await context.Citations.Where(c => c.SourceRulingId == result.RulingId).ToListAsync();
        Assert.Single(citations);
        Assert.Equal("Fallos: 328:1883", citations[0].ExternalAlias);
    }

    [Fact]
    public async Task ExecuteAsync_PersistsVotesAndLinkToParticipations()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage() with
        {
            Votes = [new VoteData("Majority", "1-5", null, [new PersonData("Ricardo", "Lorenzetti", "SIGNATORY")])]
        };

        var result = await sut.ExecuteAsync(message);

        var votes = await context.Votes.Where(v => v.RulingId == result.RulingId).ToListAsync();
        Assert.Single(votes);
        Assert.Equal(Core.Enums.VoteType.Majority, votes[0].VoteType);
        Assert.Equal("1-5", votes[0].Pages);
    }

    [Fact]
    public async Task ExecuteAsync_PersistsSumariosWithKeywords()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage() with
        {
            Sumarios = [new SumarioData(176590, "Headnote text", "349", "203", 1,
                [new SumarioKeywordData(765, "BENEFICIO DE LITIGAR SIN GASTOS", 0)])]
        };

        var result = await sut.ExecuteAsync(message);

        var sumarios = await context.Sumarios
            .Include(s => s.SumarioKeywords).ThenInclude(sk => sk.Keyword)
            .Where(s => s.RulingId == result.RulingId)
            .ToListAsync();
        Assert.Single(sumarios);
        Assert.Equal(176590, sumarios[0].ExternalId);
        Assert.Equal("Headnote text", sumarios[0].Text);
        Assert.Equal("349", sumarios[0].Volume);
        Assert.Single(sumarios[0].SumarioKeywords);
    }

    [Fact]
    public async Task ExecuteAsync_PersistsSynthesesAndLinks()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage() with
        {
            Syntheses = [new SynthesisData("Synthesis text", 0)],
            Links = [new LinkData("http://example.com/dict.pdf", "DICTAMEN", "INTERNAL")]
        };

        var result = await sut.ExecuteAsync(message);

        var syntheses = await context.RulingSyntheses
            .Where(s => s.RulingId == result.RulingId).ToListAsync();
        Assert.Single(syntheses);
        Assert.Equal("Synthesis text", syntheses[0].Text);

        var links = await context.RulingLinks
            .Where(l => l.RulingId == result.RulingId).ToListAsync();
        Assert.Single(links);
        Assert.Equal("http://example.com/dict.pdf", links[0].Url);
        Assert.Equal("DICTAMEN", links[0].Title);
    }

    [Fact]
    public async Task ExecuteAsync_PersistsProsecutorOpinionWithPersonAndProsecutorParticipation()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage() with
        {
            ProsecutorOpinion = new ProsecutorOpinionData(
                "Laura Monti", "Favorable al recurso", "UPHOLDS", true,
                "http://csjn.gov.ar/dictamen123.pdf")
        };

        var result = await sut.ExecuteAsync(message);

        var opinion = await context.ProsecutorOpinions
            .Include(po => po.Person)
            .FirstOrDefaultAsync(po => po.RulingId == result.RulingId);
        Assert.NotNull(opinion);
        Assert.Equal("Laura Monti", opinion.ProsecutorName);
        Assert.Equal("http://csjn.gov.ar/dictamen123.pdf", opinion.DocumentBlobPath);
        Assert.NotNull(opinion.PersonId);
        Assert.NotNull(opinion.Person);
        Assert.Contains("Monti", opinion.Person!.DisplayName);

        var participations = await context.RulingParticipations
            .Where(rp => rp.RulingId == result.RulingId && rp.Role == Core.Enums.RulingRole.PROSECUTOR)
            .ToListAsync();
        Assert.Single(participations);
        Assert.Equal(opinion.PersonId, participations[0].PersonId);
    }

    [Fact]
    public async Task ExecuteAsync_PersistsStructuredStatuteArticles()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage() with
        {
            Statutes = [new StatuteData("20.744", "LCT", "art. 14 inc. b",
                [new StatuteArticleData("14", "b")])]
        };

        var result = await sut.ExecuteAsync(message);

        var articles = await context.RulingStatuteArticles.ToListAsync();
        Assert.Single(articles);
        Assert.Equal("14", articles[0].Article);
        Assert.Equal("b", articles[0].Subsection);
    }

    [Fact]
    public async Task ExecuteAsync_WithParties_PersistsProceedingParties()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProviderWithRealProceedings(dbName);
        using var scope = sp.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage() with
        {
            Parties = new List<PartyData>
            {
                new("González, Pedro", "PLAINTIFF", "Physical"),
                new("Estado Nacional", "DEFENDANT", "Legal")
            }
        };

        var result = await sut.ExecuteAsync(message);

        var proceeding = await context.JudicialProceedings
            .Include(p => p.Parties)
                .ThenInclude(pp => pp.Person)
            .FirstOrDefaultAsync(p => p.CaseNumber == "CAF 9548/2021/CA1-CS1");

        Assert.NotNull(proceeding);
        Assert.Equal(2, proceeding.Parties.Count);

        var plaintiff = proceeding.Parties.FirstOrDefault(p => p.Role == Core.Enums.PartyRole.PLAINTIFF);
        Assert.NotNull(plaintiff);
        Assert.Contains("González", plaintiff.Person.DisplayName);

        var defendant = proceeding.Parties.FirstOrDefault(p => p.Role == Core.Enums.PartyRole.DEFENDANT);
        Assert.NotNull(defendant);
        Assert.Contains("Estado Nacional", defendant.Person.DisplayName);
    }

    private static ServiceProvider CreateServiceProviderWithRealProceedings(string dbName)
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
        services.AddScoped<IPersonRepository, LegalAiAr.Infrastructure.Persistence.Repositories.PersonRepository>();
        services.AddScoped<IKeywordRepository, LegalAiAr.Infrastructure.Persistence.Repositories.KeywordRepository>();
        services.AddScoped<IStatuteRepository, LegalAiAr.Infrastructure.Persistence.Repositories.StatuteRepository>();
        services.AddScoped<ICitationRepository, LegalAiAr.Infrastructure.Persistence.Repositories.CitationRepository>();
        services.AddScoped<IJudicialProceedingRepository, LegalAiAr.Infrastructure.Persistence.Repositories.JudicialProceedingRepository>();
        services.AddScoped<ILogger<PersistRulingStep>>(_ => Substitute.For<ILogger<PersistRulingStep>>());
        services.AddScoped<IKeywordNormalizationService>(_ =>
        {
            var mock = Substitute.For<IKeywordNormalizationService>();
            mock.ResolveAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns((int?)null);
            return mock;
        });
        services.AddScoped<IAuditService>(_ => Substitute.For<IAuditService>());
        services.AddScoped<PersistRulingStep>();

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

    [Fact]
    public async Task ExecuteAsync_RecordsAuditTrail()
    {
        var dbName = Guid.NewGuid().ToString();
        await using var sp = CreateServiceProvider(dbName);
        using var scope = sp.CreateScope();
        var auditService = scope.ServiceProvider.GetRequiredService<IAuditService>();
        var sut = scope.ServiceProvider.GetRequiredService<PersistRulingStep>();

        var message = CreateIndexerMessage();
        await sut.ExecuteAsync(message);

        await auditService.Received(1).RecordFullAuditAsync(
            entityType: nameof(Ruling),
            entityId: Arg.Any<string>(),
            changes: Arg.Any<IEnumerable<FieldChange>>(),
            sourceId: 1,
            ingestionJobId: Arg.Any<Guid>(),
            operation: Core.Enums.AuditOperation.Created,
            performedBy: "IndexerWorker",
            fieldsChanged: Arg.Any<IReadOnlyList<string>>(),
            changeSummary: Arg.Any<string>(),
            entitiesCreated: 1,
            fieldsUpdated: Arg.Any<int>(),
            cancellationToken: Arg.Any<CancellationToken>());
    }

    [Fact]
    public void BuildRulingFieldChanges_BuildsChangesForNonNullFields()
    {
        var message = CreateIndexerMessage();
        var changes = PersistRulingStep.BuildRulingFieldChanges(message);

        Assert.True(changes.Count > 0);
        Assert.All(changes, c =>
        {
            Assert.Equal(Core.Enums.InferenceMethod.SourceApi, c.InferenceMethod);
            Assert.Equal(Core.Enums.ChangeType.Create, c.ChangeType);
            Assert.NotNull(c.Value);
        });

        var fieldNames = changes.Select(c => c.FieldName).ToList();
        Assert.Contains("CaseTitle", fieldNames);
        Assert.Contains("CaseNumber", fieldNames);
        Assert.Contains("RulingDate", fieldNames);
        Assert.Contains("Summary", fieldNames);
        Assert.Contains("Holding", fieldNames);
    }
}
