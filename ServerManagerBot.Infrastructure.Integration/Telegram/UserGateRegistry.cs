using System.Collections.Concurrent;

namespace ServerManagerBot.Infrastructure.Integration.Telegram;

public class UserGateRegistry
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _gates = new();

    public SemaphoreSlim Get(string userId)
    {
        return _gates.GetOrAdd(userId, _ => new SemaphoreSlim(1, 1));
    }
}