using LegalAiAr.Api.Services;
using Microsoft.AspNetCore.Http;

namespace LegalAiAr.Api.Tests;

public class PlatformGatewayTokenResolverTests
{
    [Fact]
    public void ResolveIdTokenFromCookie_reads_id_token_from_cookie_header()
    {
        var options = new PlatformAuthOptions();
        var context = new DefaultHttpContext();
        context.Request.Headers.Cookie = "id_token=eyJhbGciOi.test; Path=/";

        var jwt = PlatformGatewayTokenResolver.ResolveIdTokenFromCookie(context.Request, options);

        Assert.Equal("eyJhbGciOi.test", jwt);
    }

    [Fact]
    public void ResolveIdTokenFromCookie_ignores_x_user_jwt_header()
    {
        var options = new PlatformAuthOptions();
        var context = new DefaultHttpContext();
        context.Request.Headers["X-User-Jwt"] = "header-jwt-should-be-ignored";

        var jwt = PlatformGatewayTokenResolver.ResolveIdTokenFromCookie(context.Request, options);

        Assert.Null(jwt);
    }
}
