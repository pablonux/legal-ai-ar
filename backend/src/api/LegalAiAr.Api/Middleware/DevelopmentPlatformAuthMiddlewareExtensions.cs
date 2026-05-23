namespace LegalAiAr.Api.Middleware;

public static class DevelopmentPlatformAuthMiddlewareExtensions
{
    public static IApplicationBuilder UseDevelopmentPlatformAuth(this IApplicationBuilder app)
        => app.UseMiddleware<DevelopmentPlatformAuthMiddleware>();
}
