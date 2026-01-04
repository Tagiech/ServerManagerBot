using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("echo", "Echo back the provided message", "эхо")]
public class EchoRequest : IParsableRequest<EchoRequest, UserResponse>
{
    public string Message { get; }
    
    public EchoRequest(string message)
    {
        Message = message;
    }

    public static EchoRequest Parse(CommandContext context) => new(context.Query);
}

[UsedImplicitly]
public class EchoRequestHandler : IRequestHandler<EchoRequest, UserResponse>
{
    public Task<UserResponse> Handle(EchoRequest request, CancellationToken ct)
    {
        return Task.FromResult<UserResponse>(new TextResponse(request.Message));
    }
}