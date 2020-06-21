using Discord;
using Discord.Net.Bot;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace Test
{
    public class MyBot : Bot
    {
        static void Main(string[] args)
        {
            // Create a new command handler, you need to derive from CommandHandler.
            CommandHandler commandHandler = new MyCommandHandler();

            // Start the bot by passing through the command handler created and strings for token and prefix.
            StartBot(commandHandler, "NzI0Mjg0NzEwOTc2NDg3NDI1.Xu-O7w.8bpdJXN2IldCFPi7BEw4UlqVERk", "!"); // void StartBot(CommandHandler commandHandler, string token, string prefix = "!");
        }
    }

    public class MyCommandHandler : CommandHandler
    {
        // Override the SetupHandlers method, no need to call the base method
        public override void SetupHandlers(DiscordSocketClient bot)
        {
            // Add any event handlers needed, for example UserJoined
            bot.Ready += ReadyAsync;
        }

        private async Task ReadyAsync()
        {
            await Bot.Logger(new LogMessage(LogSeverity.Critical, this.ToString(), "The bot is ready!"));
        }
    }
}
