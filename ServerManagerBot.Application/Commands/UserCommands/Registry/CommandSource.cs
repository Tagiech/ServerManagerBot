namespace ServerManagerBot.Application.Commands.UserCommands.Registry;

[Flags]
public enum CommandSource
{
    Text = 1,
    Callback = 2,
    InlineQuery = 4,
    ChosenInlineResult = 8
}