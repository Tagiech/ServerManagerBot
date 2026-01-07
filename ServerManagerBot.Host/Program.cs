using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServerManagerBot.Application.Commands.UserCommands.Registry;
using ServerManagerBot.Application.Commands.UserCommands.Requests;
using ServerManagerBot.BackgroundServices.HostedServices;
using ServerManagerBot.Domain.Interfaces.Media;
using ServerManagerBot.Domain.Interfaces.TelegramClient;
using ServerManagerBot.Host.Config;
using ServerManagerBot.Host.Extensions;
using ServerManagerBot.Infrastructure.Integration.Media;
using ServerManagerBot.Infrastructure.Integration.Telegram;
using Telegram.Bot;
using ConfigurationProvider = ServerManagerBot.Host.Config.ConfigurationProvider;
using IConfigurationProvider = ServerManagerBot.Domain.Interfaces.Configuration.IConfigurationProvider;

namespace ServerManagerBot.Host;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);

        builder.ConfigureServices((context, services) =>
        {
            services.AddAppSettings(context.Configuration);

            services.AddMediaSearch(context.Configuration);

            services.AddClients();

            services.AddServices();

            services.AddHostedServices();
        });
        var host = builder.Build();

        await host.RunAsync();
    }

    private static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<TelegramConfig>(configuration.GetSection("Telegram"));
        services.Configure<MediaConfig>(configuration.GetSection("Media"));

        services.AddSingleton<IConfigurationProvider, ConfigurationProvider>();
    }

    private static void AddMediaSearch(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("Search", client =>
        {
            var searchHost = configuration.GetSection("Media").GetValue<string>("SearchHost");
            if (string.IsNullOrWhiteSpace(searchHost))
            {
                throw new InvalidOperationException("Media SearchHost configuration value is missing.");
            }

            client.BaseAddress = new Uri(searchHost);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddHttpClient("Posters", client =>
        {
            var postersHost = configuration.GetSection("Media").GetValue<string>("PostersHost");
            if (string.IsNullOrWhiteSpace(postersHost))
            {
                throw new InvalidOperationException("Media PostersHost configuration value is missing.");
            }

            client.BaseAddress = new Uri(postersHost);
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services.AddSingleton<MediaCatalogClient>();
        services.AddSingleton<IMediaSearchService, MediaSearchService>();
    }

    private static void AddClients(this IServiceCollection services)
    {
        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var config = provider.GetRequiredService<IOptions<TelegramConfig>>().Value;
            if (string.IsNullOrWhiteSpace(config.ApiKey))
            {
                throw new InvalidOperationException("Telegram API key is missing.");
            }

            return new TelegramBotClient(config.ApiKey);
        });
    }

    private static void AddServices(this IServiceCollection services)
    {
        services.AddMediator();

        services.AddRequestHandler<PingHandler>();
        services.AddRequestHandler<EchoRequestHandler>();
        services.AddRequestHandler<HelpRequestHandler>();
        services.AddRequestHandler<SearchMovieRequestHandler>();

        services.AddSingleton<UserGateRegistry>();
        var commandDescriptors = UserCommandDiscoverer
            .Discover(typeof(UserCommandDiscoverer).Assembly);
        services.AddSingleton<IUserCommandRegistry>(new UserCommandRegistry(commandDescriptors));
        services.AddSingleton<UserCommandDispatcher>();
        services.AddSingleton<TelegramUserResponsePresenter>();

        services.AddSingleton<ITelegramService, TelegramService>();
    }

    private static void AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<TelegramPollingHostedService>();
    }
}