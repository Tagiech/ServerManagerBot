using ServerManagerBot.Domain.Interfaces.Media;

namespace ServerManagerBot.Infrastructure.Integration.Media;

public class MediaSearchService : IMediaSearchService
{
    private readonly MediaCatalogClient _mediaCatalogClient;

    public MediaSearchService(MediaCatalogClient mediaCatalogClient)
    {
        _mediaCatalogClient = mediaCatalogClient;
    }

    public async Task<MediaSearchResult[]> SearchAsync(string query, CancellationToken ct)
    {
        var mediaData = await _mediaCatalogClient.SearchMedia(query, ct);
        if (mediaData is null || mediaData.Length == 0)
        {
            return [];
        }

        var searchResults = new List<MediaSearchResult>();
        foreach (var item in mediaData.Where(r => r.Type != "3d"))
        {
            var thumbnail = _mediaCatalogClient.GetMediaPosterUri(item.Id);
            if (item.Value is null || item.Type is null)
            {
                continue;
            }

            searchResults.Add(new MediaSearchResult(item.Id, item.Value, item.Type, thumbnail));
        }

        return searchResults.ToArray();
    }
}