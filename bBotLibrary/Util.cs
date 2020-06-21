using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Bot
{
    public class Util
    {

        /// Checks if the file exists, creates the directory if it doesn't exist.
        public static bool DoesFileExist(string directory, string filename)
        {
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            return File.Exists(Path.Combine(directory, filename));
        }

        /// Post a message to a channel, automatically deletes after the time in milliseconds.
        public static async Task SendTemporaryMessageAsync(ITextChannel channel, string text = null, bool isTTS = false, Embed embed = null, RequestOptions options = null, int time = 5000)
        {
            var message_thread = Task.Run(async () => { 
                var msg = await channel.SendMessageAsync(text, isTTS, embed, options);
                await Task.Delay(time);
                await msg.DeleteAsync();
            });
        }
    }
}
