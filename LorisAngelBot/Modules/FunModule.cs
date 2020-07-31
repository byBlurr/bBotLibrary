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
    public class FunModule : ModuleBase
    {
        [Command("kill")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task KillAsync(IUser user)
        {
            await Context.Message.DeleteAsync();

            Random rnd = new Random();
            List<string> deaths = DeathsFile.Load().Deaths;
            int d = rnd.Next(0, deaths.Count);
            string death = deaths[d].Replace("USER1", Util.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", Util.ToUppercaseFirst(user.Mention));
            await Context.Channel.SendMessageAsync(death);
        }

        [Command("roast")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task RoastAsync(IUser user)
        {
            await Context.Message.DeleteAsync();
            if (user == null) user = Context.User as IUser;

            Random rnd = new Random();
            List<string> roasts = RoastsFile.Load().Roasts;
            int d = rnd.Next(0, roasts.Count);
            string roast = roasts[d].Replace("USER", Util.ToUppercaseFirst(user.Mention));
            await Context.Channel.SendMessageAsync(roast);
        }

        [Command("epic")]
        [Alias("rate")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task EpicRatingAsync(IUser user = null)
        {
            await Context.Message.DeleteAsync();
            if (user == null) user = Context.User as IUser;

            Random rnd = new Random();
            float rating = (float)(rnd.Next(0, 100) + rnd.NextDouble());

            if (user.IsBot && user.Id != 729696788097007717L) rating = (float)(rnd.Next(0, 20) + rnd.NextDouble());
            else if (user.IsBot && user.Id == 729696788097007717L) rating = (float)(rnd.Next(80, 20) + rnd.NextDouble());

            EmbedBuilder embed = new EmbedBuilder()
            {
                Author = new EmbedAuthorBuilder() { IconUrl = user.GetAvatarUrl(), Name = user.Username },
                Description = $"You are {rating}% EPIC!",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}"}
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("punish")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task PunishAsync(IUser user)
        {
            await Context.Message.DeleteAsync();

            Random rnd = new Random();
            List<string> punishments = PunishFile.Load().Punishments;
            int r = rnd.Next(0, punishments.Count);

            string punishment = punishments[r].Replace("USER1", Util.ToUppercaseFirst(Context.User.Mention)).Replace("USER2", Util.ToUppercaseFirst(user.Mention));

            await Context.Channel.SendMessageAsync(punishment);
        }
    }

    public class DeathsFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "deaths.json";

        public List<string> Deaths { get; set; }

        public DeathsFile()
        {
            Deaths = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                DeathsFile file = new DeathsFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static DeathsFile Load()
        {
            return JsonConvert.DeserializeObject<DeathsFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    public class RoastsFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "roasts.json";

        public List<string> Roasts { get; set; }

        public RoastsFile()
        {
            Roasts = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                RoastsFile file = new RoastsFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static RoastsFile Load()
        {
            return JsonConvert.DeserializeObject<RoastsFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    public class PunishFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "fun");

        [JsonIgnore]
        static readonly string Filename = "punishments.json";

        public List<string> Punishments { get; set; }

        public PunishFile()
        {
            Punishments = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                PunishFile file = new PunishFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static PunishFile Load()
        {
            return JsonConvert.DeserializeObject<PunishFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
