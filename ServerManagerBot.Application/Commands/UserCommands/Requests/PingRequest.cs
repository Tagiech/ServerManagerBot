using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("ping",
    CommandSource.Text | CommandSource.InlineQuery | CommandSource.Callback,
    "Check bot availability",
    "пинг")]
public class PingRequest : IParsableRequest<PingRequest, CommandResponse>
{
    public string SourceId { get; }

    public PingRequest(string sourceId)
    {
        SourceId = sourceId;
    }

    public static PingRequest Parse(CommandContext context) => new(context.SourceId);
}

[UsedImplicitly]
public sealed class PingHandler : IRequestHandler<PingRequest, CommandResponse>
{
    public Task<CommandResponse> Handle(PingRequest request, CancellationToken ct)
        => Task.FromResult(new CommandResponse(request.SourceId).WithText("pong"));
}