namespace LegalAiAr.Core.Exceptions;

/// <summary>
/// Base exception for domain/business rule violations.
/// Use instead of generic exceptions for business errors.
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
