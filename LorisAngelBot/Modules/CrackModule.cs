using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class CrackModule : ModuleBase
    {
        [Command("crack")]
        [Alias("crackpass", "crackpassword")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task CrackPasswordAsync()
        {
            await Context.Message.DeleteAsync();

            if (CrackGames.GetGame(Context.User.Id) == null)
            {
                CrackGame game = new CrackGame(Context.User.Id);
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Password Cracking",
                    Description = $"The following line from a password file has been leaked.\n{game.Username}:{game.Hash}\n\nCrack the password and enter it below in plaintext.\n(Hint: Use a dictionary attack)",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                };
                game.Message = await Context.Channel.SendMessageAsync(null, false, embed.Build());
                CrackGames.Games.Add(game);
            }
        }

    }

    public class CrackGames
    {
        public static List<CrackGame> Games = new List<CrackGame>();

        public static CrackGame GetGame(ulong id)
        {
            foreach (CrackGame game in Games)
            {
                if (game.User == id)
                {
                    return game;
                }
            }
            return null;
        }

        public static async Task CheckPasswordAsync(ulong id, string plaintext)
        {
            CrackGame game = GetGame(id);
            if (game != null)
            {
                if (game.Plaintext == plaintext)
                {
                    game.Attempts++;

                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "Password Cracking",
                        Description = $"The following line from a password file has been leaked.\n{game.Username}:{game.Hash}\n\n**Username:** {game.Username}\n**Password:** {game.Plaintext}",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Logged in successfully!" }
                    };

                    await game.Message.ModifyAsync(x => x.Embed = embed.Build());
                    Games.Remove(game);
                }
                else
                {
                    game.Attempts++;

                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "Password Cracking",
                        Description = $"**The following line from a password file has been leaked.**\n{game.Username}:{game.Hash}\n\n**Username:** {game.Username}\n**Password:** ",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Attempts: {game.Attempts}/5" }
                    };
                    
                    if (game.Attempts >= 5)
                    {
                        Games.Remove(game);
                        embed.Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Login failed, account has been locked for your security." };
                    }

                    await game.Message.ModifyAsync(x => x.Embed = embed.Build());
                }
            }
        }
    }

    public class CrackGame
    {
        public string Username { get; set; }
        public string Plaintext { get; set; }
        public HashingAlgorithm Algorithm { get; set; }
        public string Hash { get; set; }
        public ulong User { get; set; }
        public int Attempts { get; set; }
        public IUserMessage Message { get; set; }

        public CrackGame(ulong user)
        {
            User = user;

            //Array values = Enum.GetValues(typeof(HashingAlgorithm));
            //Random random = new Random();
            //Algorithm = (HashingAlgorithm) values.GetValue(random.Next(values.Length));
            Algorithm = HashingAlgorithm.MD5;

            Attempts = 0;

            List<string> words = CrackFile.Load().Words;

            Random rnd = new Random();
            int u = rnd.Next(0, words.Count);
            int p = rnd.Next(0, words.Count);
            Username = words[u] + "@blurrmail.co.uk";
            Plaintext = words[p];

            switch (Algorithm)
            {
                case HashingAlgorithm.MD5:
                    Hash = CreateMD5(Plaintext);
                    break;
                case HashingAlgorithm.SHA1:
                    Hash = CreateSHA1(Plaintext);
                    break;
                case HashingAlgorithm.SHA256:
                    Hash = CreateSHA256(Plaintext);
                    break;
                case HashingAlgorithm.SHA384:
                    Hash = CreateSHA384(Plaintext);
                    break;
                case HashingAlgorithm.SHA512:
                    Hash = CreateSHA512(Plaintext);
                    break;

            }
        }

        private string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private string CreateSHA1(string input)
        {
            // Use input string to calculate MD5 hash
            using (SHA1 sha = SHA1.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private string CreateSHA256(string input)
        {
            // Use input string to calculate MD5 hash
            using (SHA256 sha = SHA256.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private string CreateSHA384(string input)
        {
            // Use input string to calculate MD5 hash
            using (SHA384 sha = SHA384.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private string CreateSHA512(string input)
        {
            // Use input string to calculate MD5 hash
            using (SHA512 sha = SHA512.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = sha.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }

    public class CrackFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "crack");

        [JsonIgnore]
        static readonly string Filename = "possibleWords.json";

        public List<string> Words { get; set; }

        public CrackFile()
        {
            Words = new List<string>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                CrackFile file = new CrackFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static CrackFile Load()
        {
            return JsonConvert.DeserializeObject<CrackFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public enum HashingAlgorithm
    {
        MD5, SHA1, SHA256, SHA384, SHA512
    }
}
