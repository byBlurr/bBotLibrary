using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Net.Bot
{
    public class Util
    {
        /// Convert a string with mentions to a string with readable names
        public static async Task<string> GetReadableMentionsAsync(IGuild guild, string original)
        {
            List<string> splits = original.Split("<@").ToList<string>();
            string result = "";

            foreach (string split in splits)
            {
                if (split.StartsWith("!"))
                {
                    string nsplit = split.Substring(1);
                    string[] splits1 = nsplit.Split(">", 2);
                    ulong id = Convert.ToUInt64(splits1[0]);
                    IGuildUser user = await guild.GetUserAsync(id);

                    result = $"{result}@{user.Username}";
                    for (int i = 1; i < splits1.Length; i++) result = $"{result}{splits1[i]}";
                }
                else
                {
                    result = $"{result}{split}";
                }
            }

            return result;
        }


        /// Get the invite link to add the bot to the server
        public static string GetInviteLink(ulong clientid, int permissions = 8)
        {
            return $"https://discordapp.com/oauth2/authorize?client_id={clientid}&scope=bot&permissions={permissions}";
        }

        /// Get a random emoji from the emojis enum (not all added because im lazy)
        public static string GetRandomEmoji()
        {
            Array values = Enum.GetValues(typeof(Emoji));
            Random random = new Random();
            Emoji randomEmoji = (Emoji)values.GetValue(random.Next(values.Length));

            return EnumUtil.GetString(randomEmoji);
        }

        public static string GetRandomHeartEmoji()
        {
            Array values = Enum.GetValues(typeof(HeartEmoji));
            Random random = new Random();
            HeartEmoji randomEmoji = (HeartEmoji)values.GetValue(random.Next(values.Length));

            return EnumUtil.GetString(randomEmoji);
        }

        /// Returns the strink with an upper case first letter
        public static string ToUppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1);
        }

        /// Send error message
        public static async Task SendErrorAsync(ITextChannel channel, string source, string message, bool printConsole = false)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.Red,
                Title = source,
                Description = message
            };

            if (printConsole) await Logger(new LogMessage(LogSeverity.Error, source, message));
            await channel.SendMessageAsync(null, false, embed.Build());
        }

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
            bool PRINT_VERBOSE = BotConfig.Load().PrintVerbose;

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
                    if (!PRINT_VERBOSE) return Task.CompletedTask;
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
