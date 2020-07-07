using Discord.Commands;
using Discord.Net.Bot.Database.Configs;
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

            foreach (BotCommand command in conf.Commands)
            {
                if (command.Category.ToString().ToLower() == section.ToLower() || command.Handle.ToLower() == section.ToLower())
                {
                    string help = $"{command.Handle}\nUsage:\n{command.Usage}\n{command.Description}";
                    if (command.Request != "") help = $"{help}\nFeature requested by: {command.Request}";

                    CommandHelpText = CommandHelpText + "\n\n" + help;
                    SendInDm = true;
                }
            }

            if (CommandHelpText.Length <= 0)
            {
                CommandHelpText = "No commands found in this category\n\n" +
                    "Command Usage: -help <category>\n\n" +
                    "Categories:\n" +
                    "BotRelated, Games, User, Tools"; // A way to check what categories are used...

                SendInDm = false;
            }

            

            string prefix = CommandHandler.GetPrefix(Context.Guild.Id);
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Bot Command Help",
                Description = CommandHelpText,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Bot Prefix: {prefix}" }
            };

            if (SendInDm)
            {
                await Context.User.SendMessageAsync(null, false, embed.Build());
                await Context.Channel.SendMessageAsync($"Help has been sent to you {Context.User.Mention}!", false);
            }
            else
            {
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }
    }
}
