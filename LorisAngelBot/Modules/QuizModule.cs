using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class QuizModule : ModuleBase
    {
        [Command("quiz")]
        private async Task QuizAsync(string topic = "")
        {
            await Context.Message.DeleteAsync();

            if (QuizGames.GetGame(Context.Guild.Id) == null)
            {
                QuizCategory pickedTopic = QuizCategory.Unspecified;
                Enum.TryParse(Util.ToUppercaseFirst(topic), out pickedTopic);

                if (pickedTopic != QuizCategory.Unspecified)
                {
                    // Start the quiz
                    QuizGame quiz = new QuizGame();
                    List<QuizQuestion> questions = QuizFile.Load().GetTopicQuestions(pickedTopic);
                    IUserMessage message = null;

                    for (int i = 0; i < questions.Count; i++)
                    {
                        bool endGame = false;
                        if (!endGame)
                        {
                            if (message != null) await message.DeleteAsync();
                            if (questions[i] != null)
                            {
                                EmbedBuilder embed = new EmbedBuilder()
                                {
                                    Title = $"Quiz {Util.ToUppercaseFirst(pickedTopic.ToString())} -  Question {i + 1}",
                                    Description = $"**Q: {Util.ToUppercaseFirst(questions[i].Question)}**\n\n" +
                                    $"**A**  {questions[i].Options[0]}\n" +
                                    $"**B**  {questions[i].Options[1]}\n" +
                                    $"**C**  {questions[i].Options[2]}\n" +
                                    $"**D**  {questions[i].Options[3]}\n\n" +
                                    $"Time: 30 seconds",
                                    Color = Color.DarkPurple,
                                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                                };
                                message = await Context.Channel.SendMessageAsync(null, false, embed.Build());


                                IEmote[] reactions = {
                                    new Discord.Emoji("🇦"),
                                    new Discord.Emoji("🇧"),
                                    new Discord.Emoji("🇨"),
                                    new Discord.Emoji("🇩"),
                                    new Discord.Emoji("❌"),
                                };

                                await message.AddReactionsAsync(reactions);
                                await Task.Delay(10000);

                                // See who was correct
                                

                                await message.RemoveAllReactionsAsync();
                                embed.Description = $"Answer was **{questions[i].Options[questions[i].Answer]}**\n\nCorrect:\n**{"Not sure who got it correct because I am dumb!"}**";
                                await message.ModifyAsync(x => x.Embed = embed.Build());

                                if (endGame) i = questions.Count + 1;
                                await Task.Delay(10000);
                            }
                        }
                    }

                    if (message != null) await message.DeleteAsync();

                    EmbedBuilder finishedEmbed = new EmbedBuilder()
                    {
                        Title = "Quiz Finished",
                        Description = "The quiz has ended!",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                    };

                    await Context.Channel.SendMessageAsync(null, false, finishedEmbed.Build());
                }
                else
                {
                    // Display available topics
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "Quiz Help",
                        Description = $"Choose a topic for your quiz using the following command...\n`{CommandHandler.GetPrefix(Context.Guild.Id)}quiz <topic>\n\n" +
                        $"**Topics**\nGeneral, Sports, ExtremeSports, Anime, Gaming, Computing, Music",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());
                }
            }
            else
            {
                // Already game active!
                // Display available topics
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Quiz Help",
                    Description = $"There is already a quiz in motion.",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

    }

    public class QuizGames
    {
        public static List<QuizGame> Games = new List<QuizGame>();

        public static QuizGame GetGame(ulong guild)
        {
            foreach (QuizGame game in Games)
            { 
                if (game.Guild == guild)
                {
                    return game;
                }
            }
            return null;
        }
    }
    public class QuizGame
    {
        public ulong Guild { get; set; }
    }

    public class QuizFile
    {

        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "quiz");

        [JsonIgnore]
        static readonly string Filename = "questions.json";

        public List<QuizQuestion> Questions { get; set; }

        public Discord.Emoji C { get; set; }

        public QuizFile()
        {
            Questions = new List<QuizQuestion>();
        }

        public List<QuizQuestion> GetTopicQuestions(QuizCategory topic)
        {
            List<QuizQuestion> questions = new List<QuizQuestion>();
            foreach (QuizQuestion q in Questions)
            {
                if (q.Category == topic) questions.Add(q);
            }

            return questions;
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                new QuizFile().Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static QuizFile Load()
        {
            return JsonConvert.DeserializeObject<QuizFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public int Answer { get; set; }
        public string[] Options { get; set; }
        public QuizCategory Category { get; set; }

        public QuizQuestion(string question, int answer, string[] options, QuizCategory category)
        {
            Question = Util.ToUppercaseFirst(question);
            Answer = answer;
            Options = options;
            Category = category;
        }
    }

    public enum QuizCategory
    {
        Unspecified, General, Sports, ExtremeSports, Anime, Gaming, Computing, Music
    }
}
