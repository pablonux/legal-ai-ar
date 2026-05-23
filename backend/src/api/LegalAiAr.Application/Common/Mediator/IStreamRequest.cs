namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Marker interface for requests that return an async stream of <typeparamref name="TResponse"/>.
/// </summary>
public interface IStreamRequest<out TResponse>;
