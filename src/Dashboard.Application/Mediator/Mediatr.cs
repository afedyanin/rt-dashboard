using Dashboard.Common.Mediator;
using Microsoft.Extensions.DependencyInjection;

namespace Dashboard.Application.Mediator;

public class Mediatr : IMediator
{
    private readonly IServiceProvider _provider;

    public Mediatr(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        dynamic handler = _provider.GetRequiredService(handlerType) ??
            throw new InvalidOperationException($"Cannot find handler for {request.GetType().Name}");

        return await handler.Handle((dynamic)request, cancellationToken);
    }

    public async Task Send(IRequest request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<>)
            .MakeGenericType(request.GetType());

        dynamic handler = _provider.GetRequiredService(handlerType) ??
            throw new InvalidOperationException($"Cannot find handler for {request.GetType().Name}");

        await handler.Handle((dynamic)request, cancellationToken);
    }
}
