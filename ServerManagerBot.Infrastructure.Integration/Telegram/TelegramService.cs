using ServerManagerBot.Application.Commands;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Domain.Interfaces.TelegramClient;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public class TelegramService : ITelegramService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUserCommandRegistry _userCommandRegistry;
    private readonly UserCommandDispatcher _userCommandDispatcher;
    private readonly UserResponsePresenter _userResponsePresenter;

    public TelegramService(ITelegramBotClient telegramBotClient,
        IUserCommandRegistry userCommandRegistry,
        UserCommandDispatcher userCommandDispatcher,
        UserResponsePresenter userResponsePresenter)
    {
        _telegramBotClient = telegramBotClient;
        _userCommandRegistry = userCommandRegistry;
        _userCommandDispatcher = userCommandDispatcher;
        _userResponsePresenter = userResponsePresenter;
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
        if (message is null || string.IsNullOrWhiteSpace(message.Text))
        {
            return;
        }
        
        var (cmd, query) = Parse(message.Text);
        var descriptor = _userCommandRegistry.Resolve(cmd);
        if (descriptor is null)
        {
            return;
        }

        var userId = message.Chat.Id;
        var commandContext = new CommandContext(userId, query);
        var response = await _userCommandDispatcher.DispatchExclusive(descriptor, commandContext, ct);
        await _userResponsePresenter.Present(response, userId, ct);
    }

    private static (string cmd, string parameters) Parse(string? messageText)
    {
        if (string.IsNullOrWhiteSpace(messageText))
        {
            return (string.Empty, string.Empty);
        }

        var text = messageText.Trim();
        
        if (text.Length > 0 && text[0] == '/')
        {
            text = text[1..].TrimStart();
        }

        var firstSpaceIndex = text.IndexOfAny([' ', '\t', '\n', '\r']);
        if (firstSpaceIndex < 0)
        {
            return (text, string.Empty);
        }

        var cmd = text[..firstSpaceIndex];
        var parameters = text[(firstSpaceIndex + 1)..].Trim();

        return (cmd, parameters);
    }

    private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}