namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

[AttributeUsage(AttributeTargets.Class)]
public sealed class UserCommandAttribute : Attribute
{
    public string Name { get; }
    public string[] Aliases { get; }
    public string Description { get; }

    public UserCommandAttribute(string name,
        string? description = null,
        params string[]? aliases
        )
    {
        Name = name;
        Description = description ?? "";
        Aliases = aliases ?? [];
    }
}