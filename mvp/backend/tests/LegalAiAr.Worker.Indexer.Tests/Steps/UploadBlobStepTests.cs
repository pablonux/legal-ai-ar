using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Core.Messages;
using LegalAiAr.Worker.Indexer.Steps;
using NSubstitute;

namespace LegalAiAr.Worker.Indexer.Tests.Steps;

public class UploadBlobStepTests
{
    [Fact]
    public async Task ExecuteAsync_WhenBlobExists_ReturnsBlobPath()
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.ExistsAsync("csjn/2024/8048522.pdf", Arg.Any<CancellationToken>())
            .Returns(true);

        var sut = new UploadBlobStep(blobStorage, Substitute.For<Microsoft.Extensions.Logging.ILogger<UploadBlobStep>>());
        var message = CreateIndexerMessage("csjn/2024/8048522.pdf");

        var result = await sut.ExecuteAsync(message);

        Assert.Equal("csjn/2024/8048522.pdf", result);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBlobMissing_ThrowsDomainException()
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);

        var sut = new UploadBlobStep(blobStorage, Substitute.For<Microsoft.Extensions.Logging.ILogger<UploadBlobStep>>());
        var message = CreateIndexerMessage("csjn/2024/missing.pdf");

        var ex = await Assert.ThrowsAsync<DomainException>(() => sut.ExecuteAsync(message));
        Assert.Contains("Blob not found", ex.Message);
        Assert.Contains("missing.pdf", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBlobPathEmpty_ThrowsDomainException()
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        var sut = new UploadBlobStep(blobStorage, Substitute.For<Microsoft.Extensions.Logging.ILogger<UploadBlobStep>>());
        var message = CreateIndexerMessage("");

        var ex = await Assert.ThrowsAsync<DomainException>(() => sut.ExecuteAsync(message));
        Assert.Contains("Blob path is required", ex.Message);
    }

    [Fact]
    public async Task ExecuteAsync_WhenBlobPathNull_ThrowsDomainException()
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        var sut = new UploadBlobStep(blobStorage, Substitute.For<Microsoft.Extensions.Logging.ILogger<UploadBlobStep>>());
        var message = CreateIndexerMessage(null!);

        var ex = await Assert.ThrowsAsync<DomainException>(() => sut.ExecuteAsync(message));
        Assert.Contains("Blob path is required", ex.Message);
    }

    private static IndexerMessage CreateIndexerMessage(string blobPath)
    {
        return new IndexerMessage(
            DocumentId: "8048522",
            ContentHash: "hash",
            SourceId: 1,
            Ruling: new RulingData(
                CaseTitle: "Test",
                RulingDate: DateOnly.FromDateTime(DateTime.UtcNow),
                CaseNumber: null,
                JurisdictionArea: null,
                Instance: null,
                Jurisdiction: null,
                ResourceType: null,
                RulingDirection: null,
                SubjectArea: null,
                IsUnconstitutional: false,
                Summary: null,
                Holding: null,
                FullText: "Full text",
                BlobPath: blobPath),
            Persons: [],
            Keywords: [],
            Statutes: [],
            Citations: [],
            Chunks: []);
    }
}
