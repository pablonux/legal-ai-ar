namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Handles a streaming request of type <typeparamref name="TRequest"/> 
/// and returns an async enumerable of <typeparamref name="TResponse"/>.
/// </summary>
public interface IStreamRequestHandler<in TRequest, out TResponse>
    where TRequest : IStreamRequest<TResponse>
{
    IAsyncEnumerable<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
