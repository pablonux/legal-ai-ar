using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Repositories;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Core.Pipeline;
using LegalAiAr.Infrastructure.Crawling;
using LegalAiAr.Infrastructure.Crawling.Options;
using LegalAiAr.Worker.Parser.Parsers;
using LegalAiAr.Worker.Parser.Tests.Fakes;
using LegalAiAr.Worker.Parser.Tests.Fixtures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace LegalAiAr.Worker.Parser.Tests.Parsers;

public class ParserProcessorTests
{
    private static readonly PipelineQueueNames QueueNames = new("pipeline");

    private static CsjnApiParser CreateCsjnApiParser(
        HttpClient httpClient,
        IOptions<CsjnApiOptions> options,
        IExternalDownloadCache downloadCache)
    {
        var gate = new CsjnApiRequestGate(options);
        var transport = new CsjnSjconsultaJsonTransport(
            httpClient,
            options,
            Substitute.For<ILogger<CsjnSjconsultaJsonTransport>>(),
            downloadCache,
            gate);
        return new CsjnApiParser(options, Substitute.For<ILogger<CsjnApiParser>>(), transport);
    }

    private static SaijLegislationParser CreateStubSaijParser() =>
        new(Substitute.For<IBlobStorageService>(),
            Substitute.For<IStatuteRepository>(),
            Substitute.For<ICitationRepository>(),
            Substitute.For<ISearchIndexService>(),
            Substitute.For<ILogger<SaijLegislationParser>>());

    private static SaijRulingParser CreateStubSaijRulingParser() =>
        new(Substitute.For<IBlobStorageService>(),
            Substitute.For<IRulingRepository>(),
            Substitute.For<ISearchIndexService>(),
            Substitute.For<ILogger<SaijRulingParser>>());

