namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Dispatches requests to their corresponding handlers, optionally applying pipeline behaviors.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends a request through the pipeline and returns the response.
    /// </summary>
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a streaming request and returns an async enumerable of response items.
    /// </summary>
    IAsyncEnumerable<TResponse> CreateStream<TResponse>(IStreamRequest<TResponse> request, CancellationToken cancellationToken = default);
}
