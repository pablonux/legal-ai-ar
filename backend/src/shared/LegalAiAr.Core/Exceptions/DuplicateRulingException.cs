namespace LegalAiAr.Core.Exceptions;

/// <summary>
/// Thrown when attempting to index a ruling that already exists (same ContentHash).
/// </summary>
public class DuplicateRulingException : DomainException
{
    public string ContentHash { get; }

    public DuplicateRulingException(string contentHash)
        : base($"A ruling with ContentHash '{contentHash}' already exists.")
    {
        ContentHash = contentHash;
    }
}
