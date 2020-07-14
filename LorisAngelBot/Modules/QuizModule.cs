using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Text;
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

    }

    public class QuizQuestion
    {
        public string Question { get; set; }
        public string Answer { get; set; }
        public string[] Options { get; set; }
        public QuizCategory Category { get; set; }

        public QuizQuestion(string question, string answer, string[] options, QuizCategory category)
        {
            Question = Util.ToUppercaseFirst(question);
            Answer = Util.ToUppercaseFirst(answer);
            Options = options;
            Category = category;
        }
    }

    public enum QuizCategory
    {
        Unspecified, General, Sports, ExtremeSports, Anime, Gaming, Computing, Music
    }
}
