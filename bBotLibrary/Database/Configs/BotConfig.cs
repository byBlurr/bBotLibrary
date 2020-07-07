using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Discord.Net.Bot.Database.Configs
{
    public class BotConfig
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "config");

        [JsonIgnore]
        static readonly string Filename = "botconfig.json";

        public ConfigType Type { get; set; }
        public string Token { get; set; }
        public bool PrintVerbose { get; set; }
        public DateTime LastStartup { get; set; }
        public IndividualConfig SoloConfig { get; set; }
        public List<IndividualConfig> Configs { get; set; }
        public List<BotCommand> Commands { get; set; }

        public BotConfig()
        {
            Type = ConfigType.Solo;
            Token = "";
            PrintVerbose = false;
            LastStartup = DateTime.UtcNow;
            SoloConfig = new IndividualConfig();
            Configs = new List<IndividualConfig>();
            Commands = new List<BotCommand>();
        }

        public IndividualConfig GetConfig(ulong guild)
        {
            foreach (IndividualConfig config in Configs)
            {
                if (config.Guild == guild) return config;
            }
            return null;
        }

        public IndividualConfig FreshConfig(ulong guild)
        {
            IndividualConfig conf = new IndividualConfig();
            conf.Guild = guild;
            conf.Prefix = SoloConfig.Prefix;
            return conf;
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            return File.Exists(Path.Combine(Dir, Filename));
        }

        public static BotConfig CheckConfig()
        {
            BotConfig config;

            if (BotConfig.Exists()) config = BotConfig.Load();
            else
            {
                // Create config...
                config = new BotConfig();
                Console.Clear();
                Console.WriteLine("No config file was found...\n");

                Console.WriteLine("Bot Token: ");
                Console.WriteLine("Create a bot at https://discord.com/developers \n");
                config.Token = Console.ReadLine();
                Console.Clear();

                Console.WriteLine("Print Verbose: ");
                Console.WriteLine("Would you like the console to print Verbose logging? Y/N\n");
                string input = Console.ReadLine();
                if (input.ToLower()[0] == 'y') config.PrintVerbose = true;
                else config.PrintVerbose = false;
                Console.Clear();

                bool typeSelected = false;
                while (!typeSelected)
                {
                    Console.WriteLine("Config Type: s | i\nS - Solo (One config for the whole bot)\nI - Individual (Different config for each guild)\n");
                    input = Console.ReadLine();
                    if (input.ToLower().Equals("s"))
                    {
                        config.Type = ConfigType.Solo;
                        typeSelected = true;
                    }
                    else if (input.ToLower().Equals("i"))
                    {
                        config.Type = ConfigType.Individual;
                        typeSelected = true;
                    }
                    Console.Clear();
                }

                Console.WriteLine("Default Prefix: ");
                config.SoloConfig.Prefix = Console.ReadLine();
                Console.Clear();

                Console.WriteLine($"Config file created...\nToken: {config.Token}\nPrefix: {config.SoloConfig.Prefix}\nType: {config.Type}");
                config.Save();
                Console.WriteLine("Config file saved...");
                Console.Clear();
            }

            Console.WriteLine("Config file loaded.");
            return config;
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static BotConfig Load()
        {
            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }
        
        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}
