using System.Linq;
using System.Threading.Tasks;

namespace Discord.Net.Bot.Utility
{
    public class Channel
    {
        // Get the first message in a channel
        public static async Task<IMessage> GetFirstMessageAsync(IMessageChannel channel) => (await channel.GetMessagesAsync(0, Direction.After, 1).FlattenAsync()).FirstOrDefault();

        // Get the last message in a channel
        public static async Task<IMessage> GetLastMessageAsync(IMessageChannel channel) => (await channel.GetMessagesAsync(1).FlattenAsync()).FirstOrDefault();
    }
}
