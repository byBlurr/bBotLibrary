using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.CommandModules;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class HelpModule : ModuleBase
    {
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

            await Context.Channel.SendMessageAsync($"The prefix for the bot in this server has been successfully changed to ''{prefix}''.");
        }

        [Command("help")]
        [Alias("h", "?")]
        private async Task HelpAsync(string section = "")
        {
            await ModuleHelp.HelpAsync(Context, section);
        }
    }
}
