using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Discord.Net.Bot.Database.Configs;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = Discord.Color;

namespace LorisAngelBot.Modules
{
    public class GeneralModule : ModuleBase
    {
        [Command("quote")]
        private async Task QuoteAsync(IUser author = null, [Remainder] string text = "")
        {
            await Context.Message.DeleteAsync();
            Random rnd = new Random();
            string path = "";

            if (text != "")
            {
                string quote = $"''{Util.ToUppercaseFirst(text)}'' - {Util.ToUppercaseFirst(author.Username)}";

                int fontSize = 40;
                int textureWidth = (quote.Length * (fontSize + 4)) / 3;

                Bitmap quoteBitmap = new Bitmap(textureWidth, fontSize);
                Graphics quoteImage = Graphics.FromImage(quoteBitmap);

                quoteImage.SmoothingMode = SmoothingMode.AntiAlias;
                quoteImage.FillRectangle(Brushes.SlateGray, new Rectangle(0, 0, textureWidth, fontSize));

                StringFormat format = new StringFormat();
                format.LineAlignment = StringAlignment.Center;
                format.Alignment = StringAlignment.Center;
                quoteImage.DrawString(quote, new Font("Arial", fontSize / 2, FontStyle.Bold), Brushes.White, textureWidth / 2, fontSize / 2, format);

                path = Path.Combine(AppContext.BaseDirectory, $"Quotes/{Context.User.Id}_{rnd.Next(0, 9999)}.jpg");
                quoteBitmap.Save(path);

                quoteImage.Dispose();
                quoteBitmap.Dispose();
            }
            else
            {
                string[] files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Quotes"));
                int index = rnd.Next(0, files.Length - 1);
                path = files[index];
                
            }

            await Context.Channel.SendFileAsync(path, "This feature is still work in progress, I will be making the quotes look better!");
        }

        [Command("avatar")]
        [Alias("av")]
        private async Task AvatarAsync(IUser user)
        {
            await Context.Message.DeleteAsync();

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
        [Alias("who")]
        private async Task WhoIsAsync(IUser user)
        {
            await Context.Message.DeleteAsync();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = user.Username, IconUrl = user.GetAvatarUrl(size: 1024) },
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };

            embed.AddField(new EmbedFieldBuilder() { Name = "Created On", Value = user.CreatedAt.DateTime.ToUniversalTime().ToString(), IsInline = true });
            embed.AddField(new EmbedFieldBuilder() { Name = "User Id", Value = user.Id, IsInline = true });

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("reverse")]
        [Alias("r")]
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
                Description = $"Out of the {guilds} guilds I am in there are {total} total users, {users} of which are from this guild!",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Id}"}
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("binary")]
        [Alias("bin")]
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
