using Microsoft.Extensions.DependencyInjection;
using ServerManagerBot.Domain.Interfaces.Mediator;
using ServerManagerBot.Infrastructure.Mediator;

namespace ServerManagerBot.Host.Extensions;

public static class MediatorServiceCollectionExtensions
{
    public static void AddMediator(this IServiceCollection services)
    {
        services.AddSingleton<IMediator, Mediator>();
    }

    public static void AddRequestHandler<TRequest, THandler, TResponse>(this IServiceCollection services)
        where TRequest : class, IRequest<TResponse>
        where THandler : class, IRequestHandler<TRequest, TResponse>
    {
        services.AddTransient<IRequestHandler<TRequest, TResponse>, THandler>();
    }

    public static void AddRequestHandler<THandler>(this IServiceCollection services)
        where THandler : class
    {
        var handlerType = typeof(THandler);
        var handlerInterface = handlerType
            .GetInterfaces()
            .Single(i => i.IsGenericType &&
                         i.GetGenericTypeDefinition() == typeof(IRequestHandler<,>));
        
        services.AddTransient(handlerInterface, handlerType);
    }
}