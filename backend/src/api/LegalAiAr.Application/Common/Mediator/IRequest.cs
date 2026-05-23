namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Marker interface for requests that return <typeparamref name="TResponse"/>.
/// </summary>
public interface IRequest<out TResponse>;
