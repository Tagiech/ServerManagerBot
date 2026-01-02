namespace ServerManagerBot.Domain.Interfaces.Mediator;

public interface IMediator
{
    Task<TResponse> Send<TRequest, TResponse>(TRequest request, CancellationToken ct) 
        where TRequest : IRequest<TResponse>;
}