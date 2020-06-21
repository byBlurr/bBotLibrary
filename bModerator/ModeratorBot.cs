using Discord;
using Discord.Net.Bot;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace bModerator
{
    class MBot : Bot
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new MCommandHandler();

            // Todo: Move token and prefix into a config file
            StartBot(handler, "NzEzODcwMzIyNjYyNzY4NzI4.Xu_IoQ.ItWLiWYGIiCrOoCIzBpcVdtuKTY", "-");
        }
    }

    class MCommandHandler : CommandHandler
    {
        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
        }

        private async Task ReadyAsync()
        {
            await GetBot().SetStatusAsync(UserStatus.DoNotDisturb);

            // Todo: Set a custom status (Not able to yet)

            await Task.CompletedTask;
        }
    }

    class MCommandModule : ModuleBase
    {

    }
}
