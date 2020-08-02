using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class BotManagementModule : ModuleBase
    {
        [Command("sendupdate")]
        private async Task SendUpdateAsync([Remainder] string update)
        {
            if (Context.User.Id != 211938243535568896) return;

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Name = "Update", IconUrl = Context.Client.CurrentUser.GetAvatarUrl(), Url = Util.DonateUrl },
                Description = $"{update}\n**Type '{Context.Client.CurrentUser.Mention} help new` to see new commands!**\n\nConsider donating to help support development of Lori's Angel ({Util.DonateUrl}).",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Enjoy the update!" },
                Color = Color.Gold
            };

            foreach (var guild in CommandHandler.GetBot().Guilds)
            {
                if (guild.Id != 730573219374825523)
                {
                    ITextChannel channel = guild.DefaultChannel;
                    await channel.SendMessageAsync(null, false, embed.Build());
                }
            }
        }
    }
}
