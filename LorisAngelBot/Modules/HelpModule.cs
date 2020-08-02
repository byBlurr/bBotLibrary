using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.CommandModules;
using Discord.Net.Bot.Database.Configs;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class HelpModule : ModuleBase
    {
        [Command("invite")]
        [Alias("inv")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task InviteAsybc()
        {
            await Context.Message.DeleteAsync();
            await Context.User.SendMessageAsync($"Invite Loris Angel to your server: {Util.GetInviteLink(729696788097007717)}\n\nJoin the support server: https://discord.gg/th7XNE8");
        }


        [Command("support")]
        [Alias("server", "request", "support server", "supportserver")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ServerAsync()
        {
            await Context.Message.DeleteAsync();

            await Context.User.SendMessageAsync($"You can join our support server https://discord.gg/th7XNE8. There you can suggest new features, get help or use the bot with other users!");
        }

        [Command("prefix")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PrefixAsybc(string prefix)
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            var gconf = conf.GetConfig(Context.Guild.Id);
            gconf.Prefix = prefix;
            conf.Save();

            await Context.Channel.SendMessageAsync($"The prefix for the bot in this server has been successfully changed to ''{prefix}''.");
        }

        [Command("help")]
        [Alias("h", "?")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task HelpAsync([Remainder] string section = "")
        {
            await ModuleHelp.HelpAsync(Context, section);
        }
    }
}
