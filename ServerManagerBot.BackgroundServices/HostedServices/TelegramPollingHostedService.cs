using Microsoft.Extensions.Hosting;
using ServerManagerBot.Domain.Interfaces.TelegramClient;

namespace ServerManagerBot.BackgroundServices.HostedServices;

public class TelegramPollingHostedService : IHostedService
{
    private readonly ITelegramService _telegramService;

    public TelegramPollingHostedService(ITelegramService telegramService)
    {
        _telegramService = telegramService;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        await _telegramService.StartAsync(ct);
    }

    public Task StopAsync(CancellationToken ct)
    {
        return Task.CompletedTask;
    }
}