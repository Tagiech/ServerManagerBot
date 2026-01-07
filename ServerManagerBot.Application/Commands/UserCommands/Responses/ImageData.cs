namespace ServerManagerBot.Application.Commands.UserCommands.Responses;

public class ImageData
{
    public int Id { get; }
    public Uri Uri { get; }
    public string Caption { get; }
    public string Description { get; }

    public ImageData(int id, Uri uri, string caption, string description)
    {
        Id = id;
        Uri = uri;
        Caption = caption;
        Description = description;
    }
}