using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
                Author = new EmbedAuthorBuilder() { Name = "Update - " + DateTime.UtcNow.ToShortDateString(), IconUrl = Context.Client.CurrentUser.GetAvatarUrl(), Url = Util.DonateUrl },
                Description = $"{update}\n**Type '{Context.Client.CurrentUser.Mention} help new` to see new commands!**\n\nConsider donating to help support development of Lori's Angel.\nOpt out of updates with '{Context.Client.CurrentUser.Mention} optout'.\nRequest new features with '{Context.Client.CurrentUser.Mention} request <request>'.",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Enjoy the update!" },
                Color = Color.Gold
            };

            foreach (var guild in CommandHandler.GetBot().Guilds)
            {
                BotManagementFile file = BotManagementFile.Load();
                bool dontSend = false;
                foreach (ulong id in file.OptedOut)
                {
                    if (guild.Id == id)
                    {
                        dontSend = true;
                    }
                }

                if (!dontSend)
                {
                    ITextChannel channel = guild.DefaultChannel;
                    await channel.SendMessageAsync(null, false, embed.Build());
                }
            }
        }

        [Command("optout")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireUserPermission(GuildPermission.Administrator)]
        private async Task OptOutAsync()
        {
            BotManagementFile file = BotManagementFile.Load();
            if (!file.OptedOut.Contains(Context.Guild.Id)) file.OptedOut.Add(Context.Guild.Id);
            file.Save();
            await Context.Channel.SendMessageAsync("Opted out :(");
        }

        [Command("request")]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task RequestAsync([Remainder] string request)
        {
            BotManagementFile file = BotManagementFile.Load();
            string req = Context.User.Username.ToLower() + ":" + request;
            if (!file.Requests.Contains(req)) file.Requests.Add(req);
            file.Save();
            await Context.Channel.SendMessageAsync("The request has been saved!");
        }

        [Command("dailyreset")]
        private async Task ResetDailyAsync()
        {
            if (Context.User.Id != 211938243535568896) return;

            BankFile file = BankFile.Load();
            file.Claimed.Clear();
            file.Save();
            await Context.Channel.SendMessageAsync("Reset.");
        }
    }

    public class BotManagementFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "config");

        [JsonIgnore]
        static readonly string Filename = "management.json";

        public List<ulong> OptedOut { get; set; }
        public List<string> Requests { get; set; }

        public BotManagementFile()
        {
            OptedOut = new List<ulong>();
            Requests = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                BotManagementFile file = new BotManagementFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static BotManagementFile Load()
        {
            return JsonConvert.DeserializeObject<BotManagementFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
