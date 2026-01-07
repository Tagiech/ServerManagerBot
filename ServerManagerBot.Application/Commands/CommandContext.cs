using ServerManagerBot.Application.Commands.UserCommands.Registry;

namespace ServerManagerBot.Application.Commands;

public class CommandContext
{
    public string SourceId { get; }
    public string UserId { get; }
    public CommandSource Source { get; }
    public string Query { get; }

    public CommandContext(string sourceId, string userId, CommandSource source, string query)
    {
        SourceId = sourceId;
        UserId = userId;
        Source = source;
        Query = query;
    }
}