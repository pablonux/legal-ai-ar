using System.Net;

namespace LegalAiAr.Infrastructure.Crawling;

/// <summary>Builds a short-lived HTTP client for a single CSJN PDF download with a custom timeout.</summary>
internal static class CsjnPdfHttp
{
    internal const string ChromeUserAgent =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";

    internal static HttpClient CreateClientForPdfDownload(string baseUrl, int timeoutSeconds)
    {
        var root = baseUrl.TrimEnd('/') + "/";
        var handler = new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All };
        var client = new HttpClient(handler)
        {
            BaseAddress = new Uri(root),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds),
        };
        client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", ChromeUserAgent);
        return client;
    }
}
