using ServerManagerBot.Application.Commands.UserCommands.Responses;
using Telegram.Bot;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public sealed class UserResponsePresenter
{
    private readonly ITelegramBotClient _telegramBotClient;

    public UserResponsePresenter(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task Present(UserResponse r, long chatId, CancellationToken ct)
    {
        switch (r)
        {
            case TextResponse t:
                await SendText(chatId, t.Text, t.Markdown, ct);
                break;

            // case ImageResponse i:
            //     await SendImage(chatId, i.Image, i.Caption, ct);
            //     break;

            case MultiResponse m:
                foreach (var item in m.Items)
                    await Present(item, chatId, ct);
                break;

            case NoResponse:
                break;

            default:
                throw new NotSupportedException(r.GetType().Name);
        }
    }
    
    private async Task SendText(long chatId, string text, bool markdown, CancellationToken ct)
    {
        await _telegramBotClient.SendMessage(
            chatId: chatId,
            text: text,
            // parseMode: markdown ?  Telegram.Bot.Types.Enums.ParseMode.MarkdownV2 : Telegram.Bot.Types.Enums.ParseMode.Default,
            cancellationToken: ct);
    }
}
