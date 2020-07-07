using System;

namespace Discord.Net.Bot.Database.Configs
{
    public class BotCommand
    {
        public string Handle { get; set; }
        public string Usage { get; set; }
        public string Description { get; set; }
        public CommandCategory Category { get; set; }
        public string Request { get; set; }

        public BotCommand(string handle, string usage, string description, CommandCategory category, string request)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            Usage = usage ?? throw new ArgumentNullException(nameof(usage));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Category = category;
            Request = request;
        }
    }
}
