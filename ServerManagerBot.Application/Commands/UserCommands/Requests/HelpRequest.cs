using JetBrains.Annotations;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Responses;
using ServerManagerBot.Domain.Interfaces.Mediator;

namespace ServerManagerBot.Application.Commands.UserCommands.Requests;

[UsedImplicitly]
[UserCommand("help", "Show this message", "помощь", "хелп")]
public class HelpRequest : IParsableRequest<HelpRequest, UserResponse>
{
    public static HelpRequest Parse(CommandContext context) => new();
}

[UsedImplicitly]
public sealed class HelpRequestHandler : IRequestHandler<HelpRequest, UserResponse>
{
    private readonly IUserCommandRegistry _commandRegistry;

    public HelpRequestHandler(IUserCommandRegistry commandRegistry)
    {
        _commandRegistry = commandRegistry;
    }

    public Task<UserResponse> Handle(HelpRequest request, CancellationToken ct)
    {
        var commands = _commandRegistry.GetAll();

        var helpMessage = "Available commands:\n\n" +
                          string.Join(
                              "\n",
                              commands.Select(cmd =>
                                  $"/{cmd.Name} - {cmd.Description}"));

        return Task.FromResult<UserResponse>(new TextResponse(helpMessage));
    }
}