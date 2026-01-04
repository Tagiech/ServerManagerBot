using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("ping", "Check bot availability", "пинг")]
public class PingRequest : IParsableRequest<PingRequest, UserResponse>
{
    public static PingRequest Parse(CommandContext context) => new();
}

[UsedImplicitly]
public sealed class PingHandler : IRequestHandler<PingRequest, UserResponse>
{
    public Task<UserResponse> Handle(PingRequest request, CancellationToken ct)
        => Task.FromResult<UserResponse>(new TextResponse("pong"));
}