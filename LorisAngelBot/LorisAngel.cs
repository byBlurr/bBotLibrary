﻿using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using Discord.Net.Bot.Modules;
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

            // Games
            commands.Add(new BotCommand("BLACKJACK", "`-blackjack`", "Open a game of blackjack.", CommandCategory.Main, "Libby", true));
            commands.Add(new BotCommand("SHIP", "`-ship @user1 @user2`, `-ship name1 name2`, `-ship user2`, `-ship name2`", "Test how strong your relationship is!", CommandCategory.Games, "Jimmy"));
            commands.Add(new BotCommand("REVERSE", "`-reverse <message>`", "Reverse the message!", CommandCategory.Games, ""));
            commands.Add(new BotCommand("8BALL", "`-8ball <question>`", "Ask the bot a question!", CommandCategory.Games, "Starri"));
            commands.Add(new BotCommand("DICE", "`-dice <amount>`", "Roll a dice or 2 or 50!", CommandCategory.Games, "Mir"));
            commands.Add(new BotCommand("SNAKE", "`-snake @player2 @player3 @player4`", "Start a game of snake! Up to 4 players! [BETA]", CommandCategory.Main, "Tay", true));
            commands.Add(new BotCommand("WHO", "`-who <question>` (Mention people in the question)", "Ask a question, the bot will select a random user that is mentioned!", CommandCategory.Games, ""));
            commands.Add(new BotCommand("TICTACTOE | NAUGHTS & CROSSES", "`-ttt`", "Start a game of Tic Tac Toe (Naughts and Crosses)", CommandCategory.Main, "Libby", true));


            // NSFW
            commands.Add(new BotCommand("PUNISH", "`-punish @user`", "Punish them for their actions!", CommandCategory.NSFW, "Jimmy, Ras"));

            // User
            commands.Add(new BotCommand("OLDEST", "`-oldest`", "Find out who the oldest user in the guild is!", CommandCategory.User, ""));
            commands.Add(new BotCommand("AVATAR", "`-avatar @user`", "View the users avatar!", CommandCategory.User, "Libby"));
            commands.Add(new BotCommand("WHOIS", "`-whois @user`", "View details about the user!", CommandCategory.User, "Libby"));

            // BotRelated
            commands.Add(new BotCommand("INVITE", "`-invite`, `inv`", "Get the invite link to invite Loris Angel to your server!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("REQUEST", "`-request`", "Get the invite link to the support server where you can request new features or get help!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("PREFIX", "`-prefix <prefix>`", "Change the prefix to the bot for your server!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("UPTIME", "`-uptime`", "Check how long the bot has been online.", CommandCategory.BotRelated, ""));

            // Tools
            commands.Add(new BotCommand("BINARY", "`-binary <text>`", "Convert text to binary!", CommandCategory.Tools, ""));
            commands.Add(new BotCommand("USERS", "`-users`", "Will tell you how many members are in this guild!", CommandCategory.Tools, ""));
            commands.Add(new BotCommand("OLDEST", "`-oldest`", "Will check which user in the guild has the oldest account!", CommandCategory.Tools, ""));
        }

        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
            //bot.ReactionAdded += ReactionModule.ReactionAddedAsync;
            //bot.ReactionRemoved += ReactionModule.ReactionRemovedAsync;
            //bot.ReactionsCleared += ReactionModule.ReactionsClearedAsync;
        }

        private async Task ReadyAsync()
        {
            RelationshipFile.Exists();

            await bot.SetStatusAsync(UserStatus.Online);

            string stream = "https://www.twitch.tv/pengu";

            var status = Task.Run(async () => {
                while (true)
                {
                    await bot.SetGameAsync($"to {bot.Guilds.Count} servers {Util.GetRandomHeartEmoji()}", stream, ActivityType.Streaming);
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
