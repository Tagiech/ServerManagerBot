using Microsoft.Extensions.DependencyInjection;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Infrastructure.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken ct) where TRequest : IRequest<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        return await handler.Handle(request, ct);
    }
}