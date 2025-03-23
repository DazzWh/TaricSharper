using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Taric.Services;

using var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables(); // Get "DiscordToken" from environment variables
    })
    .ConfigureServices(services =>
    {
        services.AddSingleton<DiscordSocketClient>(x => new DiscordSocketClient(new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | 
                             GatewayIntents.GuildMembers | 
                             GatewayIntents.GuildMessages |
                             GatewayIntents.MessageContent
        }));
        services.AddSingleton<InteractionService>(x =>
            new InteractionService(x.GetRequiredService<DiscordSocketClient>()));
        services.AddHostedService<InteractionHandlingService>();
        services.AddHostedService<StartupService>();
    })
    .Build();

await host.RunAsync();