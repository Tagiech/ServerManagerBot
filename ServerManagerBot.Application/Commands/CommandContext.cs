namespace ServerManagerBot.Application.Commands;

public class CommandContext
{
    public long UserId { get; }
    public string Query { get; }

    public CommandContext(long userId, string query)
    {
        UserId = userId;
        Query = query;
    }
}