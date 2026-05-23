namespace LegalAiAr.Api.Services;

/// <summary>
/// Session authentication via GCaaS <c>id_token</c> cookie only.
/// </summary>
public class PlatformAuthOptions
{
    public const string SectionName = "Auth:Platform";

    public string IdTokenCookie { get; set; } = "id_token";

    public string? MetadataAddress { get; set; }

    public string? SigningKeyBase64 { get; set; }

    public string? ValidIssuer { get; set; }

    public string? ValidAudience { get; set; }

    public int ClockSkewSeconds { get; set; } = 300;

    public string EmailClaim { get; set; } = "email";

    public string RolesClaimType { get; set; } = "roles";

    public string? TenantId { get; set; }

    public string DefaultRole { get; set; } = PlatformRoleResolver.DefaultRole;
}
