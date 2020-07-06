using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class HelpModule : ModuleBase
    {

        BotCommand[] Commands = new BotCommand[]
        {
            new BotCommand("SHIP", "`-ship @user1 @user2`, `-ship name1 name2`, `-ship user2`, `-ship name2`", "Test how strong your relationship is!", CommandCategory.Games),
            new BotCommand("INVITE", "`-invite`, `inv`", "Get the invite link to invite Loris Angel to your server!", CommandCategory.BotRelated),
        };

        [Command("invite")]
        [Alias("inv")]
        private async Task InviteAsybc()
        {
            await Context.Message.DeleteAsync();
            await Context.User.SendMessageAsync($"Invite Loris Angel to your server: {Util.GetInviteLink(729696788097007717)}");
        }

        [Command("help")]
        [Alias("h", "?")]
        private async Task HelpAsync(string section = "NONE")
        {
            await Context.Message.DeleteAsync();

            string CommandHelpText = "";

            foreach (BotCommand command in Commands)
            {
                if (command.Category.ToString().ToLower() == section.ToLower() || Commands.Length <= 5)
                {
                    string help = $"{command.Handle}\nUsage:\n{command.Usage}\n{command.Description}";
                    CommandHelpText = CommandHelpText + "\n\n" + help;
                }
            }

            if (CommandHelpText.Length <= 0) 
                CommandHelpText = "No commands found in this category\n\n" +
                    "Command Usage: -help <category>\n\n" +
                    "Categories:\n" +
                    "BotRelated, Games";

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Loris Angel Help",
                Description = CommandHelpText,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Bot Prefix: {BotConfig.Load().GetConfig(Context.Guild.Id).Prefix}"}
            };

            await Context.User.SendMessageAsync(null, false, embed.Build());
            await Context.Channel.SendMessageAsync($"Help has been sent to you {Context.User.Mention}!", false);
        }
    }

    public class BotCommand
    {

        public string Handle { get; set; }
        public string Usage { get; set; }
        public string Description { get; set; }
        public CommandCategory Category { get; set; }

        public BotCommand(string handle, string usage, string description, CommandCategory category)
        {
            Handle = handle ?? throw new ArgumentNullException(nameof(handle));
            Usage = usage ?? throw new ArgumentNullException(nameof(usage));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Category = category;
        }
    }
}
