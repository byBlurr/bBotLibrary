using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class ShipModule : ModuleBase
    {
        [Command("ship")]
        public async Task ShipUsersAsync(IUser user, IUser user2)
        {
            await Context.Message.DeleteAsync();
            await ShipAsync(Context, user.Username, user2.Username);
        }

        [Command("ship")]
        public async Task ShipUserAsync(IUser user2)
        {
            await Context.Message.DeleteAsync();
            await ShipAsync(Context, Context.User.Username, user2.Username);
        }

        [Command("ship")]
        public async Task ShipNamesAsync(string user, string user2)
        {
            await Context.Message.DeleteAsync();
            await ShipAsync(Context, user, user2);
        }

        [Command("ship")]
        public async Task ShipNameAsync(string user2)
        {
            await Context.Message.DeleteAsync();
            await ShipAsync(Context, Context.User.Username, user2);
        }



        public async Task ShipAsync(ICommandContext Context, string user, string user2)
        {
            var relationships = Relationships.Load();
            string title;
            string message;
            float score = 0f;
            string name = "";
            bool found = false;

            foreach (var couple in relationships.Couples)
            {
                if (couple.User == user.ToLower() || couple.User2 == user.ToLower())
                {
                    if (couple.User == user2.ToLower() || couple.User2 == user2.ToLower())
                    {
                        score = couple.Percentage;
                        name = couple.Name;
                        found = true;
                        break;
                    }
                }
            }

            if (!found)
            {
                Random rnd = new Random();
                score = (float)(rnd.Next(0, 99) + rnd.NextDouble());
                string name1 = user.ToLower();
                string name2 = user2.ToLower();
                name = $"{name1[0].ToString().ToUpper()}{name1[1]}{name2[name2.Length - 3]}{name2[name2.Length - 2]}{name2[name2.Length - 1]}";

                relationships.Couples.Add(new Relationship(name1, name2, name, score));
                relationships.Save();
            }

            if (score > 95f)
            {
                title = $"{user} 💘 {user2}";
                message = $"I really ship {name}! They're soul mates, a {score}% match!";
            }
            else if (score > 55f)
            {
                title = $"{user} ❤️ {user2}";
                message = $"I ship {name}! They get a {score}% match!";
            }
            else
            {
                title = $"{user} 💔 {user2}";
                message = $"Can't say I ship {name}! They get a shitty {score}% match!";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Color = Color.DarkPurple,
                Title = title,
                Description = message,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}." },
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }
    }

    public class Relationship
    {
        public string User { get; set; }
        public string User2 { get; set; }
        public string Name { get; set; }
        public float Percentage { get; set; }

        public Relationship(string u, string u2, string n, float percentage)
        {
            User = u;
            User2 = u2;
            Name = n;
            Percentage = percentage;
        }
    }

    public class Relationships
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "ships");

        [JsonIgnore]
        static readonly string Filename = "couples.json";

        public List<Relationship> Couples { get; set; }

        public Relationships()
        {
            Couples = new List<Relationship>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                new Relationships().Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static Relationships Load()
        {
            return JsonConvert.DeserializeObject<Relationships>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
