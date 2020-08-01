using Discord;
using Discord.Commands;
using Discord.Net.Bot.CommandModules;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class ModeratorModule : ModuleBase
    {
        [Command("kick")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.KickMembers)]
        private async Task KickAsync(IGuildUser user, [Remainder] string reason = "Unknown reason")
        {
            await Context.Message.DeleteAsync();
            await ModuleModerator.KickAsync(Context, user, reason);
        }

        [Command("ban")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.BanMembers)]
        private async Task BanAsync(IGuildUser user, [Remainder] string reason = "Unknown reason")
        {
            await Context.Message.DeleteAsync();
            await ModuleModerator.BanAsync(Context, user, reason);
        }

        [Command("mute")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        private async Task MuteAsync(IGuildUser user, [Remainder] string reason = "Unknown reason")
        {
            await Context.Message.DeleteAsync();
            await ModuleModerator.MuteAsync(Context, user, reason);
        }

        [Command("unmute")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.MuteMembers)]
        private async Task UnmuteAsync(IGuildUser user)
        {
            await Context.Message.DeleteAsync();
            await ModuleModerator.UnmuteAsync(Context, user);
        }

        [Command("createmute")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        private async Task CreateMuteAsync()
        {
            await Context.Message.DeleteAsync();
            await ModuleModerator.MakeMuteRoleAsunc(Context);
        }
    }
}
