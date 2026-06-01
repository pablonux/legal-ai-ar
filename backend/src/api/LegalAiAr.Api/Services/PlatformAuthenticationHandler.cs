using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Authenticates only via a validated <c>id_token</c> session cookie.
/// </summary>
public class PlatformAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "Platform";

    private readonly PlatformAuthOptions _platformOptions;
    private readonly PlatformUserJwtValidator _jwtValidator;

    public PlatformAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IOptions<PlatformAuthOptions> platformOptions,
        PlatformUserJwtValidator jwtValidator)
        : base(options, logger, encoder)
    {
        _platformOptions = platformOptions.Value;
        _jwtValidator = jwtValidator;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        PlatformAuthConfigurer.Apply(_platformOptions);

        var jwt = PlatformGatewayTokenResolver.ResolveIdTokenFromCookie(Request, _platformOptions);
        if (string.IsNullOrWhiteSpace(jwt))
            return AuthenticateResult.Fail("Missing id_token session cookie.");

        if (!CanValidateJwt(_platformOptions))
            return AuthenticateResult.Fail("Session JWT validation is not configured (TenantId / ValidAudience).");

        var principal = await _jwtValidator
            .ValidateAndBuildPrincipalAsync(
                jwt,
                headerEmail: string.Empty,
                headerDisplayName: null,
                headerGroups: null,
                headerRoles: null,
                Context.RequestAborted)
            .ConfigureAwait(false);

        if (principal is null)
            return AuthenticateResult.Fail("Invalid or expired id_token.");

        return AuthenticateResult.Success(new AuthenticationTicket(principal, SchemeName));
    }

    private static bool CanValidateJwt(PlatformAuthOptions opt) =>
        !string.IsNullOrWhiteSpace(opt.MetadataAddress)
        || !string.IsNullOrWhiteSpace(opt.TenantId)
        || !string.IsNullOrWhiteSpace(opt.SigningKeyBase64);
}
