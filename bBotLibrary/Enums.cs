﻿using System;
using System.Reflection;

namespace Discord.Net.Bot
{
    public enum Emoji
    {
        [StringValue("❤️")] RED_HEART,
        [StringValue("💚")] GREEN_HEART,
        [StringValue("💛")] YELLOW_HEART,
        [StringValue("💙")] BLUE_HEART,
        [StringValue("🧡")] ORANGE_HEART,
        [StringValue("💜")] PURPLE_HEART,
        [StringValue("💖")] SPARKLING_HEART,
        [StringValue("💘")] CUPID_HEART,
        [StringValue("😊")] SMILE,
        [StringValue("🤪")] WILD,
        [StringValue("🥰")] HEARTS,
        [StringValue("😇")] HALO,
        [StringValue("😍")] HEARTEYES,
        [StringValue("😀")] GRIN,
        [StringValue("❄️")] SNOWFLAKE,
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
        [StringValue("💘")] CUPID,
    }

    public enum SadEmoji
    {
        [StringValue("🤔")] THINKING,
        [StringValue("🤨")] RAISED_EYEBROW,
        [StringValue("😐")] NEUTRAL,
        [StringValue("😑")] STRAIGHT,
        [StringValue("😶")] SPEECHLESS,
        [StringValue("😒")] UNAMUSED,
        [StringValue("😬")] GRIMACING,
        [StringValue("🤥")] LYING,
        [StringValue("😔")] SAD,
        [StringValue("🤒")] SICK,
    }

    public enum ConfigType
    {
        Solo, Individual
    }
    public enum CommandCategory
    {
        Main, Fun, Games, Leaderboards, Moderation, Help, BotRelated, User, Tools, NSFW,
        Currency, Gaming, Server, Region, Statistics, Achievements, Badges
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
