using JetBrains.Annotations;

namespace ServerManagerBot.Host.Config;

[UsedImplicitly]
public class MediaConfig
{
    public string? SearchHost { get; init; }
    public string? PostersHost { get; init; }
}