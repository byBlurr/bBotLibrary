using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Text;

namespace LorisAngelBot.Modules
{
    public class QuizModule : ModuleBase
    {

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
        General, Sports, ExtremeSports, Anime, Gaming, Computing
    }
}
