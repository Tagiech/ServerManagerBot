using System.Net.Http.Json;

namespace ServerManagerBot.Infrastructure.Integration.Media;

public class MediaCatalogClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MediaCatalogClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MediaSearchDto[]?> SearchMedia(string query, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("Search");

        var searchQuery = Uri.EscapeDataString(query);
        var response = await client.GetAsync($"?query={searchQuery}", ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MediaSearchDto[]>(cancellationToken: ct);
    }

    public async Task<byte[]> GetMediaPoster(int id, CancellationToken ct)
    {
        var client = _httpClientFactory.CreateClient("Posters");

        var response = await client.GetAsync($"{id}.jpg", ct);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    public Uri GetMediaPosterUri(int id)
    {
        var client = _httpClientFactory.CreateClient("Posters");
        return new Uri(client.BaseAddress!, $"{id}.jpg");
    }
}