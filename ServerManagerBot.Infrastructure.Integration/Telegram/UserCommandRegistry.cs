using ServerManagerBot.Application.Commands.UserCommands.Registry;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public class UserCommandRegistry : IUserCommandRegistry
{
    private readonly Dictionary<string, CommandDescriptor> _commands;

    public UserCommandRegistry(IEnumerable<CommandDescriptor> descriptors)
    {
        _commands = new Dictionary<string, CommandDescriptor>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var descriptor in descriptors)
        {
            _commands[descriptor.Name] = descriptor;

            foreach (var alias in descriptor.Aliases ?? [])
            {
                _commands[alias] = descriptor;
            }
        }
    }

    public CommandDescriptor? Resolve(string commandName, CommandSource commandSource)
    {
        _commands.TryGetValue(commandName, out var descriptor);
        if (descriptor is not null)
        {
            return (descriptor.Sources & commandSource) != 0 ? descriptor : null;
        }

        return null;
    }

    public IReadOnlyCollection<CommandDescriptor> GetAll()
        => _commands.Values.DistinctBy(i => i.CommandType).ToList();
}