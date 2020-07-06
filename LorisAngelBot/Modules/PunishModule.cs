using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class PunishModule : ModuleBase
    {
        
        [Command("punish")]
        private async Task PunishAsync(IUser user)
        {
            await Context.Message.DeleteAsync();
            string punishment = GetPunishment(Util.ToUppercaseFirst(Context.User.Username), Util.ToUppercaseFirst(user.Username));
            await Context.Channel.SendMessageAsync(punishment);
        }

        private string GetPunishment(string user, string user2)
        {
            string result;

            Random rnd = new Random();
            int random = rnd.Next(0, 11);

            switch (random)
            {
                case 0:
                    result = $"**{user} slapped {user2}!**";
                    break;
                case 1:
                    result = $"**{user} spanked {user2}!**";
                    break;
                case 2:
                    result = $"**{user} paddled {user2}!**";
                    break;
                case 3:
                    result = $"**{user} used the belt on {user2}!**";
                    break;
                case 4:
                    result = $"**{user} refused to spit on {user2} during sex!**";
                    break;
                case 5:
                    result = $"**{user} whipped {user2}!**";
                    break;
                case 6:
                    result = $"**{user} chained {user2} up!**";
                    break;
                case 7:
                    result = $"**{user} gagged {user2} with their toes!**";
                    break;
                case 8:
                    result = $"**{user} pegged {user2}!**";
                    break;
                case 9:
                    result = $"**{user} ball gagged {user2}!**";
                    break;
                case 10:
                    result = $"**{user} cucked {user2}!**";
                    break;

                default:
                    result = $"**{user} punished {user2}!**";
                    break;
            }

            return result;
        }

    }
}
