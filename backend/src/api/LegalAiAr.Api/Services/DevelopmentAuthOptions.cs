namespace LegalAiAr.Api.Services;

/// <summary>
/// Local development: inject platform identity headers when the gateway is not present.
/// </summary>
public class DevelopmentAuthOptions
{
    public const string SectionName = "Auth:Development";

    /// <summary>
    /// When true (default in Development), injects a signed <c>id_token</c> cookie when missing.
    /// </summary>
    public bool InjectIdentity { get; set; } = true;

    public string Email { get; set; } = "dev@legal-ai.local";

    public string DisplayName { get; set; } = "Dev User (local)";

    public string Role { get; set; } = "admin";
}
