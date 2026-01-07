namespace ServerManagerBot.Domain.Interfaces.Media;

public class MediaSearchResult
{
    public int Id { get; }
    public string Title { get; }
    public string Type { get; }
    public Uri ThumbnailUri { get; }

    public MediaSearchResult(int id, string title, string type, Uri thumbnailUri)
    {
        Id = id;
        Title = title;
        Type = type;
        ThumbnailUri = thumbnailUri;
    }
}