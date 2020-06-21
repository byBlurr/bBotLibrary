using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Discord.Net.Bot.CommandModules
{
    public static class ModuleModerator
    {
        public static async Task KickAsync(ICommandContext Context, IGuildUser user, [Remainder] string reason)
        {
            string message = $"{Context.User.ToString()} kicked {user.ToString()}\n{reason}";
            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = "User Kicked", IconUrl = Context.User.GetAvatarUrl() },
                Description = message,
                Color = Color.DarkOrange
            };

            await user.KickAsync(reason);
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
            await Util.Logger(new LogMessage(LogSeverity.Info, "Moderation", $"{Context.User.ToString()} kicked {user.ToString()}"));
            await Task.CompletedTask;
        }
        public static async Task BanAsync(ICommandContext Context, IGuildUser user, [Remainder] string reason)
        {
            string message = $"{Context.User.ToString()} banned {user.ToString()}\n{reason}";
            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = "User Banned", IconUrl = Context.User.GetAvatarUrl() },
                Description = message,
                Color = Color.DarkRed
            };

            await user.BanAsync(0, reason);
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
            await Util.Logger(new LogMessage(LogSeverity.Info, "Moderation", $"{Context.User.ToString()} banned {user.ToString()}"));
            await Task.CompletedTask;
        }
        public static async Task WarnAsync(ICommandContext Context, IGuildUser user, [Remainder] string reason)
        {
            string message = $"{Context.User.ToString()} warned {user.ToString()}\n{reason}";
            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = "User Warned", IconUrl = Context.User.GetAvatarUrl() },
                Description = message,
                Color = Color.DarkRed
            };

            await user.SendMessageAsync(null, false, embed.Build());
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
            await Util.Logger(new LogMessage(LogSeverity.Info, "Moderation", $"{Context.User.ToString()} warned {user.ToString()}"));
            await Task.CompletedTask;
        }
        public static async Task MuteAsync(ICommandContext Context, IGuildUser user, [Remainder] string reason)
        {
            IRole mutedRole = null;
            foreach (IRole role in Context.Guild.Roles)
            {
                if (role.Name.ToLower().Equals("[muted]")) mutedRole = role;
            }

            if (mutedRole != null)
            {
                string message = $"{Context.User.ToString()} muted {user.ToString()}\n{reason}";
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder() { Name = "User Muted", IconUrl = Context.User.GetAvatarUrl() },
                    Description = message,
                    Color = Color.DarkRed
                };

                await user.AddRoleAsync(mutedRole);
                await user.SendMessageAsync(null, false, embed.Build());
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
                await Util.Logger(new LogMessage(LogSeverity.Info, "Moderation", $"{Context.User.ToString()} muted {user.ToString()}"));
            }
            else
            {
                await Context.Message.DeleteAsync();
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "No mute role was found",
                    Description = "The muted role must be called `[muted]`.",
                    Color = Color.DarkRed
                };
                await Context.User.SendMessageAsync(null, false, embed.Build());
            }

            await Task.CompletedTask;
        }
        public static async Task UnmuteAsync(ICommandContext Context, IGuildUser user)
        {
            IRole mutedRole = null;
            foreach (IRole role in Context.Guild.Roles)
            {
                if (role.Name.ToLower().Equals("[muted]")) mutedRole = role;
            }

            if (mutedRole != null)
            {
                string message = $"{Context.User.ToString()} unmuted {user.ToString()}";
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Author = new EmbedAuthorBuilder() { Name = "User Unmuted", IconUrl = Context.User.GetAvatarUrl() },
                    Description = message,
                    Color = Color.DarkGreen
                };

                await user.RemoveRoleAsync(mutedRole);
                await user.SendMessageAsync(null, false, embed.Build());
                await Util.Logger(new LogMessage(LogSeverity.Info, "Moderation", $"{Context.User.ToString()} unmuted {user.ToString()}"));
            }
            else
            {
                await Context.Message.DeleteAsync();
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "No mute role was found",
                    Description = "The muted role must be called `[muted]`.",
                    Color = Color.DarkRed
                };
                await Context.User.SendMessageAsync(null, false, embed.Build());
            }

            await Task.CompletedTask;
        }
        public static async Task FilterMutedAsync(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            IRole mutedRole = null;
            IGuild guild = (message.Channel as IGuildChannel).Guild;

            foreach (IRole role in guild.Roles)
            {
                if (role.Name.ToLower().Equals("[muted]")) mutedRole = role;
            }

            if (mutedRole != null)
            {
                var roleIds = (message.Author as IGuildUser).RoleIds;
                foreach (ulong id in roleIds)
                {
                    if (id == mutedRole.Id) await message.DeleteAsync();
                }
            }

            await Task.CompletedTask;
        }
    }
}
