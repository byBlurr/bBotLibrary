using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using Discord.WebSocket;
using LorisAngelBot.Modules;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LorisAngelBot
{
    class LCommandHandler : CommandHandler
    {
        public override void RegisterCommands(List<BotCommand> commands)
        {
            commands.Clear();
            commands.Add(new BotCommand("SHIP", "`-ship @user1 @user2`, `-ship name1 name2`, `-ship user2`, `-ship name2`", "Test how strong your relationship is!", CommandCategory.Games, "Jimmy"));
            commands.Add(new BotCommand("PUNISH", "`-punish @user`", "Punish them for their actions!", CommandCategory.Games, "Jimmy, Ras"));
            commands.Add(new BotCommand("REVERSE", "`-reverse <message>`", "Reverse the message!", CommandCategory.Games, ""));
            commands.Add(new BotCommand("8BALL", "`-8ball <question>`", "Ask the bot a question!", CommandCategory.Games, "Starri"));
            commands.Add(new BotCommand("QUOTE", "`-quote @author <message>`", "Save a quote to the database! (WIP)", CommandCategory.Games, ""));
            commands.Add(new BotCommand("OLDEST", "`-oldest`", "Find out who the oldest user in the guild is!", CommandCategory.User, ""));
            commands.Add(new BotCommand("AVATAR", "`-avatar @user`", "View the users avatar!", CommandCategory.User, "Libby"));
            commands.Add(new BotCommand("WHOIS", "`-whois @user`", "View details about the user!", CommandCategory.User, "Libby"));
            commands.Add(new BotCommand("INVITE", "`-invite`, `inv`", "Get the invite link to invite Loris Angel to your server!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("PREFIX", "`-prefix <prefix>`", "Change the prefix to the bot for your server!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("STREAM", "`-stream <stream-url>`", "Every 24 hours a random stream will be picked and displayed on the status, list of streams resets each week.", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("BINARY", "`-binary <text>`", "Convert text to binary!", CommandCategory.Tools, ""));
        }

        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
        }

        private async Task ReadyAsync()
        {
            Relationships.Exists();
            StreamFile.Exists();

            await bot.SetStatusAsync(UserStatus.Online);

            string stream = "https://www.twitch.tv/pengu";
            StreamFile file = StreamFile.Load();
            if (file.Current != null) stream = file.Current.Url;

            //await bot.SetActivityAsync(new Game($"{bot.Guilds.Count} servers {Util.GetRandomEmoji()}", ActivityType.CustomStatus));
            var status = Task.Run(async () => {
                while (true)
                {
                    if (DateTime.UtcNow.Hour == 0 && DateTime.UtcNow.Minute <= 1)
                    {
                        file = StreamFile.Load();
                        if (file.Current != null) stream = file.Current.Url;
                    }

                    await bot.SetGameAsync($"to {bot.Guilds.Count} servers {Util.GetRandomHeartEmoji()}", stream, ActivityType.Streaming);
                    await Task.Delay(2500);
                }
            });

            var streams = Task.Run(async () => {
                while (true)
                {
                    if (DateTime.UtcNow.Hour == 0)
                    {
                        await StreamModule.PickNewWinnerAsync();
                        if (DateTime.UtcNow.Day == 1)
                        {
                            StreamFile file = StreamFile.Load();
                            file.Streams.Clear();
                            file.Save();
                        }
                        await Task.Delay((60000 * 60) * 20);
                    }
                    await Task.Delay(60000);
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
