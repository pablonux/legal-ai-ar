using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LegalAiAr.Api.Tests;

internal static class JwtTestTokenFactory
{
    public const string SigningKeyBase64 = "dGVzdC1zaWduaW5nLWtleS0zMi1ieXRlcy1taW5pbW0hIQ==";
    public const string Issuer = "https://test-issuer";
    public const string Audience = "test-gcaas-client-id";

    public static string Create(
        string email,
        string? role = "admin",
        string? name = null,
        DateTimeOffset? expires = null)
    {
        var keyBytes = Convert.FromBase64String(SigningKeyBase64);
        var credentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new("email", email),
            new(ClaimTypes.Email, email),
            new(ClaimTypes.Name, name ?? email),
        };

        if (!string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim("roles", role));

        var token = new JwtSecurityToken(
            issuer: Issuer,
            audience: Audience,
            claims: claims,
            expires: (expires ?? DateTimeOffset.UtcNow.AddHours(1)).UtcDateTime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
