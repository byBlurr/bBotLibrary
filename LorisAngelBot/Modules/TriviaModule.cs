using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class TriviaModule : ModuleBase
    {
        [Command("trivia")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TriviaAsync(string cat = "")
        {
            await Context.Message.DeleteAsync();

            if (TriviaGames.GetGame(Context.User.Id) == null)
            {
                TriviaCategory category = TriviaCategory.Random;
                if (cat != "")
                {
                    switch (cat.ToLower())
                    {
                        case "general":
                            category = TriviaCategory.General;
                            break;
                        case "sports":
                            category = TriviaCategory.Sports;
                            break;
                        case "sport":
                            category = TriviaCategory.Sports;
                            break;
                        case "motorsports":
                            category = TriviaCategory.Motorsports;
                            break;
                        case "motorsport":
                            category = TriviaCategory.Motorsports;
                            break;
                        case "gaming":
                            category = TriviaCategory.Gaming;
                            break;
                        case "movies":
                            category = TriviaCategory.Movies;
                            break;
                        case "music":
                            category = TriviaCategory.Music;
                            break;
                        case "science":
                            category = TriviaCategory.Science;
                            break;
                        case "art":
                            category = TriviaCategory.Art;
                            break;
                        case "anime":
                            category = TriviaCategory.Anime;
                            break;
                        case "history":
                            category = TriviaCategory.History;
                            break;
                        case "computing":
                            category = TriviaCategory.Computing;
                            break;
                    }
                }

                TriviaFile save = TriviaFile.Load();
                List<TriviaQuestion> questions = new List<TriviaQuestion>();
                switch (category)
                {
                    case TriviaCategory.Random:
                        questions.AddRange(save.General);
                        questions.AddRange(save.Sports);
                        questions.AddRange(save.Motorsports);
                        questions.AddRange(save.Gaming);
                        questions.AddRange(save.Movies);
                        questions.AddRange(save.Music);
                        questions.AddRange(save.Science);
                        questions.AddRange(save.Art);
                        questions.AddRange(save.Anime);
                        questions.AddRange(save.History);
                        questions.AddRange(save.Computing);
                        break;
                    case TriviaCategory.General:
                        questions.AddRange(save.General);
                        break;
                    case TriviaCategory.Sports:
                        questions.AddRange(save.Sports);
                        break;
                    case TriviaCategory.Motorsports:
                        questions.AddRange(save.Motorsports);
                        break;
                    case TriviaCategory.Gaming:
                        questions.AddRange(save.Gaming);
                        break;
                    case TriviaCategory.Movies:
                        questions.AddRange(save.Movies);
                        break;
                    case TriviaCategory.Music:
                        questions.AddRange(save.Music);
                        break;
                    case TriviaCategory.Science:
                        questions.AddRange(save.Science);
                        break;
                    case TriviaCategory.Art:
                        questions.AddRange(save.Art);
                        break;
                    case TriviaCategory.Anime:
                        questions.AddRange(save.Anime);
                        break;
                    case TriviaCategory.History:
                        questions.AddRange(save.History);
                        break;
                    case TriviaCategory.Computing:
                        questions.AddRange(save.Computing);
                        break;
                }

                Random rnd = new Random();
                int q = rnd.Next(0, questions.Count);
                TriviaQuestion question = questions[q];

                List<string> possibleAnswers = question.Options.ToList();
                int n = possibleAnswers.Count;
                while (n > 0)
                {
                    n--;
                    int i = rnd.Next(n + 1);
                    string option = possibleAnswers[i];
                    possibleAnswers[i] = possibleAnswers[n];
                    possibleAnswers[n] = option;
                }

                int a = rnd.Next(0, 4);
                possibleAnswers.Insert(a, question.Answer);

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = $"Trivia - {Context.User.Username}",
                    Description = $"**{question.Question}**\n" +
                    $"**A** {possibleAnswers[0]}\n" +
                    $"**B** {possibleAnswers[1]}\n" +
                    $"**C** {possibleAnswers[2]}\n" +
                    $"**D** {possibleAnswers[3]}",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}"}
                };
                embed.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Category", Value = category.ToString() });
                embed.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Time", Value = $"{question.Seconds} seconds" });
                IUserMessage message = await Context.Channel.SendMessageAsync("BETA - In Testing", false, embed.Build());

                TriviaGame game = new TriviaGame(Context.User.Id, Context.Guild.Id, message, category, question, a);
                TriviaGames.Games.Add(game);

                await Task.Delay(question.Seconds * 1000);

                var check = TriviaGames.GetGame(Context.User.Id);
                if (check != null && check.Message.Id == message.Id)
                {
                    TriviaGames.Games.Remove(game);
                    embed.AddField(new EmbedFieldBuilder() { IsInline = false, Name = "Answer", Value = $"Out of time... (Correct: {question.Answer})" });
                    await message.ModifyAsync(x => x.Embed = embed.Build());

                    TriviaUsers users = TriviaUsers.Load();
                    TriviaUser user = users.GetUser(Context.User.Id);
                    if (user == null)
                    {
                        user = new TriviaUser();
                        user.Name = Context.User.Username;
                        user.Id = Context.User.Id;
                        users.Users.Add(user);
                    }

                    if (!user.Guilds.Contains(Context.Guild.Id)) user.Guilds.Add(Context.Guild.Id);

                    user.AddLoss();
                    users.Save();
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync("You already playing??");
            }
        }

        [Command("score trivia")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TriviaScoreAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null) user = Context.User as IUser;

            TriviaUsers users = TriviaUsers.Load();
            TriviaUser tuser = users.GetUser(user.Id);
            if (tuser == null)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Trivia Score",
                    Description = $"{user.Username} has not played trivia yet.",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Trivia Score",
                    Description = $"{user.Username} has a score of **{tuser.Points}/{tuser.Total}**.\nTheir average time to answer is **{tuser.GetAverageTime()} seconds**.",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("top trivia")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task TriviaTopAsync()
        {
            await Context.Message.DeleteAsync();

            TriviaUsers users = TriviaUsers.Load();
            List<TriviaUser> sortedList = new List<TriviaUser>();

            sortedList = users.Users.OrderByDescending(x => x.Points).ToList();
            string global = $"**Global**";
            for (int i = 0; i < 10 && i < sortedList.Count; i++)
            {
                global = $"{global}\n{sortedList[i].Name} - {sortedList[i].Points}/{sortedList[i].Total} - {sortedList[i].GetAverageTime()} seconds";
            }
            string guild = $"**{Context.Guild.Name}**";
            int count = 0;
            foreach (TriviaUser user in sortedList)
            {
                if (user.Guilds.Contains(Context.Guild.Id) && count < 10)
                {
                    guild = $"{guild}\n{user.Name} - {user.Points}/{user.Total} - {user.GetAverageTime()} seconds";
                    count++;
                }
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Trivia Leaderboard",
                Description = guild + "\n\n" + global,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

    }

    public class TriviaGames
    {
        public static List<TriviaGame> Games = new List<TriviaGame>();

        public static TriviaGame GetGame(ulong id)
        {
            foreach (TriviaGame game in Games)
            {
                if (game.UserId == id)
                {
                    return game;
                }
            }
            return null;
        }

        public static async Task TriviaAnswerAsync(ulong id, char answer)
        {
            TriviaGame game = GetGame(id);
            if (game != null)
            {
                int a;
                switch (answer)
                {
                    case 'a':
                        a = 0;
                        break;
                    case 'b':
                        a = 1;
                        break;
                    case 'c':
                        a = 2;
                        break;
                    case 'd':
                        a = 3;
                        break;
                    default:
                        return;
                }

                Games.Remove(game);
                TriviaUsers users = TriviaUsers.Load();
                TriviaUser user = users.GetUser(game.UserId);
                if (user == null)
                {
                    user = new TriviaUser();
                    IUser newUser = CommandHandler.GetBot().GetUser(game.UserId);
                    user.Name = newUser.Username;
                    user.Id = newUser.Id;
                    users.Users.Add(user);
                }
                if (!user.Guilds.Contains(game.Guild)) user.Guilds.Add(game.Guild);

                if (game.Answer == a)
                {
                    // Correct
                    EmbedBuilder embed = new EmbedBuilder();
                    foreach (Embed e in game.Message.Embeds)
                    {
                        embed.Title = e.Title;
                        embed.Description = e.Description;
                        embed.Color = e.Color;
                        embed.Footer = new EmbedFooterBuilder() { Text = e.Footer.Value.Text };
                    }
                    embed.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Category", Value = game.Category.ToString() });
                    embed.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Time", Value = $"{game.Question.Seconds} seconds" });
                    embed.AddField(new EmbedFieldBuilder() { IsInline = false, Name = "Answer", Value = $"You were correct! (Answered: {game.Question.Answer})" });
                    await game.Message.ModifyAsync(x => x.Embed = embed.Build());

                    TimeSpan timeSpan = DateTime.UtcNow - game.Message.CreatedAt.UtcDateTime;
                    user.AddWin((int)timeSpan.TotalSeconds);
                }
                else
                {
                    // Wrong
                    EmbedBuilder embed = new EmbedBuilder();
                    foreach (Embed e in game.Message.Embeds)
                    {
                        embed.Title = e.Title;
                        embed.Description = e.Description;
                        embed.Color = e.Color;
                        embed.Footer = new EmbedFooterBuilder() { Text = e.Footer.Value.Text };
                    }
                    embed.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Category", Value = game.Category.ToString() });
                    embed.AddField(new EmbedFieldBuilder() { IsInline = true, Name = "Time", Value = $"{game.Question.Seconds} seconds" });
                    embed.AddField(new EmbedFieldBuilder() { IsInline = false, Name = "Answer", Value = $"You were wrong... (Correct: {game.Question.Answer})" });
                    await game.Message.ModifyAsync(x => x.Embed = embed.Build());
                    user.AddLoss();
                }
                users.Save();
            }
        }
    }

    public class TriviaGame
    {
        public ulong UserId { get; set; }
        public ulong Guild { get; set; }
        public IUserMessage Message { get; set; }
        public TriviaCategory Category { get; set; }
        public TriviaQuestion Question { get; set; }
        public int Answer { get; set; }

        public TriviaGame(ulong id, ulong guild, IUserMessage message, TriviaCategory category, TriviaQuestion question, int answer)
        {
            UserId = id;
            Guild = guild;
            Message = message;
            Category = category;
            Question = question;
            Answer = answer;
        }
    }

    public enum TriviaCategory
    {
        Random, General, Sports, Motorsports, Gaming, Movies, Music, Science, Art, Anime, History, Computing
    }

    public class TriviaFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "trivia");

        [JsonIgnore]
        static readonly string Filename = "questions.json";

        public List<TriviaQuestion> General { get; set; }
        public List<TriviaQuestion> Sports { get; set; }
        public List<TriviaQuestion> Motorsports { get; set; }
        public List<TriviaQuestion> Gaming { get; set; }
        public List<TriviaQuestion> Movies { get; set; }
        public List<TriviaQuestion> Music { get; set; }
        public List<TriviaQuestion> Science { get; set; }
        public List<TriviaQuestion> Art { get; set; }
        public List<TriviaQuestion> Anime { get; set; }
        public List<TriviaQuestion> History { get; set; }
        public List<TriviaQuestion> Computing { get; set; }

        public TriviaFile()
        {
            General = new List<TriviaQuestion>();
            Sports = new List<TriviaQuestion>();
            Motorsports = new List<TriviaQuestion>();
            Gaming = new List<TriviaQuestion>();
            Movies = new List<TriviaQuestion>();
            Music = new List<TriviaQuestion>();
            Science = new List<TriviaQuestion>();
            Art = new List<TriviaQuestion>();
            Anime = new List<TriviaQuestion>();
            History = new List<TriviaQuestion>();
            Computing = new List<TriviaQuestion>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                TriviaFile file = new TriviaFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static TriviaFile Load()
        {
            return JsonConvert.DeserializeObject<TriviaFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class TriviaQuestion
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string[] Options { get; set; }
        public int Seconds { get; set; }

        public TriviaQuestion()
        {
            Question = "?";
            Answer = ".";
            Options = new string[3];
        }
    }

    public class TriviaUsers
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "trivia");

        [JsonIgnore]
        static readonly string Filename = "users.json";

        public List<TriviaUser> Users { get; set; }

        public TriviaUsers()
        {
            Users = new List<TriviaUser>();
        }

        public TriviaUser GetUser(ulong id)
        {
            foreach (TriviaUser user in Users)
            {
                if (user.Id == id)
                {
                    return user;
                }
            }

            return null;
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                TriviaUsers file = new TriviaUsers();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static TriviaUsers Load()
        {
            return JsonConvert.DeserializeObject<TriviaUsers>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class TriviaUser
    {
        public string Name { get; set; }
        public List<ulong> Guilds { get; set; }
        public ulong Id { get; set; }
        public int Points { get; set; }
        public int Total { get; set; }
        public List<int> Times { get; set; }

        public TriviaUser()
        {
            Name = "";
            Guilds = new List<ulong>();
            Id = 0L;
            Points = 0;
            Total = 0;
            Times = new List<int>();
        }

        public void AddLoss()
        {
            Total++;
        }

        public void AddWin(int time)
        {
            Total++;
            Points++;
            Times.Add(time);
            if (Times.Count > 100) Times.RemoveAt(0);
        }

        public int GetAverageTime()
        {
            int games = 0;
            int total = 0;
            int average = 0;

            foreach (int time in Times)
            {
                if (time > 0)
                {
                    total += time;
                    games++;
                }
            }

            if (total > 0) average = total / games;
            return average;
        }
    }
}
