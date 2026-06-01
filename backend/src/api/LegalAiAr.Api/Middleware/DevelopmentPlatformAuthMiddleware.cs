using LegalAiAr.Api.Services;
using Microsoft.Extensions.Options;

namespace LegalAiAr.Api.Middleware;

/// <summary>
/// In Development, simulates GCaaS by injecting an <c>id_token</c> session cookie when absent.
/// </summary>
public sealed class DevelopmentPlatformAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptionsMonitor<DevelopmentAuthOptions> _developmentOptions;
    private readonly IOptionsMonitor<PlatformAuthOptions> _platformOptions;

    public DevelopmentPlatformAuthMiddleware(
        RequestDelegate next,
        IOptionsMonitor<DevelopmentAuthOptions> developmentOptions,
        IOptionsMonitor<PlatformAuthOptions> platformOptions)
    {
        _next = next;
        _developmentOptions = developmentOptions;
        _platformOptions = platformOptions;
    }

    public Task InvokeAsync(HttpContext context)
    {
        var dev = _developmentOptions.CurrentValue;
        if (!dev.InjectIdentity)
            return _next(context);

        var platform = _platformOptions.CurrentValue;
        if (!string.IsNullOrWhiteSpace(PlatformGatewayTokenResolver.ResolveIdTokenFromCookie(context.Request, platform)))
            return _next(context);

        var token = DevelopmentSessionTokenIssuer.Create(dev, platform);
        var cookieName = platform.IdTokenCookie;
        var existing = context.Request.Headers.Cookie.FirstOrDefault();
        var injected = string.IsNullOrWhiteSpace(existing)
            ? $"{cookieName}={token}"
            : $"{existing}; {cookieName}={token}";
        context.Request.Headers.Cookie = injected;

        return _next(context);
    }
}
