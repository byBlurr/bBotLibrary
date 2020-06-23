using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        public IndividualConfig SoloConfig { get; set; }
        public List<IndividualConfig> Configs { get; set; }

        public BotConfig()
        {
            Type = ConfigType.Solo;
            Token = "";
            SoloConfig = new IndividualConfig();
            Configs = new List<IndividualConfig>();
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

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static BotConfig Load()
        {
            return JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }
        
        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);

    }

    public class IndividualConfig
    {
        public ulong Guild { get; set; }
        public string Prefix { get; set; }

        public IndividualConfig()
        {
            Guild = 0L;
            Prefix = "-";
        }
    }

    public enum ConfigType
    {
        Solo, Individual
    }
}
