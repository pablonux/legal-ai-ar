using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace LegalAiAr.Application.Mediation;

/// <summary>
/// Default mediator that resolves handlers from DI and wraps them with pipeline behaviors.
/// Uses reflection-based dispatch with cached method lookups.
/// </summary>
public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, RequestHandlerMetadata> _handlerCache = new();
    private static readonly ConcurrentDictionary<Type, StreamHandlerMetadata> _streamCache = new();

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var meta = _handlerCache.GetOrAdd(requestType, static rt =>
        {
            var responseType = rt.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];

            var handlerInterfaceType = typeof(IRequestHandler<,>).MakeGenericType(rt, responseType);
            var behaviorInterfaceType = typeof(IPipelineBehavior<,>).MakeGenericType(rt, responseType);

            return new RequestHandlerMetadata(
                handlerInterfaceType,
                handlerInterfaceType.GetMethod(nameof(IRequestHandler<IRequest<object>, object>.Handle))!,
                behaviorInterfaceType,
                behaviorInterfaceType.GetMethod(nameof(IPipelineBehavior<IRequest<object>, object>.Handle))!);
        });

        var handler = _serviceProvider.GetService(meta.HandlerType)
            ?? throw new InvalidOperationException(
                $"No handler registered for {requestType.Name}. Register an IRequestHandler<{requestType.Name}, {typeof(TResponse).Name}>.");

        var behaviors = _serviceProvider.GetServices(meta.BehaviorType).Cast<object>().ToList();

        if (behaviors.Count == 0)
        {
            return await (Task<TResponse>)meta.HandleMethod.Invoke(handler, [request, cancellationToken])!;
        }

        RequestHandlerDelegate<TResponse> pipeline = () =>
            (Task<TResponse>)meta.HandleMethod.Invoke(handler, [request, cancellationToken])!;

        for (var i = behaviors.Count - 1; i >= 0; i--)
        {
            var behavior = behaviors[i];
            var next = pipeline;
            pipeline = () =>
                (Task<TResponse>)meta.BehaviorHandleMethod.Invoke(behavior, [request, next, cancellationToken])!;
        }

        return await pipeline();
    }

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var meta = _streamCache.GetOrAdd(requestType, static rt =>
        {
            var responseType = rt.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IStreamRequest<>))
                .GetGenericArguments()[0];

            var handlerInterfaceType = typeof(IStreamRequestHandler<,>).MakeGenericType(rt, responseType);

            return new StreamHandlerMetadata(
                handlerInterfaceType,
                handlerInterfaceType.GetMethod(nameof(IStreamRequestHandler<IStreamRequest<object>, object>.Handle))!);
        });

        var handler = _serviceProvider.GetService(meta.HandlerType)
            ?? throw new InvalidOperationException(
                $"No stream handler registered for {requestType.Name}.");

        return (IAsyncEnumerable<TResponse>)meta.HandleMethod.Invoke(handler, [request, cancellationToken])!;
    }

    private sealed record RequestHandlerMetadata(
        Type HandlerType,
        MethodInfo HandleMethod,
        Type BehaviorType,
        MethodInfo BehaviorHandleMethod);

    private sealed record StreamHandlerMetadata(
        Type HandlerType,
        MethodInfo HandleMethod);
}
