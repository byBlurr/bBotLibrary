using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace Discord.Net.Bot
{
    public abstract class CommandHandler
    {
        private string prefix;
        private CommandService commands;
        private static DiscordSocketClient bot;
        private IServiceProvider map;
        public void SetUp(IServiceProvider provider, string _prefix)
        {
            prefix = _prefix;
            map = provider;
            bot = map.GetService<DiscordSocketClient>();
            commands = map.GetService<CommandService>();

            bot.MessageReceived += HandleCommandAsync;
            SetupHandlers(bot);
        }

        public virtual void SetupHandlers(DiscordSocketClient bot) { }
        private async Task HandleCommandAsync(SocketMessage pMsg)
        {
            SocketUserMessage message = pMsg as SocketUserMessage;
            if (message == null) return;
            var context = new SocketCommandContext(bot, message);
            if (message.Author.IsBot) return;
                
            int argPos = 0;
            if (message.HasStringPrefix(prefix, ref argPos))
            {
                var result = await commands.ExecuteAsync(context, argPos, map);
                if (!result.IsSuccess && result.ErrorReason != "Unknown command.") Console.WriteLine(result.ErrorReason);
            }
        }

        public async Task ConfigureAsync() { await commands.AddModulesAsync(Assembly.GetEntryAssembly(), map); }

        public static DiscordSocketClient GetBot() => bot;
    }
}
