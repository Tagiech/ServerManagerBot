using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands;

public interface IParsableRequest<out TRequest, TResponse> : IRequest<TResponse>
    where TRequest : IParsableRequest<TRequest, TResponse>
    where TResponse : CommandResponse
{
    static abstract TRequest Parse(CommandContext context);
}