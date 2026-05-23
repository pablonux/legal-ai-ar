using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LegalAiAr.Api.Services;

/// <summary>
/// Issues a signed session JWT for local development (simulates GCaaS <c>id_token</c> cookie).
/// </summary>
internal static class DevelopmentSessionTokenIssuer
{
    public const string DefaultSigningKeyBase64 = "dGVzdC1zaWduaW5nLWtleS0zMi1ieXRlcy1taW5pbW0hIQ==";
    public const string DefaultIssuer = "https://legal-ai-ar-dev";
    public const string DefaultAudience = "legal-ai-ar-dev-client";

    public static string Create(DevelopmentAuthOptions dev, PlatformAuthOptions platform)
    {
        var keyBase64 = platform.SigningKeyBase64 ?? DefaultSigningKeyBase64;
        var issuer = platform.ValidIssuer ?? DefaultIssuer;
        var audience = platform.ValidAudience ?? DefaultAudience;
        var keyBytes = Convert.FromBase64String(keyBase64);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("email", dev.Email),
            new(ClaimTypes.Email, dev.Email),
            new(ClaimTypes.Name, dev.DisplayName),
            new(ClaimTypes.Role, dev.Role),
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
