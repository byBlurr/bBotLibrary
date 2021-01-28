using System;

namespace Discord.Net.Bot
{
    public class EmojiUtil
    {
        /// Get a random emoji from the emojis enum (not all added because im lazy)
        public static string GetRandomEmoji()
        {
            Array values = Enum.GetValues(typeof(Emoji));
            Random random = new Random();
            Emoji randomEmoji = (Emoji)values.GetValue(random.Next(values.Length));

            return EnumUtil.GetString(randomEmoji);
        }

        public static string GetRandomHeartEmoji()
        {
            Array values = Enum.GetValues(typeof(HeartEmoji));
            Random random = new Random();
            HeartEmoji randomEmoji = (HeartEmoji)values.GetValue(random.Next(values.Length));

            return EnumUtil.GetString(randomEmoji);
        }

        public static string GetRandomSadEmoji()
        {
            Array values = Enum.GetValues(typeof(SadEmoji));
            Random random = new Random();
            SadEmoji randomEmoji = (SadEmoji)values.GetValue(random.Next(values.Length));

            return EnumUtil.GetString(randomEmoji);
        }
    }
}
