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
            BotConfig config = CheckConfig();

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

        public BotConfig CheckConfig()
        {
            BotConfig config;

            if (BotConfig.Exists()) config = BotConfig.Load();
            else
            {
                // Create config...
                config = new BotConfig();
                Console.Clear();
                Console.WriteLine("No config file was found...\n");

                Console.WriteLine("Bot Token: ");
                Console.WriteLine("Create a bot at https://discord.com/developers \n");
                config.Token = Console.ReadLine();
                Console.Clear();

                bool typeSelected = false;
                while (!typeSelected)
                {
                    Console.WriteLine("Config Type: s | i\nS - Solo (One config for the whole bot)\nI - Individual (Different config for each guild)\n");
                    string input = Console.ReadLine();
                    if (input.ToLower().Equals("s"))
                    {
                        config.Type = ConfigType.Solo;
                        typeSelected = true;
                    }
                    else if (input.ToLower().Equals("i"))
                    {
                        config.Type = ConfigType.Individual;
                        typeSelected = true;
                    }
                    Console.Clear();
                }

                Console.WriteLine("Default Prefix: ");
                config.SoloConfig.Prefix = Console.ReadLine();
                Console.Clear();

                Console.WriteLine($"Config file created...\nToken: {config.Token}\nPrefix: {config.SoloConfig.Prefix}\nType: {config.Type}");
                config.Save();
                Console.WriteLine("Config file saved...");
                Console.Clear();
            }

            Console.WriteLine("Config file loaded.");
            return config;
        }
        
        public IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection().AddSingleton(client).AddSingleton(new CommandService(new CommandServiceConfig { CaseSensitiveCommands = false }));
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);

            return provider;
        }
    }
}
