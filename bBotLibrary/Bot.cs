using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Discord.Net.Bot
{
    public class Bot
    {
        public static void StartBot(CommandHandler chandler, string token, string prefix = "!") => new Bot().StartAsync(chandler, token, prefix).GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandHandler handler;
        public async Task StartAsync(CommandHandler chandler, string token, string prefix)
        {
            client = new DiscordSocketClient(new DiscordSocketConfig()
            {
                LogLevel = Discord.LogSeverity.Verbose
            });

            client.Log += Util.Logger;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = chandler;

            chandler.SetUp(serviceProvider, prefix);
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
