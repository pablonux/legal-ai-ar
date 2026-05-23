namespace LegalAiAr.Core.Exceptions;

/// <summary>
/// Thrown when CSJN search form returns an error instead of results page.
/// E.g. "La consulta arroja 16031 resultados, el máximo permitido es 5000".
/// Cannot paginate; search must be refined (e.g. narrower date range).
/// </summary>
public class CsjnSearchErrorException : DomainException
{
    public CsjnSearchErrorException(string message)
        : base(message)
    {
    }
}

/// <summary>
/// Thrown when CSJN search returns more results than the 5000 limit.
/// The Crawler uses ResultCount to split the date range into smaller sub-jobs.
/// </summary>
public class CsjnResultLimitExceededException : CsjnSearchErrorException
{
    public int ResultCount { get; }
    public int MaxAllowed { get; }

    public CsjnResultLimitExceededException(string message, int resultCount, int maxAllowed)
        : base(message)
    {
        ResultCount = resultCount;
        MaxAllowed = maxAllowed;
    }
}
