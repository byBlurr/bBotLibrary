using System;
using System.Collections.Generic;

namespace Discord.Net.Bot.Database.Configs
{
    public class IndividualConfig
    {
        public ulong Guild { get; set; }
        public string Prefix { get; set; }
        public bool LogCommands { get; set; }
        public bool LogActions { get; set; }
        public ulong LogChannel { get; set; }
        public Boolean Censor { get; set; }
        public List<string> CensoredWords { get; private set; }

        public IndividualConfig()
        {
            Guild = 0L;
            Prefix = "-";
            LogCommands = false;
            LogActions = false;
            LogChannel = 0L;
            Censor = false;
            CensoredWords = new List<string>();
        }

        public void AddCensoredWord(string word)
        {
            if (!CensoredWords.Contains(word))
            {
                CensoredWords.Add(word);
            }
        }
    }
}
