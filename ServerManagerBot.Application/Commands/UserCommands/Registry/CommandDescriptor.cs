namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

public class CommandDescriptor
{
    public string Name { get; }
    public string Description { get; }
    public string[]? Aliases { get; }
    public Type CommandType { get; }
    public CommandSource Sources { get; }

    public CommandDescriptor(string name,
        string description,
        string[]? aliases,
        CommandSource sources,
        Type commandType)
    {
        Name = name;
        Description = description;
        Aliases = aliases;
        Sources = sources;
        CommandType = commandType;
    }
}