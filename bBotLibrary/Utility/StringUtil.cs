using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discord.Net.Bot
{
    public class StringUtil
    {
        public static string DonateUrl = "https://www.buymeacoffee.com/blurr";

        /// Convert a string with mentions to a string with readable names
        public static async Task<string> GetReadableMentionsAsync(IGuild guild, string original)
        {
            List<string> splits = original.Split("<@").ToList<string>();
            string result = "";

            foreach (string split in splits)
            {
                if (split.StartsWith("!"))
                {
                    string nsplit = split.Substring(1);
                    string[] splits1 = nsplit.Split(">", 2);
                    ulong id = Convert.ToUInt64(splits1[0]);
                    IGuildUser user = await guild.GetUserAsync(id);

                    result = $"{result}@{user.Username}";
                    for (int i = 1; i < splits1.Length; i++) result = $"{result}{splits1[i]}";
                }
                else
                {
                    result = $"{result}{split}";
                }
            }

            return result;
        }


        /// Get the invite link to add the bot to the server
        public static string GetInviteLink(ulong clientid, int permissions = 8)
        {
            return $"https://discordapp.com/oauth2/authorize?client_id={clientid}&scope=bot&permissions={permissions}";
        }

        /// Returns the strink with an upper case first letter
        public static string ToUppercaseFirst(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            return char.ToUpper(s[0]) + s.Substring(1).ToLower();
        }
    }
}
