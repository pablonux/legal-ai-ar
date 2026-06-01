using LegalAiAr.Core.Exceptions;
using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Core.Interfaces.Services;
using LegalAiAr.Worker.Parser.Parsers;
using LegalAiAr.Worker.Parser.Tests.Fixtures;
using NSubstitute;

namespace LegalAiAr.Worker.Parser.Tests.Parsers;

public class BlobPdfExtractorTests
{
    [Fact]
    public async Task ExtractTextAsync_ValidBlobPath_ReturnsNormalizedText()
    {
        var pdfBytes = FixtureLoader.LoadBytes("minimal-ruling.pdf");
        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
        blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<Stream>(new MemoryStream(pdfBytes)));

        var normalizer = new PdfTextNormalizer();
        var pdfExtractor = new PdfTextExtractor(normalizer);
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<BlobPdfExtractor>>();
        var sut = new BlobPdfExtractor(blobStorage, pdfExtractor, logger);

        var result = await sut.ExtractTextAsync("csjn/2024/8048522.pdf");

        Assert.NotNull(result);
        Assert.Contains("Dummy PDF", result);
        await blobStorage.Received(1).ExistsAsync("csjn/2024/8048522.pdf", Arg.Any<CancellationToken>());
        await blobStorage.Received(1).DownloadAsync("csjn/2024/8048522.pdf", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtractTextAsync_BlobNotFound_ThrowsDomainException()
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(false);

        var pdfExtractor = Substitute.For<PdfTextExtractor>(Substitute.For<ITextNormalizer>());
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<BlobPdfExtractor>>();
        var sut = new BlobPdfExtractor(blobStorage, pdfExtractor, logger);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => sut.ExtractTextAsync("csjn/2024/nonexistent.pdf"));

        Assert.Contains("Blob not found", ex.Message);
        Assert.Contains("nonexistent.pdf", ex.Message);
        await blobStorage.DidNotReceive().DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData(null!)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task ExtractTextAsync_EmptyBlobPath_ThrowsArgumentException(string blobPath)
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        var pdfExtractor = Substitute.For<PdfTextExtractor>(Substitute.For<ITextNormalizer>());
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<BlobPdfExtractor>>();
        var sut = new BlobPdfExtractor(blobStorage, pdfExtractor, logger);

        var ex = await Assert.ThrowsAsync<ArgumentException>(
            () => sut.ExtractTextAsync(blobPath!));

        Assert.Equal("blobPathPdf", ex.ParamName);
        await blobStorage.DidNotReceive().ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task ExtractTextAsync_PdfExtractorThrows_WrapsInDomainException()
    {
        var blobStorage = Substitute.For<IBlobStorageService>();
        blobStorage.ExistsAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(true);
        blobStorage.DownloadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(_ => Task.FromResult<Stream>(new ThrowingStream()));

        var normalizer = new PdfTextNormalizer();
        var pdfExtractor = new PdfTextExtractor(normalizer);
        var logger = Substitute.For<Microsoft.Extensions.Logging.ILogger<BlobPdfExtractor>>();
        var sut = new BlobPdfExtractor(blobStorage, pdfExtractor, logger);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => sut.ExtractTextAsync("csjn/2024/8048522.pdf"));

        Assert.Contains("Failed to extract text", ex.Message);
        Assert.Contains("8048522.pdf", ex.Message);
        Assert.NotNull(ex.InnerException);
    }

    private sealed class ThrowingStream : Stream
    {
        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new InvalidOperationException("PdfPig failed");
        public override long Position { get => throw new InvalidOperationException("PdfPig failed"); set => throw new InvalidOperationException("PdfPig failed"); }

        public override int Read(byte[] buffer, int offset, int count) =>
            throw new InvalidOperationException("PdfPig failed");

        public override long Seek(long offset, SeekOrigin origin) => throw new InvalidOperationException("PdfPig failed");
        public override void SetLength(long value) => throw new InvalidOperationException("PdfPig failed");
        public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException("PdfPig failed");
        public override void Flush() { }
    }
}
