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
            int random = rnd.Next(0, 15);

            switch (random)
            {
                case 0:
                    result = $"**_{user} slapped {user2}!_**";
                    break;
                case 1:
                    result = $"**_{user} spanked {user2}!_**";
                    break;
                case 2:
                    result = $"**_{user} paddled {user2}!_**";
                    break;
                case 3:
                    result = $"**_{user} used the belt on {user2}!_**";
                    break;
                case 4:
                    result = $"**_{user} refused to spit on {user2} during sex!_**";
                    break;
                case 5:
                    result = $"**_{user} whipped {user2}!_**";
                    break;
                case 6:
                    result = $"**_{user} chained {user2} up!_**";
                    break;
                case 7:
                    result = $"**_{user} gagged {user2} with their toes!_**";
                    break;
                case 8:
                    result = $"**_{user} pegged {user2}!_**";
                    break;
                case 9:
                    result = $"**_{user} ball gagged {user2}!_**";
                    break;
                case 10:
                    result = $"**_{user} cucked {user2}!_**";
                    break;
                case 11:
                    result = $"**_{user} force fed {user2} cheese!_**";
                    break;
                case 12:
                    result = $"**_{user} threw a spoon at {user2}'s forehead!_**";
                    break;
                case 13:
                    result = $"**_{user} sat on {user2}'s face until they passed out!_**";
                    break;
                case 14:
                    result = $"**_{user} shoved hard boiled eggs up {user2}'s ass whilst they were asleep... good luck in the morning!_**";
                    break;

                default:
                    result = $"**_{user} punished {user2}!_**";
                    break;
            }

            return result;
        }

    }
}
