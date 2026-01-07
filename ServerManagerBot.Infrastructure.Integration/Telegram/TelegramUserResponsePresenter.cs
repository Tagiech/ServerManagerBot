using ServerManagerBot.Application.Commands.UserCommands.Responses;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public sealed class TelegramUserResponsePresenter
{
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramUserResponsePresenter(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task Present(CommandResponse response, CancellationToken ct)
    {
        if (response.InlineImages is not null)
        {
            await SendInlineImages(response.SourceId, response.InlineImages, ct);
        }

        if (response.Images is not null)
        {
            foreach (var image in response.Images)
            {
                await SendImage(response.SourceId, image.Uri.ToString(), image.Caption, response.Buttons, ct);
            }
        }

        await SendText(response.SourceId, response.Text!, response.Markdown, response.Buttons, ct);
    }

    private async Task SendText(string chatId,
        string text,
        bool markdown,
        InlineButton[][]? buttons,
        CancellationToken ct)
    {
        await _telegramBotClient.SendMessage(
            chatId: chatId,
            text: text,
            parseMode: markdown ? ParseMode.MarkdownV2 : ParseMode.None,
            replyMarkup: MapInlineKeyboard(buttons),
            cancellationToken: ct);
    }

    private async Task SendImage(string chatId,
        string imageUrl,
        string? caption,
        InlineButton[][]? buttons,
        CancellationToken ct)
    {
        await _telegramBotClient.SendPhoto(
            chatId: chatId,
            photo: InputFile.FromUri(imageUrl),
            caption: caption,
            replyMarkup: MapInlineKeyboard(buttons),
            cancellationToken: ct);
    }

    private async Task SendInlineImages(string queryId, ImageData[] images, CancellationToken ct)
    {
        var searchQuery = images.Select(i =>
            new InlineQueryResultArticle(i.Id.ToString(), i.Caption,
                new InputTextMessageContent($"download {i.Id}"))
            {
                Description = i.Description,
                ThumbnailUrl = i.Uri.ToString()
            });

        await _telegramBotClient.AnswerInlineQuery(
            inlineQueryId: queryId,
            results: searchQuery.ToArray(),
            cancellationToken: ct);
    }

    private static InlineKeyboardMarkup? MapInlineKeyboard(InlineButton[][]? buttons)
    {
        if (buttons is null)
        {
            return null;
        }

        var rows = buttons
            .Select(row => row
                .Select(b =>
                {
                    if (!string.IsNullOrWhiteSpace(b.CallbackData))
                    {
                        return InlineKeyboardButton.WithCallbackData(b.Caption, b.CallbackData);
                    }

                    return InlineKeyboardButton.WithSwitchInlineQueryCurrentChat(
                        b.Caption,
                        b.InlineQuery);
                })
                .ToArray())
            .ToArray();

        return new InlineKeyboardMarkup(rows);
    }
}