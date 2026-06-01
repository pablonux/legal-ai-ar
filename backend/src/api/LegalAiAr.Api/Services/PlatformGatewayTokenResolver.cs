namespace LegalAiAr.Api.Services;

/// <summary>
/// Reads the GCaaS session JWT from the <c>id_token</c> cookie only.
/// </summary>
public static class PlatformGatewayTokenResolver
{
    public static string? ResolveIdTokenFromCookie(HttpRequest request, PlatformAuthOptions options)
    {
        if (!TryGetCookie(request, options.IdTokenCookie, out var idToken))
            return null;

        return idToken;
    }

    private static bool TryGetCookie(HttpRequest request, string cookieName, out string token)
    {
        token = string.Empty;
        if (string.IsNullOrWhiteSpace(cookieName))
            return false;

        if (request.Cookies.TryGetValue(cookieName, out var value) && !string.IsNullOrWhiteSpace(value))
        {
            token = value.Trim();
            return true;
        }

        var header = request.Headers.Cookie.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(header))
            return false;

        foreach (var segment in header.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var eq = segment.IndexOf('=');
            if (eq <= 0)
                continue;

            var name = segment[..eq].Trim();
            if (!string.Equals(name, cookieName, StringComparison.OrdinalIgnoreCase))
                continue;

            token = segment[(eq + 1)..].Trim().Trim('"');
            return !string.IsNullOrWhiteSpace(token);
        }

        return false;
    }
}
