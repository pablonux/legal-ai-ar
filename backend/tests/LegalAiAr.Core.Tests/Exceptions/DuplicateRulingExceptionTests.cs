using LegalAiAr.Core.Exceptions;

namespace LegalAiAr.Core.Tests.Exceptions;

public class DuplicateRulingExceptionTests
{
    [Fact]
    public void Constructor_WithContentHash_SetsContentHashAndMessage()
    {
        const string contentHash = "abc123def456";

        var exception = new DuplicateRulingException(contentHash);

        Assert.Equal(contentHash, exception.ContentHash);
        Assert.Contains(contentHash, exception.Message);
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void DuplicateRulingException_IsDomainException()
    {
        var exception = new DuplicateRulingException("hash123");

        Assert.IsAssignableFrom<DomainException>(exception);
    }
}
