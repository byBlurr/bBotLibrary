using Discord.Net.Bot.CommandModules;
using System;

namespace Discord.Net.Bot.Database.Configs
{
    public class BotCommand
    {
        public string Name { get; set; }
        public CommandUsage[] Usage { get; set; }
        public string Description { get; set; }
        public CommandCategory Category { get; set; }
        public string ExtraInfo { get; set; }
        public bool New { get; set; }

        public BotCommand(string name, CommandUsage[] usage, string description, CommandCategory category, string extra = "", bool newCommand = false)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Usage = usage ?? throw new ArgumentNullException(nameof(usage));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Category = category;
            ExtraInfo = extra;
            New = newCommand;
        }
    }
}
