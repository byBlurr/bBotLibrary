using Discord;
using Discord.Commands;
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
            StartBot(commandHandler, "NzI0Mjg0NzEwOTc2NDg3NDI1.Xu-yTA.w9DjeFqwTRoIU4MRR8GSyuBBLPc", "!"); // void StartBot(CommandHandler commandHandler, string token, string prefix = "!");
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
            foreach (var guild in GetBot().Guilds)
            {
                await guild.DefaultChannel.SendMessageAsync("The bot is ready!");
            }
        }
    }

    public class MyCommandModule : ModuleBase
    {
        [Command("who")]
        [Alias("whodis", "whoisthis")]
        private async Task WhoCommandAsync(IUser user)
        {
            await Context.Message.DeleteAsync();

            string name = user.Username + "#" + user.Discriminator;
            string ava = user.GetAvatarUrl().Replace("128", "2048");
            UserStatus status = user.Status;
            string created = user.CreatedAt.DateTime.ToUniversalTime().ToString();

            Color embedCol;
            if (status == UserStatus.Online) embedCol = Color.DarkGreen;
            else if (status == UserStatus.DoNotDisturb) embedCol = Color.DarkRed;
            else if (status == UserStatus.AFK || status == UserStatus.Idle) embedCol = Color.Orange;
            else embedCol = Color.DarkGrey;

            EmbedBuilder embed = new EmbedBuilder() { 
                Author = new EmbedAuthorBuilder() { Name = name, IconUrl = ava }, 
                Color = embedCol, 
                Description = $"Created: {created}"
            };

            await Util.SendTemporaryMessageAsync(Context.Channel as ITextChannel, null, false, embed.Build(), null, 10000);
        }
    }
}
