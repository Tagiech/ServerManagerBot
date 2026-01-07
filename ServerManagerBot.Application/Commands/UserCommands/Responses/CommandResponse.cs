namespace ServerManagerBot.Application.Commands.UserCommands.Responses;

public record CommandResponse
{
    public string SourceId { get; }
    public string? Text { get; private set; }
    public bool Markdown { get; private set; }
    public ImageData[]? Images { get; private set; }
    public InlineButton[][]? Buttons { get; private set; }
    public ImageData[]? InlineImages { get; private set; }

    public CommandResponse(string sourceId)
    {
        SourceId = sourceId;
    }

    public bool IsEmpty =>
        Text is null &&
        Images is null &&
        (Buttons is null || Buttons.Length == 0) &&
        (InlineImages is null || InlineImages.Length == 0);

    public CommandResponse WithText(string text, bool markdown = false)
    {
        Text = text;
        Markdown = markdown;
        return this;
    }

    public CommandResponse WithImages(params ImageData[] imageUrls)
    {
        Images = imageUrls;
        return this;
    }

    public CommandResponse WithInlineButtons(InlineButton[][] buttons)
    {
        Buttons = buttons;
        return this;
    }

    public CommandResponse WithInlineImages(params ImageData[] inlineImages)
    {
        InlineImages = inlineImages;
        return this;
    }
}