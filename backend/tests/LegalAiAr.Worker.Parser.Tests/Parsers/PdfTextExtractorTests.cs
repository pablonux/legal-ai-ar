using LegalAiAr.Core.Interfaces.Pipeline;
using LegalAiAr.Worker.Parser.Parsers;
using LegalAiAr.Worker.Parser.Tests.Fixtures;
using NSubstitute;

namespace LegalAiAr.Worker.Parser.Tests.Parsers;

public class PdfTextExtractorTests
{
    [Fact]
    public async Task ExtractAsync_ValidPdf_ReturnsNormalizedText()
    {
        var normalizer = new PdfTextNormalizer();
        var sut = new PdfTextExtractor(normalizer);

        using var stream = FixtureLoader.LoadStream("minimal-ruling.pdf");
        var result = await sut.ExtractAsync(stream);

        Assert.NotNull(result);
        Assert.Contains("Dummy PDF", result);
    }

    [Fact]
    public async Task ExtractAsync_NormalizerIsCalled_WithExtractedText()
    {
        var normalizer = Substitute.For<ITextNormalizer>();
        normalizer.Normalize(Arg.Any<string>()).Returns("normalized");
        var sut = new PdfTextExtractor(normalizer);

        using var stream = FixtureLoader.LoadStream("minimal-ruling.pdf");
        var result = await sut.ExtractAsync(stream);

        Assert.Equal("normalized", result);
        normalizer.Received(1).Normalize(Arg.Is<string>(s => s.Contains("Dummy PDF")));
    }

    [Fact]
    public async Task ExtractAsync_NonSeekableStream_CopiesAndExtracts()
    {
        var normalizer = new PdfTextNormalizer();
        var sut = new PdfTextExtractor(normalizer);

        var pdfBytes = FixtureLoader.LoadBytes("minimal-ruling.pdf");
        using var source = new MemoryStream(pdfBytes);
        using var nonSeekable = new NonSeekableStream(source);
        var result = await sut.ExtractAsync(nonSeekable);

        Assert.Contains("Dummy PDF", result);
    }

    private sealed class NonSeekableStream : Stream
    {
        private readonly Stream _inner;

        public NonSeekableStream(Stream inner) => _inner = inner;

        public override bool CanSeek => false;
        public override bool CanRead => true;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count) => _inner.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        public override void Flush() { }
    }
}
