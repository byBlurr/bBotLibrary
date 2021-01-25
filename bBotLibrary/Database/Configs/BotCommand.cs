using Discord.Net.Bot.CommandModules;
using System;

namespace Discord.Net.Bot.Database.Configs
{
    public class BotCommand
    {
        public string Handle { get; set; }
        public CommandUsage[] Usage { get; set; }
        public string Description { get; set; }
        public CommandCategory Category { get; set; }
        public string ExtraInfo { get; set; }
        public bool New { get; set; }

        public BotCommand(string handle, CommandUsage[] usage, string description, CommandCategory category, string extra = "", bool newCommand = false)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            Usage = usage ?? throw new ArgumentNullException(nameof(usage));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Category = category;
            ExtraInfo = extra;
            New = newCommand;
        }
    }
}
