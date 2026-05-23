namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Delegate representing the next step in the pipeline (either another behavior or the handler).
/// </summary>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();

/// <summary>
/// Pipeline behavior that wraps request handling, enabling cross-cutting concerns
/// such as validation, logging, and caching.
/// </summary>
public interface IPipelineBehavior<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken);
}
