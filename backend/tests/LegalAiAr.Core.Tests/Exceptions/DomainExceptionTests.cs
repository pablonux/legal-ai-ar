using LegalAiAr.Core.Exceptions;

namespace LegalAiAr.Core.Tests.Exceptions;

public class DomainExceptionTests
{
    [Fact]
    public void Constructor_WithMessage_SetsMessage()
    {
        const string message = "Source not enabled";

        var exception = new DomainException(message);

        Assert.Equal(message, exception.Message);
    }

    [Fact]
    public void Constructor_WithMessageAndInnerException_SetsBoth()
    {
        const string message = "Domain error";
        var inner = new InvalidOperationException("Inner error");

        var exception = new DomainException(message, inner);

        Assert.Equal(message, exception.Message);
        Assert.Same(inner, exception.InnerException);
    }
}
