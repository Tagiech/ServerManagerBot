namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

public interface IUserCommandRegistry
{
    CommandDescriptor? Resolve(string commandName);
    IReadOnlyCollection<CommandDescriptor> GetAll();
}