namespace LegalAiAr.Api.Services;

/// <summary>
/// Resolves platform auth options at startup (OIDC metadata from tenant, validation).
/// </summary>
public static class PlatformAuthConfigurer
{
    public const string EntraMetadataTemplate =
        "https://login.microsoftonline.com/{0}/v2.0/.well-known/openid-configuration";

    public static void Apply(PlatformAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MetadataAddress)
            && !string.IsNullOrWhiteSpace(options.TenantId))
        {
            options.MetadataAddress = string.Format(
                EntraMetadataTemplate,
                options.TenantId.Trim());
        }

        if (string.IsNullOrWhiteSpace(options.EmailClaim))
            options.EmailClaim = "email";

        if (string.IsNullOrWhiteSpace(options.RolesClaimType))
            options.RolesClaimType = "roles";
    }

    public static void ValidateForStartup(PlatformAuthOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.MetadataAddress)
            && string.IsNullOrWhiteSpace(options.SigningKeyBase64))
        {
            throw new InvalidOperationException(
                "Auth:Platform requires TenantId (or MetadataAddress) or SigningKeyBase64 for session JWT validation.");
        }

        if (string.IsNullOrWhiteSpace(options.ValidAudience)
            && string.IsNullOrWhiteSpace(options.SigningKeyBase64))
        {
            throw new InvalidOperationException(
                "Auth:Platform requires ValidAudience (App Registration client id from id_token aud) " +
                "or SigningKeyBase64 for development.");
        }
    }
}
