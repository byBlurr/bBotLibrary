using Discord;
using Discord.Net.Bot;
using Discord.Net.Bot.CommandModules;
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

            // Games
            commands.Add(new BotCommand("TRIVIA", "`-trivia` or `-trivia <category>`", "Request a trivia question!", CommandCategory.Games, "Siena, Libby"));
            commands.Add(new BotCommand("TICTACTOE", "`-ttt`", "Start a game of Tic Tac Toe (Naughts and Crosses)", CommandCategory.Games, "Libby"));
            commands.Add(new BotCommand("BLACKJACK", "`-blackjack`", "Open a game of blackjack.", CommandCategory.Games, "Libby", true));
            commands.Add(new BotCommand("SNAKE", "`-snake @player2 @player3 @player4`", "Start a game of snake! Up to 4 players! [BETA]", CommandCategory.Games, "Tay"));
            
            // Fun
            commands.Add(new BotCommand("QUOTE", "`-quote @user <message>`", "Create a fake quote (Use shift enter for new lines, will look like multiple messages)!", CommandCategory.Fun, ""));
            commands.Add(new BotCommand("CRACK", "`-crack`", "Retrieve a hashed password to crack!", CommandCategory.Fun, ""));
            commands.Add(new BotCommand("SHIP", "`-ship @user1 @user2`, `-ship name1 name2`, `-ship user2`, `-ship name2`", "Test how strong your relationship is!", CommandCategory.Fun, "Jimmy"));
            commands.Add(new BotCommand("8BALL", "`-8ball <question>`", "Ask the bot a question!", CommandCategory.Fun, "Siena"));
            commands.Add(new BotCommand("DICE", "`-dice <amount>`", "Roll a dice or 2 or 50!", CommandCategory.Fun, "Mir"));
            commands.Add(new BotCommand("WHO", "`-who <question>` (Mention people in the question)", "Ask a question, the bot will select a random user that is mentioned!", CommandCategory.Fun, ""));
            commands.Add(new BotCommand("KILL", "`-kill @user`", "Kill the user!", CommandCategory.Fun, ""));
            commands.Add(new BotCommand("ROAST", "`-roast @user`", "Roast the user!", CommandCategory.Fun, ""));
            commands.Add(new BotCommand("COMPLIMENTS", "`-compliment @user`", "Compliment the user!", CommandCategory.Fun, ""));
            commands.Add(new BotCommand("EPICRATING", "`-epic @user` or `-rate @user`", "See just how epic they are!", CommandCategory.Fun, "Libby"));
            commands.Add(new BotCommand("PUNISH", "`-punish @user`", "Punish them for their actions!", CommandCategory.Fun, "Jimmy, Ras"));

            // Currency
            commands.Add(new BotCommand("BANK", "`-bank`", "View how much money you have in the bank!", CommandCategory.Currency, "Libby", true));
            commands.Add(new BotCommand("GIVE", "`-give @user <amount>`", "give another user some of your money!", CommandCategory.Currency, "Libby", true));
            commands.Add(new BotCommand("DAILY", "`-daily`", "Claim your daily $50!", CommandCategory.Currency, "Libby", true));
            commands.Add(new BotCommand("RICHEST", "`-richest`", "See who has the most money!", CommandCategory.Currency, "Libby", true));

            // Leaderboards
            commands.Add(new BotCommand("SCORE TRIVIA", "`-score trivia` or `-score trivia @user`", "Check the users score on trivia.", CommandCategory.Leaderboards));
            commands.Add(new BotCommand("TOP TRIVIA", "`-top trivia`", "Check the users with the highest scores on trivia.", CommandCategory.Leaderboards));

            // User
            commands.Add(new BotCommand("OLDEST", "`-oldest`", "Find out who the oldest user in the guild is!", CommandCategory.User, ""));
            commands.Add(new BotCommand("AVATAR", "`-avatar @user`", "View the users avatar!", CommandCategory.User, "Libby"));
            commands.Add(new BotCommand("WHOIS", "`-whois @user`", "View details about the user!", CommandCategory.User, "Libby"));

            // BotRelated
            commands.Add(new BotCommand("DONATE", "`-donate`, `-beer`, `-buybeer`", "Buy me a beer!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("INVITE", "`-invite`, `inv`", "Get the invite link to invite Loris Angel to your server!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("REQUEST", "`-request`", "Get the invite link to the support server where you can request new features or get help!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("PREFIX", "`-prefix <prefix>`", "Change the prefix to the bot for your server!", CommandCategory.BotRelated, ""));
            commands.Add(new BotCommand("UPTIME", "`-uptime`", "Check how long the bot has been online.", CommandCategory.BotRelated, ""));

            // Tools
            commands.Add(new BotCommand("BINARY", "`-binary <text>`", "Convert text to binary!", CommandCategory.Tools, ""));
            commands.Add(new BotCommand("USERS", "`-users`", "Will tell you how many members are in this guild!", CommandCategory.Tools, ""));
            commands.Add(new BotCommand("OLDEST", "`-oldest`", "Will check which user in the guild has the oldest account!", CommandCategory.Tools, ""));
            commands.Add(new BotCommand("REVERSE", "`-reverse <message>`", "Reverse the message!", CommandCategory.Tools, ""));

            // Moderation
            commands.Add(new BotCommand("KICK", "`-kick @user <reason>`", "Kick the member from the guild!", CommandCategory.Moderation, ""));
            commands.Add(new BotCommand("BAN", "`-ban @user <reason>`", "Ban the member from the guild!", CommandCategory.Moderation, ""));
            commands.Add(new BotCommand("MUTE", "`-mute @user <reason>`", "Mute the member!", CommandCategory.Moderation, ""));
            commands.Add(new BotCommand("UNMUTE", "`-unmute @user`", "Unmute the member!", CommandCategory.Moderation, ""));
            commands.Add(new BotCommand("CREATEMUTE", "`-createmute`", "Create the mute role!", CommandCategory.Moderation, ""));
        }

        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
            bot.MessageReceived += MessageReceivedAsync;
            bot.MessageReceived += ModuleModerator.FilterMutedAsync;
            bot.JoinedGuild += JoinedGuildAsync;
            bot.LeftGuild += LeftGuildAsync;
        }

        public async Task MessageReceivedAsync(SocketMessage msg)
        {
            if (msg.Author.IsBot) return;

            if (TriviaGames.GetGame(msg.Author.Id) != null)
            {
                char answer = msg.Content.ToLower()[0];
                var thread = Task.Run(async () => { await TriviaGames.TriviaAnswerAsync(msg.Author.Id, answer); });
            }
            else if (CrackGames.GetGame(msg.Author.Id) != null)
            {
                var thread = Task.Run(async () => { await CrackGames.CheckPasswordAsync(msg.Author.Id, msg.Content); });
            }
        }

        private async Task ReadyAsync()
        {
            DonateFile.Exists();
            BankFile.Exists();
            RelationshipFile.Exists();
            TriviaFile.Exists();
            TriviaUsers.Exists();
            DeathsFile.Exists();
            RoastsFile.Exists();
            ComplimentsFile.Exists();
            PunishFile.Exists();
            CrackFile.Exists();

            await bot.SetStatusAsync(UserStatus.Online);

            string stream = "https://www.twitch.tv/pengu";

            var status = Task.Run(async () => {
                int i = 0;
                while (true)
                {
                    if (i == 0)
                    {
                        await bot.SetGameAsync($"to {bot.Guilds.Count} servers {Util.GetRandomHeartEmoji()}", stream, ActivityType.Streaming);
                        i++;
                    }
                    else if (i == 1)
                    {
                        await bot.SetGameAsync($"try -help {Util.GetRandomHeartEmoji()}", stream, ActivityType.Streaming);
                        i++;
                    }
                    else if (i == 2)
                    {
                        await bot.SetGameAsync($"try -donate {Util.GetRandomHeartEmoji()}", stream, ActivityType.Streaming);
                        i++;
                    }
                    else
                    {
                        Random rnd = new Random();
                        BotConfig conf = BotConfig.Load();
                        int j = rnd.Next(0, conf.Commands.Count);

                        await bot.SetGameAsync($"try -{conf.Commands[j].Handle.ToLower()} {Util.GetRandomHeartEmoji()}", stream, ActivityType.Streaming);
                        i = 0;
                    }
                    await Task.Delay(15000);
                }
            });
        }

        private async Task JoinedGuildAsync(SocketGuild guild)
        {
            var sg = bot.GetGuild(730573219374825523);
            await sg.GetTextChannel(739308321655226469).SendMessageAsync("Joined guild " + guild.Name);
        }

        private async Task LeftGuildAsync(SocketGuild guild)
        {
            var sg = bot.GetGuild(730573219374825523);
            await sg.GetTextChannel(739308321655226469).SendMessageAsync("Left guild " + guild.Name);
        }
    }


    class LorisAngel : Bot
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new LCommandHandler();
            handler.RestartEveryMs = 21600000; // Every 6 hours
            StartBot(handler);
        }
    }
}