    private static void RegisterCsjnHappyPathRoutes(FakeHttpMessageHandler handler)
    {
        handler.EnqueueResponseForUrlContains("fallos/abrirAnalisis.html", System.Net.HttpStatusCode.OK, FixtureLoader.LoadJson("abrirAnalisis.json"));
        handler.EnqueueResponseForUrlContains("documentos/getAllDocumentos.html", System.Net.HttpStatusCode.OK, FixtureLoader.LoadJson("getAllDocumentos.json"));
        handler.EnqueueResponseForUrlContains("sumarios/getSumariosAnalisis.html", System.Net.HttpStatusCode.OK, FixtureLoader.LoadJson("getSumariosAnalisis.json"));
        handler.EnqueueResponseForUrlContains("documentos/getCitas.html", System.Net.HttpStatusCode.OK, FixtureLoader.LoadJson("getCitas.json"));
        handler.EnqueueResponseForUrlContains("documentos/getCitantes.html", System.Net.HttpStatusCode.OK, FixtureLoader.LoadJson("getCitantes.json"));
        handler.EnqueueResponseForUrlContains("fallos/getDictamenesAnalisis.html", System.Net.HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getSintesisAnalisis.html", System.Net.HttpStatusCode.OK, "{}");
        handler.EnqueueResponseForUrlContains("fallos/getEnlacesAnalisis.html", System.Net.HttpStatusCode.OK, "{}");
    }

    private static void AddHappyPathResponsesFromFixtures(FakeHttpMessageHandler handler) =>
        RegisterCsjnHappyPathRoutes(handler);

    private static ParserMessage CreateParserMessage() =>
        new(
            SourceId: 1,
            DocumentId: "8048522",
            AnalysisId: "804852",
            BlobPathPdf: "legal-ai-ar-kb/ruling/csjn/2024-04/8048522.pdf",
            ContentHash: "abc123",
            ApiMetadata: null);

    [Fact]
    public async Task ProcessAsync_CsjnMessage_PublishesEnrichmentMessage()
    {
        var handler = new FakeHttpMessageHandler();
        AddHappyPathResponsesFromFixtures(handler);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };

        var options = Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 3,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30,
            PreferBlobApiCacheBeforeHttp = false
        });

        var downloadCache = Substitute.For<IExternalDownloadCache>();
        downloadCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>(null));

        var csjnApiParser = CreateCsjnApiParser(httpClient, options, downloadCache);

        var blobPdfExtractor = Substitute.For<IBlobPdfExtractor>();
        blobPdfExtractor.ExtractTextAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns("Normalized PDF text content");

        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.UploadAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.ArgAt<string>(0));
        blobStorage.MoveAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.ArgAt<string>(1));

        EnrichmentMessage? capturedMessage = null;
        var queuePublisher = Substitute.For<IQueuePublisher>();
        await queuePublisher.PublishAsync(
            Arg.Any<string>(),
            Arg.Do<EnrichmentMessage>(m => capturedMessage = m),
            Arg.Any<CancellationToken>());

        var sut = new ParserProcessor(
            csjnApiParser,
            CreateStubSaijParser(),
            CreateStubSaijRulingParser(),
            blobPdfExtractor,
            blobStorage,
            queuePublisher,
            QueueNames,
            Substitute.For<ILogger<ParserProcessor>>());

        var message = CreateParserMessage();
        await sut.ProcessAsync(message);

        Assert.NotNull(capturedMessage);
        Assert.Equal("8048522", capturedMessage.DocumentId);
        Assert.Equal(1, capturedMessage.SourceId);
        Assert.Equal("", capturedMessage.NormalizedText);
        Assert.Equal("legal-ai-ar-kb/ruling/csjn/2024-01/8048522.txt", capturedMessage.TextBlobPath);
        Assert.Equal("Test Case Title", capturedMessage.ExtractedMetadata.CaseTitle);
        Assert.Equal(new DateOnly(2024, 1, 15), capturedMessage.ExtractedMetadata.RulingDate);
        Assert.Null(capturedMessage.ExtractedMetadata.Summary);
        Assert.Null(capturedMessage.ExtractedMetadata.Holding);
        Assert.Equal(["judges", "cited_statutes", "citation_types"], capturedMessage.MissingFields);
        Assert.Equal("legal-ai-ar-kb/ruling/csjn/2024-01/8048522.pdf", capturedMessage.BlobPath);
        Assert.Equal("abc123", capturedMessage.ContentHash);

        await queuePublisher.Received(1).PublishAsync(
            "pipeline-enricher",
            Arg.Any<EnrichmentMessage>(),
            Arg.Any<CancellationToken>());

        await blobPdfExtractor.Received(1).ExtractTextAsync("legal-ai-ar-kb/ruling/csjn/2024-04/8048522.pdf", Arg.Any<CancellationToken>());
        await blobStorage.Received(2).UploadAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>());
        await blobStorage.Received(1).MoveAsync(
            "legal-ai-ar-kb/ruling/csjn/2024-04/8048522.pdf",
            "legal-ai-ar-kb/ruling/csjn/2024-01/8048522.pdf",
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_SameMonth_DoesNotMoveBlob()
    {
        var handler = new FakeHttpMessageHandler();
        AddHappyPathResponsesFromFixtures(handler);
        var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://sjconsulta.csjn.gov.ar/sjconsulta/") };
        var options = Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions
        {
            BaseUrl = "https://sjconsulta.csjn.gov.ar/sjconsulta/",
            ThrottlingDelayMs = 0,
            ThrottlingMaxRetries = 3,
            ThrottlingBackoffMultiplier = 2.0,
            RequestTimeoutSeconds = 30,
            PreferBlobApiCacheBeforeHttp = false
        });
        var downloadCache = Substitute.For<IExternalDownloadCache>();
        downloadCache.GetAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<byte[]?>(null));
        var csjnApiParser = CreateCsjnApiParser(httpClient, options, downloadCache);
        var blobPdfExtractor = Substitute.For<IBlobPdfExtractor>();
        blobPdfExtractor.ExtractTextAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns("Text");
        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.UploadAsync(Arg.Any<string>(), Arg.Any<Stream>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.ArgAt<string>(0));

        var sut = new ParserProcessor(csjnApiParser, CreateStubSaijParser(), CreateStubSaijRulingParser(), blobPdfExtractor, blobStorage,
            Substitute.For<IQueuePublisher>(), QueueNames, Substitute.For<ILogger<ParserProcessor>>());

        var message = CreateParserMessage() with
        {
            BlobPathPdf = "legal-ai-ar-kb/ruling/csjn/2024-01/8048522.pdf"
        };
        await sut.ProcessAsync(message);

        await blobStorage.DidNotReceive().MoveAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_NonCsjnSource_ThrowsDomainException()
    {
        var csjnApiParser = CreateCsjnApiParser(
            new HttpClient(),
            Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions()),
            Substitute.For<IExternalDownloadCache>());
        var blobPdfExtractor = Substitute.For<IBlobPdfExtractor>();
        var blobStorage = Substitute.For<IBlobStorageService>();
        var queuePublisher = Substitute.For<IQueuePublisher>();

        var sut = new ParserProcessor(
            csjnApiParser,
            CreateStubSaijParser(),
            CreateStubSaijRulingParser(),
            blobPdfExtractor,
            blobStorage,
            queuePublisher,
            QueueNames,
            Substitute.For<ILogger<ParserProcessor>>());

        var message = CreateParserMessage() with { SourceId = 99 };

        var ex = await Assert.ThrowsAsync<DomainException>(() => sut.ProcessAsync(message));

        Assert.Contains("99", ex.Message);
        await queuePublisher.DidNotReceive().PublishAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
        await blobPdfExtractor.DidNotReceive().ExtractTextAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ProcessAsync_NullAnalysisId_ThrowsArgumentException()
    {
        var csjnApiParser = CreateCsjnApiParser(
            new HttpClient(),
            Microsoft.Extensions.Options.Options.Create(new CsjnApiOptions()),
            Substitute.For<IExternalDownloadCache>());
        var blobPdfExtractor = Substitute.For<IBlobPdfExtractor>();
        var blobStorage = Substitute.For<IBlobStorageService>();
        var queuePublisher = Substitute.For<IQueuePublisher>();

        var sut = new ParserProcessor(
            csjnApiParser,
            CreateStubSaijParser(),
            CreateStubSaijRulingParser(),
            blobPdfExtractor,
            blobStorage,
            queuePublisher,
            QueueNames,
            Substitute.For<ILogger<ParserProcessor>>());

        var message = CreateParserMessage() with { AnalysisId = null };

        var ex = await Assert.ThrowsAsync<ArgumentException>(() => sut.ProcessAsync(message));

        Assert.Equal("message", ex.ParamName);
        await queuePublisher.DidNotReceive().PublishAsync(Arg.Any<string>(), Arg.Any<object>(), Arg.Any<CancellationToken>());
    }
}
