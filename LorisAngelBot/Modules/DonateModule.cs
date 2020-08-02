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
    public class DonateModule : ModuleBase
    {
        [Command("donate")]
        [Alias("beer", "buybeer")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task DonateAsync()
        {
            await Context.Message.DeleteAsync();
            await SendDonateMessageAsync(Context.Channel as ITextChannel, "Buy me a beer so I can continue to add features and host the bot!\nAlternatively you can become a member and retrieve Lori's Angel benefits!");
        }

        public static async Task SendDonateMessageAsync(ITextChannel channel, string message)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { Url = Util.DonateUrl, IconUrl = "https://img.buymeacoffee.com/api/?url=aHR0cHM6Ly9jZG4uYnV5bWVhY29mZmVlLmNvbS91cGxvYWRzL3Byb2ZpbGVfcGljdHVyZXMvMjAyMC8wOC9hMzM3NTlmMTM3MWU4MGI4MTZmMDk1MzE4NDliZTFhYS5wbmc=&size=300&name=blurr", Name = "Donate Here!" },
                Description = $"{message}",
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomHeartEmoji()}  {Util.DonateUrl}" },
                Color = Color.DarkPurple
            };
            await channel.SendMessageAsync(null, false, embed.Build());
        }

        public static bool IsDonator(ulong id)
        {
            DonateFile file = DonateFile.Load();

            foreach (Donator donator in file.Donators)
            {
                if (donator.Id == id)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class DonateFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "config");

        [JsonIgnore]
        static readonly string Filename = "donators.json";

        public List<Donator> Donators { get; set; }

        public DonateFile()
        {
            Donators = new List<Donator>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                DonateFile file = new DonateFile();

                Donator me = new Donator();
                me.Id = 211938243535568896;
                me.Member = "Blurr";
                file.Donators.Add(me);

                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static DonateFile Load()
        {
            return JsonConvert.DeserializeObject<DonateFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class Donator
    {
        public ulong Id { get; set; }
        public string Member { get; set; }
    }
}
