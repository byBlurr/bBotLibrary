using Discord;
using Discord.Net.Bot;
using Discord.WebSocket;
using LorisAngelBot.Modules;
using System;
using System.Threading.Tasks;

namespace LorisAngelBot
{
    class LCommandHandler : CommandHandler
    {
        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
        }

        private async Task ReadyAsync()
        {
            Relationships.Exists();
            await bot.SetStatusAsync(UserStatus.Online);

            //await bot.SetActivityAsync(new Game($"{bot.Guilds.Count} servers {Util.GetRandomEmoji()}", ActivityType.CustomStatus));
            var status = Task.Run(async () => {
                while (true)
                {
                    await bot.SetGameAsync($"{bot.Guilds.Count} servers {Util.GetRandomHeartEmoji()}", null, ActivityType.Watching);
                    await Task.Delay(10000);
                }
            });
            
        }
    }


    class LorisAngel : Bot
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new LCommandHandler();
            StartBot(handler);
        }
    }
}
