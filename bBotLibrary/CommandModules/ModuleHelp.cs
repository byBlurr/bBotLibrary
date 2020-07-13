using Discord.Commands;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Net.Bot.CommandModules
{
    public class ModuleHelp
    {
        public static async Task HelpAsync(ICommandContext Context, string section = "")
        {
            await Context.Message.DeleteAsync();
            BotConfig conf = BotConfig.Load();

            string CommandHelpText = "";
            bool SendInDm = false;
            List<CommandCategory> Categories = new List<CommandCategory>();
            bool hasNewCommands = false;
            string CategoryText = "";
            string Prefix = CommandHandler.GetPrefix(Context.Guild.Id);

            foreach (BotCommand command in conf.Commands)
            {
                if (section.ToLower() != "new")
                {
                    if (command.Category.ToString().ToLower() == section.ToLower() || command.Handle.ToLower() == section.ToLower())
                    {
                        string help = $"**{command.Handle}**\nUsage:\n{command.Usage}\n{command.Description}";
                        if (command.New) help = $"**NEW** - {help}";
                        if (command.ExtraInfo != "") help = $"{help}\nFeature requested by: {command.ExtraInfo}";

                        CommandHelpText = CommandHelpText + "\n\n" + help;
                        SendInDm = true;
                    }
                }
                else
                {
                    if (command.New)
                    {
                        string help = $"**NEW** - **{command.Handle}**\nUsage:\n{command.Usage}\n{command.Description}";
                        if (command.ExtraInfo != "") help = $"{help}\nFeature requested by: {command.ExtraInfo}";

                        CommandHelpText = CommandHelpText + "\n\n" + help;
                        SendInDm = true;
                    }
                }

                if (!Categories.Contains(command.Category))
                {
                    Categories.Add(command.Category);
                    CategoryText = $"{CategoryText}, {Util.ToUppercaseFirst(command.Category.ToString())}";
                }
                if (command.New) hasNewCommands = true;
            }
            CategoryText = CategoryText.Split(",", 2)[1];
            if (hasNewCommands) CategoryText = $"{CategoryText}, New";

            if (CommandHelpText.Length <= 0)
            {
                CommandHelpText = "No commands found in this category\n\n" +
                    $"Command Usage: {Prefix}help <category>\n\n" +
                    "Categories:\n" +
                    CategoryText;

                SendInDm = false;
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Bot Command Help",
                Description = CommandHelpText,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Bot Prefix: {Prefix}" }
            };

            if (SendInDm)
            {
                await Context.User.SendMessageAsync(null, false, embed.Build());
                await Context.Channel.SendMessageAsync($"Help for `{section.ToLower()}` has been sent to you {Context.User.Mention}!", false);
            }
            else
            {
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }
    }
}
