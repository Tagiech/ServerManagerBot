using ServerManagerBot.Domain.Interfaces.TelegramClient;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramService(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }
    public Task StartAsync(CancellationToken ct)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message]
        };
        _telegramBotClient.StartReceiving(HandleUpdate, HandlePollingError, receiverOptions, ct);

        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var message = update.Message;
        if (message is null)
        {
            return;
        }

        if (message.Text == "ping")
        {
            await botClient.SendMessage(message.Chat.Id, "pong", cancellationToken: ct);
        }
    }
    
    private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}