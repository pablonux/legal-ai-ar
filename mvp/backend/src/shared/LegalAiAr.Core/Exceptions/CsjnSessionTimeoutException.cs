namespace LegalAiAr.Core.Exceptions;

/// <summary>
/// Thrown when CSJN returns "Ha excedido el tiempo de inactividad" (session timeout).
/// Transient: re-submit the search form and retry pagination.
/// </summary>
public class CsjnSessionTimeoutException : DomainException
{
    public CsjnSessionTimeoutException(string message)
        : base(message)
    {
    }
}
