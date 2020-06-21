using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.CommandModules;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace bModerator
{
    class MBot : Bot
    {
        static void Main(string[] args)
        {
            CommandHandler handler = new MCommandHandler();

            // Todo: Move token and prefix into a config file
            StartBot(handler, "NzEzODcwMzIyNjYyNzY4NzI4.Xu_Pxw.DqT0a3HAfVF4DEy4s-_cBf0qlBY", "~");
        }
    }

    class MCommandHandler : CommandHandler
    {
        public override void SetupHandlers(DiscordSocketClient bot)
        {
            bot.Ready += ReadyAsync;
            bot.MessageReceived += ModuleModerator.FilterMutedAsync;
        }

        private async Task ReadyAsync()
        {
            await GetBot().SetStatusAsync(UserStatus.DoNotDisturb);

            // Todo: Set a custom status (Not able to yet)

            await Task.CompletedTask;
        }
    }

    public class MCommandModule : ModuleBase
    {
        [Command("kick")]
        [Alias("k")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        private async Task KickAsync(IGuildUser user, [Remainder] string reason) => await ModuleModerator.KickAsync(Context, user, reason);

        [Command("ban")]
        [Alias("b")]
        [RequireUserPermission(GuildPermission.BanMembers)]
        private async Task BanAsync(IGuildUser user, [Remainder] string reason) => await ModuleModerator.BanAsync(Context, user, reason);

        [Command("warn")]
        [Alias("w")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        private async Task WarnAsync(IGuildUser user, [Remainder] string reason) => await ModuleModerator.WarnAsync(Context, user, reason);

        [Command("mute")]
        [Alias("m")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        private async Task MuteAsync(IGuildUser user, [Remainder] string reason) => await ModuleModerator.MuteAsync(Context, user, reason);

        [Command("unmute")]
        [Alias("um")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        private async Task UnmuteAsync(IGuildUser user) => await ModuleModerator.UnmuteAsync(Context, user);
        
    }
}
