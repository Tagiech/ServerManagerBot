using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("help",
    CommandSource.Text | CommandSource.InlineQuery | CommandSource.Callback,
    "Show this message", "помощь", "хелп")]
public class HelpRequest : IParsableRequest<HelpRequest, CommandResponse>
{
    public string SourceId { get; }

    public HelpRequest(string sourceId)
    {
        SourceId = sourceId;
    }

    public static HelpRequest Parse(CommandContext context) => new(context.SourceId);
}

[UsedImplicitly]
public sealed class HelpRequestHandler : IRequestHandler<HelpRequest, CommandResponse>
{
    private readonly IUserCommandRegistry _commandRegistry;

    public HelpRequestHandler(IUserCommandRegistry commandRegistry)
    {
        _commandRegistry = commandRegistry;
    }

    public Task<CommandResponse> Handle(HelpRequest request, CancellationToken ct)
    {
        var commands = _commandRegistry.GetAll();

        var helpMessage = "Available commands:\n\n" +
                          string.Join(
                              "\n",
                              commands.Select(cmd =>
                                  $"/{cmd.Name} - {cmd.Description}"));

        return Task.FromResult(new CommandResponse(request.SourceId).WithText(helpMessage));
    }
}