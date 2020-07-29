using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class _8BallModule : ModuleBase
    {

        private string[] YesReplies =
        {
            "Yes",
            "Definitely",
            "Aye",
            "All signs point to yes",
            "You may rely on it",
            "Do pigs roll in mud?",
        };

        private string[] NoReplies =
        {
            "No",
            "Not in a million years",
            "Keep dreaming",
            "Absolutely not",
            "Nay",
            "My sources say no"
        };

        private string[] MaybeReplies =
        {
            "Maybe",
            "Possibly",
            "Most probably",
            "Can not predict right now",
            "Don't count on it",
            "Better to not tell you right now",
        };

        [Command("8ball")]
        [Alias("8b")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task EightBallAsync([Remainder] string question)
        {
            await Context.Message.DeleteAsync();

            string reply;

            Random rnd = new Random();
            int random = rnd.Next(0, 3);
            switch (random)
            {
                case 0:
                    int y = rnd.Next(0, YesReplies.Length);
                    reply = YesReplies[y];
                    break;
                case 1:
                    int n = rnd.Next(0, NoReplies.Length);
                    reply = NoReplies[n];
                    break;
                default:
                    int m = rnd.Next(0, MaybeReplies.Length);
                    reply = MaybeReplies[m];
                    break;
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = question,
                Description = reply + "!",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
