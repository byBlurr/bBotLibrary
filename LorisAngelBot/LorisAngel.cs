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
            commands.Add(new BotCommand("BINARY", "`-binary <text>`", "Convert text to binary!", CommandCategory.Tools, ""));
        }

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
                    await bot.SetGameAsync($"to {bot.Guilds.Count} servers {Util.GetRandomHeartEmoji()}", "https://www.twitch.tv/pengu", ActivityType.Streaming);
                    await Task.Delay(2500);
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
