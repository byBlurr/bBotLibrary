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
            List<CommandCategory> Categories = new List<CommandCategory>();
            bool hasNewCommands = false;
            bool isSpecificCommand = false;
            string CategoryText = "";
            string Prefix = CommandHandler.GetPrefix(Context.Guild.Id);

            foreach (BotCommand command in conf.Commands)
            {
                if (command.Category.ToString().ToLower() == section.ToLower() || (section.ToLower() == "new" && command.New))
                {
                    if (CommandHelpText.Length > 0) CommandHelpText = CommandHelpText + ", " + Util.ToUppercaseFirst(command.Handle);
                    else CommandHelpText = Util.ToUppercaseFirst(command.Handle);
                }

                if (command.Handle.ToLower() == section.ToLower())
                {
                    string help = $"**{command.Handle}**\nUsage:\n{command.Usage}\n{command.Description}";
                    if (command.New) help = $"**NEW** - {help}";
                    if (command.ExtraInfo != "") help = $"{help}\nFeature requested by: {command.ExtraInfo}";

                    CommandHelpText = CommandHelpText + "\n\n" + help;
                    isSpecificCommand = true;
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
                CommandHelpText = $"Command Usage: {Prefix}help <category>\n\nCategories:\n{CategoryText}\n\nKnown Errors:\nerror 50007 - Users privacy settings prevent dm's\nerror 403 - Bot does not have required permissions\nOther errors are tracked by the bot, and will be looked into.";
            }
            else if (!isSpecificCommand) CommandHelpText = $"Command Usage: {Prefix}help <command>\n\n{Util.ToUppercaseFirst(section)} Commands:\n{CommandHelpText}";

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Bot Command Help",
                Description = CommandHelpText,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Bot Prefix: {Prefix}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
            
        }
    }
}
