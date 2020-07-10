using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class DiceModule : ModuleBase
    {
        [Command("dice")]
        [Alias("die")]
        private async Task DiceAsync(int amount = 1)
        {
            await Context.Message.DeleteAsync();
            if (amount < 1) amount = 1;
            else if (amount > 100) amount = 100;

            Random rnd = new Random();
            List<int> rolls = new List<int>();
            int total = 0;
            string text = "";

            for (int i = 0; i < amount; i++)
            {
                int roll = rnd.Next(1, 7);
                rolls.Add(roll);
                total += roll;
                text += $"**{roll}**, ";
            }
            text = text.Substring(0, text.Length - 2);

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Dice Roll",
                Description = $"You rolled **{amount}** dice for **{total}**! Dice: [{text}]",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }
}
