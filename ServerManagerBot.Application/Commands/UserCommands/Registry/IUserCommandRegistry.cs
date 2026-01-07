namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

public interface IUserCommandRegistry
{
    CommandDescriptor? Resolve(string commandName, CommandSource commandSource);
    IReadOnlyCollection<CommandDescriptor> GetAll();
}