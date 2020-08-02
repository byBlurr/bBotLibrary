using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class CurrencyModule : ModuleBase
    {
        [Command("bank")]
        [Alias("balance", "money")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task BalanceAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();

            if (user == null) user = Context.User as IUser;

            BankFile bank = BankFile.Load();
            foreach (BankAccount account in bank.Accounts)
            {
                if (account.Id == user.Id)
                {
                    account.Username = user.Username;
                    bank.Save();

                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = $"{user.Username}'s Bank Balance",
                        Description = $"{user.Username} has ${account.Balance}",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());

                    return;
                }
            }

            BankAccount newAccount = new BankAccount();
            newAccount.Id = user.Id;
            newAccount.Balance = 50;
            newAccount.Username = user.Username;
            bank.Accounts.Add(newAccount);
            bank.Save();

            EmbedBuilder failEmbed = new EmbedBuilder()
            {
                Title = $"{user.Username}'s Bank Balance",
                Description = $"Bank account created.\n{user.Username} has ${newAccount.Balance}",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
            };
            await Context.Channel.SendMessageAsync(null, false, failEmbed.Build());
        }

        [Command("daily")]
        [Alias("claim", "bonus")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task ClaimAsync()
        {
            await Context.Message.DeleteAsync();

            BankFile bank = BankFile.Load();
            foreach (ulong id in bank.Claimed)
            {
                if (id == Context.User.Id)
                {
                    // Already claimed
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = $"Daily Bonuses",
                        Description = $"You have already claimed your daily bonus.",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());

                    return;
                }
            }

            int bonus = DonateModule.IsDonator(Context.User.Id) ? 150 : 50;
            foreach (BankAccount account in bank.Accounts)
            {
                if (account.Id == Context.User.Id)
                {
                    account.Balance += bonus;
                    account.Username = Context.User.Username;
                    bank.Claimed.Add(Context.User.Id);
                    bank.Save();

                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = $"Daily Bonuses",
                        Description = $"You have claimed a bonus of ${bonus}.\nDonators get $150!",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());

                    return;
                }
            }

            EmbedBuilder failEmbed = new EmbedBuilder()
            {
                Title = $"Daily Bonuses",
                Description = $"You do not have an open bank account, open an account with `{CommandHandler.GetPrefix(Context.Guild.Id)}bank`!",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
            };
            await Context.Channel.SendMessageAsync(null, false, failEmbed.Build());
        }

        [Command("give")]
        [Alias("givemoney")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task GiveMoneyAsync(IUser user, int amount)
        {
            await Context.Message.DeleteAsync();

            BankFile bank = BankFile.Load();
            BankAccount giver = null;
            BankAccount receiver = null;
            foreach (BankAccount account in bank.Accounts)
            {
                if (account.Id == Context.User.Id) giver = account;
                if (account.Id == user.Id) receiver = account;
            }

            if (giver == null)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = $"Bank Error",
                    Description = $"{Context.User.ToString()} doesn't have an open bank account. They need to do `{CommandHandler.GetPrefix(Context.Guild.Id)}bank`.",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else if (receiver == null)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = $"Bank Error",
                    Description = $"{user.ToString()} doesn't have an open bank account. They need to do `{CommandHandler.GetPrefix(Context.Guild.Id)}bank`.",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else if (giver.Id == receiver.Id)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = $"Bank Error",
                    Description = $"You can't give yourself money?",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                if (amount < 0) amount = 0;
                else if (amount > giver.Balance) amount = giver.Balance;

                giver.Username = Context.User.Username;
                giver.Balance -= amount;
                receiver.Username = user.Username;
                receiver.Balance += amount;
                bank.Save();

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = $"{Context.User.ToString()} gave {user.ToString()} $$$",
                    Description = $"You gave {user.ToString()} ${amount}.\nNew Balance: ${giver.Balance}",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}"}
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("richest")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task RichestAsync()
        {
            await Context.Message.DeleteAsync();

            BankFile bank = BankFile.Load();
            List<BankAccount> sortedList = bank.Accounts.OrderByDescending(x => x.Balance).ToList();

            string text = "";
            for (int i = 0; i < 10 && i < sortedList.Count; i++)
            {
                text = $"{text}\n{sortedList[i].Username} **${sortedList[i].Balance}**";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Richest Bank Accounts",
                Description = text,
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.ToString()}" }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

    }

    public class BankFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "config");

        [JsonIgnore]
        static readonly string Filename = "bank.json";

        public List<BankAccount> Accounts { get; set; }
        public List<ulong> Claimed { get; set; }

        public BankFile()
        {
            Accounts = new List<BankAccount>();
            Claimed = new List<ulong>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                BankFile file = new BankFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static BankFile Load()
        {
            return JsonConvert.DeserializeObject<BankFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class BankAccount
    {
        public ulong Id { get; set; }
        public string Username { get; set; }
        public int Balance { get; set; }
    }
}
