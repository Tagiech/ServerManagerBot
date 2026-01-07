using Microsoft.Extensions.Options;
using ServerManagerBot.Domain.Interfaces.Configuration;

namespace ServerManagerBot.Host.Config;

public class ConfigurationProvider : IConfigurationProvider
{
    private readonly TelegramConfig _telegramConfig;

    public ConfigurationProvider(IOptions<TelegramConfig> telegramConfig)
    {
        _telegramConfig = telegramConfig.Value;
    }

    public long[] GetAllowedUserIds()
    {
        return _telegramConfig.AllowedUserIds
               ?? throw new InvalidOperationException("AllowedUserIds is not configured in TelegramConfig.");
    }
}