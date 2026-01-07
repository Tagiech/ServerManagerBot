using System.Net.Http.Json;

namespace ServerManagerBot.Infrastructure.Integration.Media;

public class MediaCatalogClient
{
    private readonly IHttpClientFactory _httpClientFactory;

    public MediaCatalogClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<MediaSearchDto[]?> SearchMedia(string query)
    {
        var client = _httpClientFactory.CreateClient("Search");

        var searchQuery = Uri.EscapeDataString(query);
        var response = await client.GetAsync($"?query={searchQuery}");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<MediaSearchDto[]>();
    }

    public async Task<byte[]> GetMediaPoster(int id)
    {
        var client = _httpClientFactory.CreateClient("Posters");

        var response = await client.GetAsync($"{id}.jpg");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }

    public Uri GetMediaPosterUri(int id)
    {
        var client = _httpClientFactory.CreateClient("Posters");
        return new Uri(client.BaseAddress!, $"{id}.jpg");
    }
}