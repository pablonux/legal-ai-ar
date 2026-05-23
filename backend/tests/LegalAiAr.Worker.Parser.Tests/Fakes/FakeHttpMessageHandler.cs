using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

namespace LegalAiAr.Worker.Parser.Tests.Fakes;

/// <summary>
/// HttpMessageHandler for unit tests. Returns configurable responses per request.
/// Supports URL-keyed queues (for parallel HTTP) and legacy FIFO <see cref="AddResponse"/>.
/// </summary>
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly object _sync = new();
    private int _sequentialIndex;
    private readonly List<Func<HttpRequestMessage, Task<HttpResponseMessage>>> _sequential = [];
    private readonly Dictionary<string, Queue<Func<HttpRequestMessage, Task<HttpResponseMessage>>>> _routes =
        new(StringComparer.OrdinalIgnoreCase);
    private int _totalCalls;

    public int CallCount => _totalCalls;

    /// <summary>
    /// Adds a response that will be returned for the next matching request (legacy FIFO when no route matches).
    /// </summary>
    public void AddResponse(HttpStatusCode statusCode, byte[]? content = null)
    {
        lock (_sync)
        {
            _sequential.Add(_ => Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = content != null ? new ByteArrayContent(content) : null
            }));
        }
    }

    /// <summary>
    /// Adds a response with string content (e.g. JSON).
    /// </summary>
    public void AddResponse(HttpStatusCode statusCode, string? content)
    {
        lock (_sync)
        {
            _sequential.Add(_ => Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = content != null ? new StringContent(content) : null
            }));
        }
    }

    /// <summary>
    /// Enqueues a response for the next request whose URL contains <paramref name="urlSubstring"/>.
    /// </summary>
    public void EnqueueResponseForUrlContains(string urlSubstring, HttpStatusCode statusCode, string? content)
    {
        lock (_sync)
        {
            if (!_routes.TryGetValue(urlSubstring, out var q))
            {
                q = new Queue<Func<HttpRequestMessage, Task<HttpResponseMessage>>>();
                _routes[urlSubstring] = q;
            }

            q.Enqueue(_ => Task.FromResult(new HttpResponseMessage(statusCode)
            {
                Content = content != null ? new StringContent(content) : null
            }));
        }
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri?.ToString() ?? string.Empty;
        Func<HttpRequestMessage, Task<HttpResponseMessage>>? handler = null;

        lock (_sync)
        {
            foreach (var key in _routes.Keys.OrderByDescending(k => k.Length))
            {
                if (url.Contains(key, StringComparison.OrdinalIgnoreCase) && _routes[key].Count > 0)
                {
                    handler = _routes[key].Dequeue();
                    break;
                }
            }

            if (handler is null && _sequentialIndex < _sequential.Count)
                handler = _sequential[_sequentialIndex++];
        }

        Interlocked.Increment(ref _totalCalls);

        if (handler is null)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent($"No response configured for URL: {url}")
            });
        }

        return handler(request);
    }
}
