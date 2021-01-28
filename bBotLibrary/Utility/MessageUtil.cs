using System.Threading.Tasks;

namespace Discord.Net.Bot
{
    public class MessageUtil
    {
        /// Send error message
        public static async Task SendErrorAsync(ITextChannel channel, string source, string message, bool printConsole = false)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Red,
                Title = source,
                Description = message
            };

            if (printConsole) await Util.LoggerAsync(new LogMessage(LogSeverity.Error, source, message));
            await channel.SendMessageAsync(null, false, embed.Build());
        }

        /// Post a message to a channel, automatically deletes after the time in milliseconds.
        public static async Task SendTemporaryMessageAsync(ITextChannel channel, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, int time = 5000)
        {
            var msg = await channel.SendMessageAsync(text, isTTS, embed, options);

            var message_thread = Task.Run(async () =>
            {
                await Task.Delay(time);
                await msg.DeleteAsync();
            });

            await Task.CompletedTask;
        }
    }
}
