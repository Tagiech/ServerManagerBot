using ServerManagerBot.Application.Commands;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Domain.Interfaces.Configuration;
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
    private readonly TelegramUserResponsePresenter _telegramUserResponsePresenter;
    private readonly HashSet<long> _allowedUserIds;

    public TelegramService(ITelegramBotClient telegramBotClient,
        IUserCommandRegistry userCommandRegistry,
        UserCommandDispatcher userCommandDispatcher,
        TelegramUserResponsePresenter telegramUserResponsePresenter,
        IConfigurationProvider configurationProvider)
    {
        _telegramBotClient = telegramBotClient;
        _userCommandRegistry = userCommandRegistry;
        _userCommandDispatcher = userCommandDispatcher;
        _telegramUserResponsePresenter = telegramUserResponsePresenter;
        _allowedUserIds = new HashSet<long>(configurationProvider.GetAllowedUserIds());
    }

    public Task StartAsync(CancellationToken ct)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates =
            [
                UpdateType.Message,
                UpdateType.InlineQuery,
                UpdateType.ChosenInlineResult,
                UpdateType.CallbackQuery
            ]
        };
        _telegramBotClient.StartReceiving(HandleUpdate, HandlePollingError, receiverOptions, ct);

        return Task.CompletedTask;
    }

    private async Task HandleUpdate(ITelegramBotClient botClient, Update update, CancellationToken ct)
    {
        var (descriptor, context) = update.Type switch
        {
            UpdateType.Message => await HandleTextMessage(update, ct),
            UpdateType.InlineQuery => HandleInlineQuery(update),
            UpdateType.ChosenInlineResult => HandleChosenInlineResult(update),
            UpdateType.CallbackQuery => HandleCallbackQuery(update),
            _ => (null, null)
        };
        if (descriptor is null || context is null)
        {
            return;
        }

        var response = await _userCommandDispatcher.DispatchExclusive(descriptor, context, ct);
        await _telegramUserResponsePresenter.Present(response, ct);
    }

    private async Task<(CommandDescriptor?, CommandContext?)> HandleTextMessage(Update update, CancellationToken ct)
    {
        var message = update.Message;
        var userId = message?.From?.Id ?? message?.Chat.Id;

        if (string.IsNullOrWhiteSpace(message?.Text) || userId is null || !IsUserAllowed(userId))
        {
            return (null, null);
        }

        if (message.Text == "me")
        {
            await _telegramBotClient.SendMessage(userId, $"Your userId is `{userId}`.", cancellationToken: ct);
            return (null, null);
        }

        var (cmd, query) = Parse(message.Text);
        var descriptor = _userCommandRegistry.Resolve(cmd, CommandSource.Text);
        var context = new CommandContext(userId.ToString()!, userId.ToString()!, CommandSource.Text, query);

        return (descriptor, context)!;
    }

    private (CommandDescriptor?, CommandContext?) HandleInlineQuery(Update update)
    {
        var inlineQuery = update.InlineQuery;
        var userId = inlineQuery?.From.Id;

        if (inlineQuery is null || userId is null || string.IsNullOrWhiteSpace(inlineQuery.Query) ||
            !IsUserAllowed(userId))
        {
            return (null, null);
        }

        var (cmd, query) = Parse(inlineQuery.Query);
        if (string.IsNullOrWhiteSpace(cmd))
        {
            return (null, null);
        }

        var descriptor = _userCommandRegistry.Resolve(cmd, CommandSource.InlineQuery);
        var context = new CommandContext(inlineQuery.Id, userId.ToString()!, CommandSource.InlineQuery, query);

        return (descriptor, context);
    }

    private (CommandDescriptor?, CommandContext?) HandleChosenInlineResult(Update update)
    {
        var chosen = update.ChosenInlineResult;
        var userId = chosen?.From.Id;

        if (chosen is null || string.IsNullOrWhiteSpace(chosen.Query) || userId is null || !IsUserAllowed(userId))
        {
            return (null, null);
        }

        var (cmd, query) = Parse(chosen.Query);
        if (string.IsNullOrWhiteSpace(cmd))
        {
            return (null, null);
        }

        var descriptor = _userCommandRegistry.Resolve(cmd, CommandSource.ChosenInlineResult);
        var context = new CommandContext(userId.Value.ToString(), userId.Value.ToString(),
            CommandSource.ChosenInlineResult, query);

        return (descriptor, context);
    }

    private (CommandDescriptor?, CommandContext?) HandleCallbackQuery(Update update)
    {
        var callback = update.CallbackQuery;
        var userId = callback?.From.Id;

        if (callback is null || string.IsNullOrWhiteSpace(callback.Data) || userId is null || !IsUserAllowed(userId))
        {
            return (null, null);
        }

        var (cmd, query) = Parse(callback.Data);
        if (string.IsNullOrWhiteSpace(cmd))
        {
            return (null, null);
        }

        var descriptor = _userCommandRegistry.Resolve(cmd, CommandSource.Callback);
        var context = new CommandContext(userId.Value.ToString(), userId.Value.ToString(), CommandSource.Callback,
            query);

        return (descriptor, context);
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

    private bool IsUserAllowed(long? userId)
    {
        return userId is not null && _allowedUserIds.Contains(userId.Value);
    }

    private Task HandlePollingError(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}