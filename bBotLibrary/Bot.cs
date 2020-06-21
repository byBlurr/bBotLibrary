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

            client.Log += Logger;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            var serviceProvider = ConfigureServices();
            handler = chandler;

            chandler.SetUp(serviceProvider, prefix);
            await handler.ConfigureAsync();

            await Task.Delay(-1);
        }
        public static Task Logger(LogMessage lmsg)
        {
            var cc = Console.ForegroundColor;
            switch (lmsg.Severity)
            {
                case LogSeverity.Critical:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }
            Console.WriteLine($"{DateTime.Now} [{lmsg.Severity,8}] {lmsg.Source}: {lmsg.Message}");
            Console.ForegroundColor = cc;
            return Task.CompletedTask;
        }


        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection().AddSingleton(client).AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }
    }
}
