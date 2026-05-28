using System.Net;
using System.Text;
using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Infrastructure.Crawling.Options;
using LegalAiAr.Worker.Parser.Parsers;
using LegalAiAr.Worker.Parser.Tests.Fakes;
using LegalAiAr.Worker.Parser.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LegalAiAr.Worker.Parser.Tests.Parsers;

public class CsjnApiParserTests
{
    private static CsjnApiParser CreateSut(
        HttpClient? httpClient = null,
        IOptions<CsjnApiOptions>? options = null,
        IExternalDownloadCache? downloadCache = null)
    {
        var opts = options ?? Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 3,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30
        });
        var client = httpClient ?? new HttpClient();
        var logger = Substitute.For<ILogger<CsjnApiParser>>();
        IExternalDownloadCache cache;
        if (downloadCache is not null)
        {
            cache = downloadCache;
        }
        else
        {
            cache = Substitute.For<IExternalDownloadCache>();
            // PreferBlobApiCacheBeforeHttp defaults to true — unconfigured substitute must MISS cache
            // (otherwise empty body is parsed as "{}" and HTTP is skipped incorrectly).
            cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<byte[]?>(null));
        }

        var gate = new CsjnApiRequestGate(opts);
        var transportLogger = Substitute.For<ILogger<CsjnSjconsultaJsonTransport>>();
        var transport = new CsjnSjconsultaJsonTransport(client, opts, transportLogger, cache, gate);
        return new CsjnApiParser(opts, logger, transport);
    }

    /// <summary>Registers HTTP 200 fixtures for all eight CSJN API paths (parallel-safe).</summary>
    private static void RegisterFullHappyPathRoutes(FakeHttpMessageHandler handler)
    {
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", HttpStatusCode.OK, FixtureLoader.LoadJson("abrirAnalisis.json"));
        RegisterPostAbrirHappyPathRoutes(handler);
    }

    private static void RegisterPostAbrirHappyPathRoutes(FakeHttpMessageHandler handler)
    {
        handler.EnqueueResponseForUrlContains("documentos/getAllDocumentos.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getAllDocumentos.json"));
        handler.EnqueueResponseForUrlContains("sumarios/getSumariosAnalisis.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getSumariosAnalisis.json"));
        handler.EnqueueResponseForUrlContains("documentos/getCitas.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getCitas.json"));
        handler.EnqueueResponseForUrlContains("documentos/getCitantes.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getCitantes.json"));
        handler.EnqueueResponseForUrlContains("fallos/getDictamenesAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getSintesisAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getEnlacesAnalisis.html", HttpStatusCode.OK, "{}");
    }

    private static void RegisterRecordsArrayHappyPathRoutes(FakeHttpMessageHandler handler)
    {
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", HttpStatusCode.OK, FixtureLoader.LoadJson("abrirAnalisis.json"));
        handler.EnqueueResponseForUrlContains("documentos/getAllDocumentos.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getAllDocumentos.json"));
        handler.EnqueueResponseForUrlContains("sumarios/getSumariosAnalisis.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getSumariosAnalisis-records.json"));
        handler.EnqueueResponseForUrlContains("documentos/getCitas.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getCitas.json"));
        handler.EnqueueResponseForUrlContains("documentos/getCitantes.html", HttpStatusCode.OK, FixtureLoader.LoadJson("getCitantes.json"));
        handler.EnqueueResponseForUrlContains("fallos/getDictamenesAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getSintesisAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getEnlacesAnalisis.html", HttpStatusCode.OK, "{}");
    }

    private static void AddHappyPathResponsesFromFixtures(FakeHttpMessageHandler handler) =>
        RegisterFullHappyPathRoutes(handler);

    /// <summary>Blob cache payloads so <see cref="CsjnApiParser.FetchMetadataAsync"/> never opens HTTP in cache-only tests.</summary>
    private static byte[]? CacheBytesForCsjnApiKey(string key)
    {
        if (key.Contains("abrirAnalisis", StringComparison.Ordinal))
            return Encoding.UTF8.GetBytes(FixtureLoader.LoadJson("abrirAnalisis.json"));
        if (key.Contains("getAllDocumentos", StringComparison.Ordinal))
            return Encoding.UTF8.GetBytes(FixtureLoader.LoadJson("getAllDocumentos.json"));
        if (key.Contains("getSumariosAnalisis", StringComparison.Ordinal))
            return Encoding.UTF8.GetBytes(FixtureLoader.LoadJson("getSumariosAnalisis.json"));
        if (key.Contains("getCitas", StringComparison.Ordinal))
            return Encoding.UTF8.GetBytes(FixtureLoader.LoadJson("getCitas.json"));
        if (key.Contains("getCitantes", StringComparison.Ordinal))
            return Encoding.UTF8.GetBytes(FixtureLoader.LoadJson("getCitantes.json"));
        if (key.Contains("getDictamenesAnalisis", StringComparison.Ordinal)
            || key.Contains("getSintesisAnalisis", StringComparison.Ordinal)
            || key.Contains("getEnlacesAnalisis", StringComparison.Ordinal))
            return "{}"u8.ToArray();
        return null;
    }

    [Fact]
    public async Task FetchMetadataAsync_HappyPath_ReturnsMergedMetadata()
    {
        var handler = new FakeHttpMessageHandler();
        AddHappyPathResponsesFromFixtures(handler);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var sut = CreateSut(httpClient: client);

        var result = await sut.FetchMetadataAsync("8048522", "804852");

        Assert.NotNull(result);
        Assert.Equal("Test Case Title", result.CaseTitle);
        Assert.Equal(new DateOnly(2024, 1, 15), result.RulingDate);
        Assert.Equal("APELACION EXTRAORDINARIA", result.Jurisdiction);
        Assert.Equal("RECURSO EXTRAORDINARIO FEDERAL", result.ResourceType);
        Assert.Equal("UPHOLDS", result.RulingDirection);
        Assert.Equal("Tributario", result.SubjectArea);
        Assert.False(result.IsUnconstitutional);
        Assert.Equal("Summary text", result.Summary);
        Assert.Equal("Holding text", result.Holding);
        Assert.Single(result.Keywords);
        Assert.Equal(2093, result.Keywords[0].ExternalCode);
        Assert.Equal("IMPUESTO A LAS GANANCIAS", result.Keywords[0].Description);
        Assert.Single(result.Citations);
        Assert.Equal("Fallos: 328:1883", result.Citations[0].Alias);
        Assert.Equal(56748, result.Citations[0].SummaryId);
        Assert.Single(result.CitedBy);
        Assert.Equal("818955", result.CitedBy[0].AnalysisId);
        Assert.Equal("CAF 003507/2024/CS001", result.CitedBy[0].CaseNumber);
    }

    [Fact]
    public async Task FetchMetadataAsync_MinimalAbrirWithoutCaratula_UsesFallbackCaseTitle()
    {
        var handler = new FakeHttpMessageHandler();
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", HttpStatusCode.OK, """{"fecha":"2024-01-15"}""");
        handler.EnqueueResponseForUrlContains("documentos/getAllDocumentos.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("sumarios/getSumariosAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("documentos/getCitas.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("documentos/getCitantes.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getDictamenesAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getSintesisAnalisis.html", HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getEnlacesAnalisis.html", HttpStatusCode.OK, "{}");
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var sut = CreateSut(httpClient: client);

        var result = await sut.FetchMetadataAsync("8048522", "804852");

        Assert.Contains("8048522", result.CaseTitle, StringComparison.Ordinal);
        Assert.Equal(new DateOnly(2024, 1, 15), result.RulingDate);
    }

    [Fact]
    public async Task FetchMetadataAsync_MissingRulingDate_ThrowsCsjnSchemaViolationException()
    {
        var handler = new FakeHttpMessageHandler();
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", HttpStatusCode.OK, """{"tituloCausa":"Test"}""");
        var client = new HttpClient(handler);
        var sut = CreateSut(httpClient: client);

        var ex = await Assert.ThrowsAsync<CsjnSchemaViolationException>(
            () => sut.FetchMetadataAsync("8048522", "804852"));

        Assert.Contains("ruling date", ex.Message);
    }

    [Fact]
    public async Task FetchMetadataAsync_NullAnalysisId_ThrowsArgumentException()
    {
        var sut = CreateSut();

        await Assert.ThrowsAsync<ArgumentException>(
            () => sut.FetchMetadataAsync("8048522", null!));

        await Assert.ThrowsAsync<ArgumentException>(
            () => sut.FetchMetadataAsync("8048522", ""));
    }

    [Fact]
    public async Task FetchMetadataAsync_RecordsArray_ReturnsMergedMetadata()
    {
        var handler = new FakeHttpMessageHandler();
        RegisterRecordsArrayHappyPathRoutes(handler);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var sut = CreateSut(httpClient: client);

        var result = await sut.FetchMetadataAsync("8048522", "804852");

        Assert.NotNull(result);
        Assert.Equal("Holding from Records array", result.Holding);
        Assert.Single(result.Keywords);
        Assert.Equal("IMPUESTO A LAS GANANCIAS", result.Keywords[0].Description);
    }

    [Fact]
    public async Task FetchMetadataAsync_SimulatedRateLimit_RetriesAndSucceeds()
    {
        var handler = new FakeHttpMessageHandler();
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", (HttpStatusCode)429, null);
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", (HttpStatusCode)429, null);
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", HttpStatusCode.OK, FixtureLoader.LoadJson("abrirAnalisis.json"));
        RegisterPostAbrirHappyPathRoutes(handler);
        var client = new HttpClient(handler);
        var opts = Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 5,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30
        });
        var sut = CreateSut(httpClient: client, options: opts);

        var result = await sut.FetchMetadataAsync("8048522", "804852");

        Assert.NotNull(result);
        Assert.Equal("Test Case Title", result.CaseTitle);
    }

    [Fact]
    public async Task FetchMetadataAsync_UseCacheTrue_ReturnsCachedResponses_NoHttpCalls()
    {
        var cache = Substitute.For<IExternalDownloadCache>();
        cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => CacheBytesForCsjnApiKey(callInfo.ArgAt<string>(0)));

        var handler = new FakeHttpMessageHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var sut = CreateSut(httpClient: client, downloadCache: cache);

        var result = await sut.FetchMetadataAsync("8048522", "804852", useCache: true);

        Assert.NotNull(result);
        Assert.Equal("Test Case Title", result.CaseTitle);
        Assert.Equal(0, handler.CallCount);
        await cache.Received(8).GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await cache.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FetchMetadataAsync_PreferBlobTrue_UseCacheFalse_StillUsesBlob_NoHttpCalls()
    {
        var cache = Substitute.For<IExternalDownloadCache>();
        cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => CacheBytesForCsjnApiKey(callInfo.ArgAt<string>(0)));

        var handler = new FakeHttpMessageHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var opts = Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 3,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30,
            PreferBlobApiCacheBeforeHttp = true
        });
        var sut = CreateSut(httpClient: client, options: opts, downloadCache: cache);

        var result = await sut.FetchMetadataAsync("8048522", "804852", useCache: false);

        Assert.NotNull(result);
        Assert.Equal("Test Case Title", result.CaseTitle);
        Assert.Equal(0, handler.CallCount);
        await cache.Received(8).GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await cache.DidNotReceive().SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FetchMetadataAsync_UseCacheFalse_WritesToCacheAfterDownload()
    {
        var cache = Substitute.For<IExternalDownloadCache>();

        var handler = new FakeHttpMessageHandler();
        AddHappyPathResponsesFromFixtures(handler);
        var client = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var opts = Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 3,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30,
            PreferBlobApiCacheBeforeHttp = false
        });
        var sut = CreateSut(httpClient: client, options: opts, downloadCache: cache);

        await sut.FetchMetadataAsync("8048522", "804852", useCache: false);

        await cache.DidNotReceive().GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await cache.Received(8).SetAsync(Arg.Any<string>(), Arg.Any<byte[]>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task FetchMetadataAsync_OutboundDisabled_CacheMiss_ThrowsInvalidOperationException()
    {
        var cache = Substitute.For<IExternalDownloadCache>();
        cache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>(null));

        var handler = new FakeHttpMessageHandler();
        var client = new HttpClient(handler);
        var opts = Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 3,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30,
            PreferBlobApiCacheBeforeHttp = false,
            OutboundHttpEnabled = false
        });
        var sut = CreateSut(httpClient: client, options: opts, downloadCache: cache);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            sut.FetchMetadataAsync("8048522", "804852", useCache: false));

        Assert.Contains("OutboundHttpEnabled=false", ex.Message, StringComparison.Ordinal);
        Assert.Equal(0, handler.CallCount);
    }
}
