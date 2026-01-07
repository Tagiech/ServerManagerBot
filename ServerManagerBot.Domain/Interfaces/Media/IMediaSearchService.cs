namespace ServerManagerBot.Domain.Interfaces.Media;

public interface IMediaSearchService
{
    Task<MediaSearchResult[]> SearchAsync(string query, CancellationToken ct);
}