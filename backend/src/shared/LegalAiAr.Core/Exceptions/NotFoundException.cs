namespace LegalAiAr.Core.Exceptions;

/// <summary>
/// Thrown when a requested entity is not found.
/// Maps to HTTP 404 in the API.
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
