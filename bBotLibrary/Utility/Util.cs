using Discord.Net.Bot.Database.Configs;
using System;
using System.IO;
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

        /// Send a log message to the console
        public static Task LoggerAsync(LogMessage lmsg)
        {
            bool PRINT_VERBOSE = BotConfig.Load().PrintVerbose;

            // These unknown dispatches are known... they spam the console on bots in a high number of servers.
            if (!PRINT_VERBOSE && lmsg.Message.Contains("Unknown Dispatch") && (lmsg.Message.Contains("INTEGRATION") || lmsg.Message.Contains("INVITE"))) return Task.CompletedTask;

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
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogSeverity.Verbose:
                    if (!PRINT_VERBOSE) return Task.CompletedTask;
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
            }

            // Send the log
            Console.WriteLine($"{DateTime.Now,22} [{lmsg.Severity,8}] {lmsg.Source,10}: {lmsg.Message}");

            // Reset the foreground color
            Console.ForegroundColor = cc;

            // Task completed
            return Task.CompletedTask;
        }
    }
}
