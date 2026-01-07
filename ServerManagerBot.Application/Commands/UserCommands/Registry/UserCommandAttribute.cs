namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

[AttributeUsage(AttributeTargets.Class)]
public sealed class UserCommandAttribute : Attribute
{
    public string Name { get; }
    public string[] Aliases { get; }
    public string Description { get; }
    public CommandSource Sources { get; }

    public UserCommandAttribute(string name,
        CommandSource sources,
        string? description = null,
        params string[]? aliases
        )
    {
        Name = name;
        Sources = sources;
        Description = description ?? "";
        Aliases = aliases ?? [];
    }
}