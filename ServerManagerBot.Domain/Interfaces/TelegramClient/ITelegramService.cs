namespace ServerManagerBot.Domain.Interfaces.TelegramClient;

public interface ITelegramService
{
    Task StartAsync(CancellationToken ct);
}