using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Discord.Net.Bot
{
    public enum Emoji
    {
        [StringValue("❤️")] HEART,
        [StringValue("💘")] CUPID,
        [StringValue("😊")] SMILE,
        [StringValue("🤪")] WILD,
        [StringValue("🥰")] HEARTS,
        [StringValue("😇")] HALO,
        [StringValue("😍")] HEARTEYES,
        [StringValue("😀")] GRIN,
    }

    public enum HeartEmoji
    {
        [StringValue("❤️")] RED,
        [StringValue("💚")] GREEN,
        [StringValue("💛")] YELLOW,
        [StringValue("💙")] BLUE,
        [StringValue("🧡")] ORANGE,
        [StringValue("💜")] PURPLE,
        [StringValue("💖")] SPARKLING,
    }

    public enum CommandCategory
    {
        Games, Moderation, Help, BotRelated
    }

    public class StringValue : Attribute
    {
        private readonly string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public static class EnumUtil
    {
        public static string GetString(Enum value)
        {
            string output = "";
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs =
               fi.GetCustomAttributes(typeof(StringValue),
                                       false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}
