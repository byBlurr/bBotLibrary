using Discord.Commands;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Discord.Net.Bot.CommandModules
{
    public class ModuleHelp
    {
        // TODO: UPDATE TO USE NEW USAGE PARAMS
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
                    if (CommandHelpText.Length > 0) CommandHelpText = CommandHelpText + ", " + StringUtil.ToUppercaseFirst(command.Handle);
                    else CommandHelpText = StringUtil.ToUppercaseFirst(command.Handle);
                }

                if (command.Handle.ToLower() == section.ToLower())
                {
                    string help = $"**{command.Handle.ToUpper()}**\nUsage:\n{command.Usage}\n{command.Description}";
                    if (command.New) help = $"**NEW** - {help}";
                    if (command.ExtraInfo != "") help = $"{help}\nFeature requested by: {command.ExtraInfo}";

                    CommandHelpText = CommandHelpText + "\n\n" + help;
                    isSpecificCommand = true;
                }

                if (!Categories.Contains(command.Category))
                {
                    Categories.Add(command.Category);
                    CategoryText = $"{CategoryText}, {StringUtil.ToUppercaseFirst(command.Category.ToString())}";
                }
                if (command.New) hasNewCommands = true;
            }
            CategoryText = CategoryText.Split(",", 2)[1];
            if (hasNewCommands) CategoryText = $"{CategoryText}, New";

            if (CommandHelpText.Length <= 0)
            {
                CommandHelpText = $"Command Usage: {Prefix}help <category>\n\nCategories:\n{CategoryText}\n\nKnown Errors:\nerror 50007 - Users privacy settings prevent dm's\nerror 403 - Bot does not have required permissions\nOther errors are tracked by the bot, and will be looked into.";
            }
            else if (!isSpecificCommand) CommandHelpText = $"Command Usage: {Prefix}help <command>\n\n{StringUtil.ToUppercaseFirst(section)} Commands:\n{CommandHelpText}";

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Bot Command Help",
                Description = CommandHelpText,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{EmojiUtil.GetRandomEmoji()}  Bot Prefix: {Prefix}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
            
        }
    }

    public class CommandUsage
    {
        public string Handle { get; set; }
        public List<CommandArgument> Arguments { get; set; }

        public CommandUsage(string handle, List<CommandArgument> arguments)
        {
            Handle = handle;
            Arguments = arguments;
        }
        public CommandUsage(string handle)
        {
            Handle = handle;
            Arguments = new List<CommandArgument>(0);
        }

        public override string ToString()
        {
            string helptext;

            helptext = Handle;
            foreach (CommandArgument arg in Arguments)
            {
                helptext += $" {arg.ToString()}";
            }

            return helptext;
        }

        public string ToExample()
        {
            string helptext;

            helptext = Handle;
            foreach (CommandArgument arg in Arguments)
            {
                helptext += $" {arg.ToExample()}";
            }

            return helptext;
        }
    }

    public class CommandArgument
    {
        public CommandArgumentType ArgumentType { get; set; }
        public bool Optional { get; set; }
        public string Example { get; set; }

        public CommandArgument(CommandArgumentType type, bool optional, string example = null)
        {
            ArgumentType = type;
            Optional = optional;
            Example = example ?? string.Empty;
        }

        public override string ToString()
        {
            string display;

            if (Optional) display = "[";
            else display = "<";

            display += StringUtil.ToUppercaseFirst(ArgumentType.ToString());

            if (Optional) display += "]";
            else display += ">";

            return display;
        }

        public string ToExample()
        {
            string display;

            if (Example != string.Empty) display = Example;
            else
            {
                switch (ArgumentType)
                {
                    case CommandArgumentType.USER:
                        display = "@Blurr";
                        break;
                    case CommandArgumentType.ID:
                        display = "729696788097007717";
                        break;
                    case CommandArgumentType.TEXT:
                        display = "The frog jumped over the pond.";
                        break;
                    case CommandArgumentType.NUMBER:
                        display = "18";
                        break;
                    case CommandArgumentType.TOGGLE:
                        display = "true";
                        break;
                    default:
                        display = "Unknown Type";
                        break;
                }
            }

            return display;
        }
    }

    public enum CommandArgumentType
    {
        USER, ID, TEXT, NUMBER, TOGGLE
    }
}
