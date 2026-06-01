using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Validates the GCaaS <c>id_token</c> session cookie.
/// </summary>
public sealed class PlatformUserJwtValidator
{
    private readonly IOptionsMonitor<PlatformAuthOptions> _options;
    private readonly ILogger<PlatformUserJwtValidator> _logger;
    private readonly SemaphoreSlim _configLock = new(1, 1);
    private ConfigurationManager<OpenIdConnectConfiguration>? _configurationManager;
    private string? _configurationManagerAddress;

    public PlatformUserJwtValidator(IOptionsMonitor<PlatformAuthOptions> options, ILogger<PlatformUserJwtValidator> logger)
    {
        _options = options;
        _logger = logger;
    }

    public async Task<ClaimsPrincipal?> ValidateAndBuildPrincipalAsync(
        string jwt,
        string headerEmail,
        string? headerDisplayName,
        string? headerGroups,
        string? headerRoles,
        CancellationToken cancellationToken)
    {
        var opt = _options.CurrentValue;
        PlatformAuthConfigurer.Apply(opt);

        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(jwt))
            return null;

        var tvp = new TokenValidationParameters
        {
            ValidateIssuer = !string.IsNullOrWhiteSpace(opt.ValidIssuer),
            ValidIssuer = opt.ValidIssuer,
            ValidateAudience = !string.IsNullOrWhiteSpace(opt.ValidAudience),
            ValidAudience = opt.ValidAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(Math.Max(0, opt.ClockSkewSeconds)),
            ValidateIssuerSigningKey = true,
            NameClaimType = ClaimTypes.Name,
            RoleClaimType = opt.RolesClaimType,
        };

        try
        {
            if (!string.IsNullOrWhiteSpace(opt.MetadataAddress))
            {
                await ApplySigningKeysFromMetadataAsync(opt.MetadataAddress, opt, tvp, cancellationToken).ConfigureAwait(false);
            }
            else if (!string.IsNullOrWhiteSpace(opt.SigningKeyBase64))
            {
                var keyBytes = Convert.FromBase64String(opt.SigningKeyBase64);
                tvp.IssuerSigningKey = new SymmetricSecurityKey(keyBytes);
            }
            else
            {
                _logger.LogError(
                    "Session JWT validation is not configured (MetadataAddress/TenantId or SigningKeyBase64).");
                return null;
            }

            var principal = handler.ValidateToken(jwt, tvp, out _);

            var jwtEmail = GetEmailFromPrincipal(principal, opt);
            if (string.IsNullOrWhiteSpace(jwtEmail))
            {
                _logger.LogWarning("Gateway JWT has no recognizable email/upn claim.");
                return null;
            }

            return PlatformJwtPrincipalNormalizer.Normalize(
                principal,
                opt,
                string.IsNullOrWhiteSpace(headerEmail) ? jwtEmail : headerEmail,
                headerDisplayName,
                headerGroups,
                headerRoles);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Gateway JWT validation failed.");
            return null;
        }
    }

    private static string? GetEmailFromPrincipal(ClaimsPrincipal principal, PlatformAuthOptions opt)
    {
        if (!string.IsNullOrWhiteSpace(opt.EmailClaim))
        {
            var v = principal.FindFirst(opt.EmailClaim)?.Value;
            if (!string.IsNullOrWhiteSpace(v))
                return v;
        }

        return principal.FindFirst(ClaimTypes.Email)?.Value
               ?? principal.FindFirst(ClaimTypes.Upn)?.Value
               ?? principal.FindFirst("preferred_username")?.Value
               ?? principal.FindFirst("email")?.Value;
    }

    private async Task ApplySigningKeysFromMetadataAsync(
        string metadataAddress,
        PlatformAuthOptions opt,
        TokenValidationParameters tvp,
        CancellationToken cancellationToken)
    {
        await _configLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_configurationManager is null || !string.Equals(_configurationManagerAddress, metadataAddress, StringComparison.Ordinal))
            {
                _configurationManager?.RequestRefresh();
                _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    metadataAddress.Trim(),
                    new OpenIdConnectConfigurationRetriever(),
                    new HttpDocumentRetriever { RequireHttps = metadataAddress.StartsWith("https://", StringComparison.OrdinalIgnoreCase) });
                _configurationManagerAddress = metadataAddress.Trim();
            }

            var oidc = await _configurationManager
                .GetConfigurationAsync(cancellationToken)
                .ConfigureAwait(false);
            tvp.IssuerSigningKeys = oidc.SigningKeys;

            if (string.IsNullOrWhiteSpace(opt.ValidIssuer) && !string.IsNullOrWhiteSpace(oidc.Issuer))
            {
                tvp.ValidIssuer = oidc.Issuer;
                tvp.ValidateIssuer = true;
            }
        }
        finally
        {
            _configLock.Release();
        }
    }
}
