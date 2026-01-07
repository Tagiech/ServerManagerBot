using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("echo", CommandSource.Text, "Echo back the provided message", "эхо")]
public class EchoRequest : IParsableRequest<EchoRequest, CommandResponse>
{
    public string SourceId { get; }
    public string Message { get; }

    public EchoRequest(string sourceId, string message)
    {
        SourceId = sourceId;
        Message = message;
    }

    public static EchoRequest Parse(CommandContext context) => new(context.SourceId, context.Query);
}

[UsedImplicitly]
public class EchoRequestHandler : IRequestHandler<EchoRequest, CommandResponse>
{
    public Task<CommandResponse> Handle(EchoRequest request, CancellationToken ct)
    {
        return Task.FromResult(new CommandResponse(request.SourceId).WithText(request.Message));
    }
}