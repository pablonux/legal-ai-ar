namespace LegalAiAr.Api.Interfaces;

/// <summary>
/// Convention-based Minimal API endpoint (see architecture standard §4.4).
/// </summary>
public interface IEndpoint
{
    /// <summary>Route group segment under <c>api/{GroupName}</c> (lowercase).</summary>
    string GroupName { get; }

    /// <summary>Named authorization policy for the route group; null uses the default authenticated policy.</summary>
    string? AuthorizationPolicy => null;

    void MapEndpoint(IEndpointRouteBuilder app);
}
