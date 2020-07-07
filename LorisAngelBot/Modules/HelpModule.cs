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
            new BotCommand("SHIP", "`-ship @user1 @user2`, `-ship name1 name2`, `-ship user2`, `-ship name2`", "Test how strong your relationship is!", CommandCategory.Games, "Jimmy"),
            new BotCommand("PUNISH", "`-punish @user`", "Punish them for their actions!", CommandCategory.Games, "Jimmy, Ras"),
            new BotCommand("REVERSE", "`-reverse <message>`", "Reverse the message!", CommandCategory.Games, ""),
            new BotCommand("8BALL", "`-8ball <question>`", "Ask the bot a question!", CommandCategory.Games, "Starri"),
            new BotCommand("QUOTE", "`-quote @author <message>`", "Save a quote to the database! (WIP)", CommandCategory.Games, ""),
            new BotCommand("OLDEST", "`-oldest`", "Find out who the oldest user in the guild is!", CommandCategory.User, ""),
            new BotCommand("AVATAR", "`-avatar @user`", "View the users avatar!", CommandCategory.User, "Libby"),
            new BotCommand("WHOIS", "`-whois @user`", "View details about the user!", CommandCategory.User, "Libby"),
            new BotCommand("INVITE", "`-invite`, `inv`", "Get the invite link to invite Loris Angel to your server!", CommandCategory.BotRelated, ""),
            new BotCommand("PREFIX", "`-prefix <prefix>`", "Change the prefix to the bot for your server!", CommandCategory.BotRelated, ""),
            new BotCommand("BINARY", "`-binary <text>`", "Convert text to binary!", CommandCategory.Tools, ""),
        };

        [Command("invite")]
        [Alias("inv")]
        private async Task InviteAsybc()
        {
            await Context.Message.DeleteAsync();
            await Context.User.SendMessageAsync($"Invite Loris Angel to your server: {Util.GetInviteLink(729696788097007717)}");
        }

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        private async Task PrefixAsybc(string prefix)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);
            gconf.Prefix = prefix;
            conf.Save();

            await Context.Channel.SendMessageAsync($"The prefix for the bot in this server has been succefully changed to ''{prefix}''.");
        }

        [Command("help")]
        [Alias("h", "?")]
        private async Task HelpAsync(string section = "NONE")
        {
            await Context.Message.DeleteAsync();

            string CommandHelpText = "";
            bool SendInDm = false;

            foreach (BotCommand command in Commands)
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
                    "BotRelated, Games, User, Tools";

                SendInDm = false;
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Loris Angel Help",
                Description = CommandHelpText,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Bot Prefix: {BotConfig.Load().GetConfig(Context.Guild.Id).Prefix}"}
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
