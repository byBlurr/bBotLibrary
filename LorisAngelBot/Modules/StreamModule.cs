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
    public class StreamModule : ModuleBase
    {
        [Command("stream")]
        [Alias("twitch")]
        private async Task AddStreamAsync(string url)
        {
            await Context.Message.DeleteAsync();
            if (url.ToLower().Contains("twitch"))
            {
                StreamFile file = StreamFile.Load();
                var stream = new Stream(Context.User.Username, Context.User.Id, url);

                if (!file.Streams.Contains(stream))
                {
                    file.Streams.Add(stream);
                    file.Save();
                }

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Random Stream Ad",
                    Description = "Your stream has been added to the list!",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Good Luck!" }
                };

                await Context.User.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                await Context.Channel.SendMessageAsync("We currently only support twitch streams.");
            }
        }

        public static async Task PickNewWinnerAsync()
        {
            StreamFile file = StreamFile.Load();

            Random rnd = new Random();
            int random = rnd.Next(0, file.Streams.Count);

            file.Current = file.Streams[random];
            file.Save();
        }
    }

    public class Stream
    {
        public Stream(string user, ulong id, string url)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Id = id;
            Url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public string User { get; set; }
        public ulong Id { get; set; }
        public string Url { get; set; }
    }

    public class StreamFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "stream");

        [JsonIgnore]
        static readonly string Filename = "streams.json";

        public List<Stream> Streams { get; set; }
        public Stream Current { get; set; }

        public StreamFile()
        {
            Streams = new List<Stream>();
            Current = null;
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                new StreamFile().Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static StreamFile Load()
        {
            return JsonConvert.DeserializeObject<StreamFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
