using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace ServerManagerBot.Infrastructure.Integration.Media;

[UsedImplicitly]
public class MediaSearchDto
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("value")]
    public string Value { get; set; }
    
    [JsonPropertyName("type")]
    public string Type { get; set; }
}