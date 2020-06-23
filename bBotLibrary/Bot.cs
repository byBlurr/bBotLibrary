using Discord.Commands;
using Discord.Net.Bot.Database.Configs;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Discord.Net.Bot
{
    public class Bot
    {
        public static void StartBot(CommandHandler handler) => new Bot().StartAsync(handler).GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandHandler handler;
        public async Task StartAsync(CommandHandler chandler)
        {
            BotConfig config = BotConfig.CheckConfig();

            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = LogSeverity.Verbose
            });

            Console.Clear();
            client.Log += Util.Logger;

            await client.LoginAsync(TokenType.Bot, config.Token);
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = chandler;

            chandler.SetUp(serviceProvider, config.Type);
            await handler.ConfigureAsync();

            await Task.Delay(-1);
        }
        
        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection().AddSingleton(client).AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }
    }
}
