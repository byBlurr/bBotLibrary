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
            // Check if the directory exists, create if not
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

            // Check if the file exists and return result
            return File.Exists(Path.Combine(directory, filename));
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

        /// Send a log message to the console
        public static Task Logger(LogMessage lmsg)
        {
            // Set the foreground color based on severity
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

            // Send the log
            Console.WriteLine($"{DateTime.Now} [{lmsg.Severity,8}] {lmsg.Source}: {lmsg.Message}");

            // Reset the foreground color
            Console.ForegroundColor = cc;

            // Task completed
            return Task.CompletedTask;
        }
    }
}
