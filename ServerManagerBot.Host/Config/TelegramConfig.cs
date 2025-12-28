using JetBrains.Annotations;

namespace ServerManagerBot.Host.Config;

[UsedImplicitly]
public class TelegramConfig
{
    public string? ApiKey { get; init; }
}