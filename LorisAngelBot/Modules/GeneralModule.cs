using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Discord.Color;

namespace LorisAngelBot.Modules
{
    public class GeneralModule : ModuleBase
    {
        [Command("region")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task RegionAsync()
        {
            await Context.Message.DeleteAsync();
            await Context.Channel.SendMessageAsync("Guild Region: " + Util.ToUppercaseFirst(Context.Guild.VoiceRegionId));
        }

        [Command("who")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task WhoAsync([Remainder] string question)
        {
            await Context.Message.DeleteAsync();

            ulong[] mentions = Context.Message.MentionedUserIds.ToArray<ulong>();

            Random rnd = new Random();
            int answer = rnd.Next(0, mentions.Length);
            IGuildUser answerUser = await Context.Guild.GetUserAsync(mentions[answer]);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"Who {await Util.GetReadableMentionsAsync(Context.Guild as IGuild, question.ToLower())}",
                Description = $"The answer to that would be {answerUser.Username}.",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}"}
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("avatar")]
        [Alias("av")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task AvatarAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null) user = Context.User as IUser;
            string avatar = user.GetAvatarUrl(size: 2048);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{user.Username}#{user.Discriminator}",
                Color = Color.DarkPurple,
                ImageUrl = avatar,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("avatar")]
        [Alias("av")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task AvatarAsync(ulong userid)
        {
            await Context.Message.DeleteAsync();

            IUser user = await Context.Guild.GetUserAsync(userid) as IUser;
            if (user == null)
            {
                foreach (var guild in CommandHandler.GetBot().Guilds)
                {
                    if (await (guild as IGuild).GetUserAsync(userid) as IUser != null)
                    {
                        user = await (guild as IGuild).GetUserAsync(userid) as IUser;
                    }
                }
            }

            string avatar = user.GetAvatarUrl(size: 2048);
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = $"{user.Username}#{user.Discriminator}",
                Color = Color.DarkPurple,
                ImageUrl = avatar,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("whois")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task WhoIsAsync(IUser user)
        {
            await Context.Message.DeleteAsync();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = user.Username, IconUrl = user.GetAvatarUrl(size: 1024) },
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
                ImageUrl = user.GetAvatarUrl(size: 1024),
            };

            if ((user as IGuildUser).Nickname != null) embed.AddField(new EmbedFieldBuilder() { Name = "Nickname", Value = (user as IGuildUser).Nickname, IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Created On", Value = user.CreatedAt.DateTime.ToUniversalTime().ToString(), IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "Joined At", Value = (user as IGuildUser).JoinedAt.Value.DateTime.ToUniversalTime().ToString(), IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "User Id", Value = user.Id, IsInline = true });
            if ((user as IGuildUser).IsBot) embed.AddField(new EmbedFieldBuilder() { Name = "Bot?", Value = "Yes", IsInline = true });

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("reverse")]
        [Alias("r")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task ReverseAsync([Remainder] string message = "")
        {
            if (message != "")
            {
                await Context.Message.DeleteAsync();
                string newMessage = "";

                foreach (char l in message)
                {
                    newMessage = l + newMessage;
                }

                await Context.Channel.SendMessageAsync(newMessage);
            }
        }

        [Command("oldest")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        public async Task OldestAsync()
        {
            await Context.Message.DeleteAsync();
            IUser oldest = null;

            foreach (var user in await Context.Guild.GetUsersAsync())
            {
                if (!user.IsBot)
                {
                    if (oldest == null) oldest = user;
                    else if (oldest.CreatedAt.Date > user.CreatedAt.Date)
                    {
                        oldest = user;
                    }
                }
            }

            if (oldest != null)
            {
                EmbedBuilder embed = new EmbedBuilder() { };
                embed.WithAuthor(new EmbedAuthorBuilder() { Name = oldest.Username + "#" + oldest.Discriminator, IconUrl = oldest.GetAvatarUrl() });
                embed.WithDescription($"The oldest account in the server, first registered {oldest.CreatedAt.Date}!");
                embed.WithColor(Color.DarkPurple);
                embed.Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("users")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task UsersAsync()
        {
            await Context.Message.DeleteAsync();

            var bot = LCommandHandler.GetBot();
            int guilds = bot.Guilds.Count;
            int users = (await Context.Guild.GetUsersAsync()).Count;
            int total = 0;
            foreach (var guild in bot.Guilds) total += guild.Users.Count;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Lori's Angels Statistics",
                Color = Color.DarkPurple,
                Description = $"Out of the {guilds} guilds I am watching {total} total users, {users} of which are from this guild!",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("uptime")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task UptimeAsync()
        {
            await Context.Message.DeleteAsync();

            BotConfig conf = BotConfig.Load();
            DateTime time = DateTime.UtcNow;
            int minutes = (int)((time - conf.LastStartup).TotalMinutes);
            int uptime = 0;
            string m = "minutes";

            if (minutes >= 60)
            {
                uptime = minutes / 60;
                m = "hours";
            }
            else uptime = minutes;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Lori's Angels Statistics",
                Color = Color.DarkPurple,
                Description = $"Lori's Angel has been online since {time}, thats an uptime of {uptime} {m}!",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("binary")]
        [Alias("bin")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BinaryAsync([Remainder] string text)
        {
            await Context.Message.DeleteAsync();
            var binary = ToBinary(ConvertToByteArray(text, Encoding.ASCII));

            EmbedBuilder embed = new EmbedBuilder() {
                Title = $"Text to Binary",
                Description = $"''{text}''\n\n{binary}",
                Color = Color.DarkPurple
            };
            embed.Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        public static byte[] ConvertToByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }

        public static String ToBinary(Byte[] data)
        {
            return string.Join(" ", data.Select(byt => Convert.ToString(byt, 2).PadLeft(8, '0')));
        }
    }
}
