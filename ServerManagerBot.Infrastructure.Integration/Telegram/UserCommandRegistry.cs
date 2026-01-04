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

    public CommandDescriptor? Resolve(string commandName)
    {
        _commands.TryGetValue(commandName, out var descriptor);
        return descriptor;
    }

    public IReadOnlyCollection<CommandDescriptor> GetAll()
        => _commands.Values.DistinctBy(i => i.CommandType).ToList();
}