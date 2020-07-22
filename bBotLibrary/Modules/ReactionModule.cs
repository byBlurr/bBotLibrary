using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Discord.Net.Bot.Modules
{
    public class ReactionModule
    {
        private static List<MessageReactions> Reactions = new List<MessageReactions>();

        public static MessageReactions GetMessageReactions(ulong messageId)
        {
            foreach (var r in Reactions) if (r.MessageId == messageId) return r;

            return null;
        }

        public static async Task ReactionAddedAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            MessageReactions msg = null;

            foreach (var r in Reactions)
            {
                if (r.MessageId == message.Id) msg = r;
            }
            if (msg == null) msg = new MessageReactions(message.Id);

            msg.Reactions.Add(new MessageReaction(message.Value.Author.Id, reaction.Emote));
            Console.WriteLine("Added");
        }

        public static async Task ReactionRemovedAsync(Cacheable<IUserMessage, ulong> message, ISocketMessageChannel channel, SocketReaction reaction)
        {
            throw new NotImplementedException();
        }

        public static Task ReactionsClearedAsync(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {
            throw new NotImplementedException();
        }
    }

    public class MessageReactions
    {
        public ulong MessageId { get; set; }
        public List<MessageReaction> Reactions { get; set; }

        public MessageReactions(ulong message)
        {
            MessageId = message;
            Reactions = new List<MessageReaction>();
        }

        public override bool Equals(object obj)
        {
            return obj is MessageReactions reactions &&
                   MessageId == reactions.MessageId;
        }
    }

    public class MessageReaction
    {
        public ulong User { get; set; }
        public IEmote Reaction { get; set; }

        public MessageReaction(ulong user, IEmote emote)
        {
            User = user;
            Reaction = emote;
        }
    }
}
