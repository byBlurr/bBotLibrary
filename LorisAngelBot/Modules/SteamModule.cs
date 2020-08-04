using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using DragonFruit.Link;
using DragonFruit.Link.User.Extensions;
using DragonFruit.Link.User.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class SteamModule : ModuleBase
    {
        [Command("steam link")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SteamLinkAsync(string steamUrl)
        {
            await Context.Message.DeleteAsync();

            SteamFile steam = SteamFile.Load();
            foreach (LinkedUser user in steam.LinkedProfiles)
            {
                if (user.DiscordId == Context.User.Id)
                {
                    EmbedBuilder failEmbed = new EmbedBuilder()
                    {
                        Title = "Error linking Steam",
                        Description = $"You have already linked your steam.\nYou can use `{CommandHandler.GetPrefix(Context.Guild.Id)}steam unlink` to unlink your current account.",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Steam commands are still in development along side Link." }
                    };
                    await Context.Channel.SendMessageAsync(null, false, failEmbed.Build());

                    return;
                }
            }

            LinkedUser newUser = new LinkedUser()
            {
                DiscordId = Context.User.Id,
                SteamId = SteamApiHandler.GetSteamId(steamUrl)
            };
            steam.LinkedProfiles.Add(newUser);
            steam.Save();

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Successfully Linked",
                Description = $"You successfully linked your steam account",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Steam commands are still in development along side Link." }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("steam unlink")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SteamUnlinkAsync()
        {
            SteamFile steam = SteamFile.Load();
            foreach (LinkedUser user in steam.LinkedProfiles)
            {
                if (user.DiscordId == Context.User.Id)
                {
                    steam.LinkedProfiles.Remove(user);
                    steam.Save();
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "Unlinked Steam",
                        Description = $"Steam profile unlinked from user.",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Steam commands are still in development along side Link." }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());

                    return;
                }
            }
        }

        [Command("steam")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SteamProfileAsync()
        {
            await Context.Message.DeleteAsync();

            SteamFile steam = SteamFile.Load();
            bool found = false;
            foreach (LinkedUser user in steam.LinkedProfiles)
            {
                if (user.DiscordId == Context.User.Id)
                {
                    found = true;
                    var profile = SteamApiHandler.GetSteamProfile(user.SteamId);
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = profile.Name + " on Steam",
                        Description = $"Currently {profile.OnlineState.ToString()}",
                        Color = Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Steam commands are still in development along side Link." }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());
                }
            }

            if (!found)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "Unlinked Steam Account",
                    Description = $"Link your steam account with `{CommandHandler.GetPrefix(Context.Guild.Id)}steam link <profile_url>`",
                    Color = Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Steam commands are still in development along side Link." }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            
        }

        [Command("playtime")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SteamPlaytimeAsync(string game)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Still in Development!",
                Description = $"Steam Link Integration is still being developed.\n\nHelp support the development: {Util.DonateUrl}",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}" }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

        [Command("find game")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        private async Task SteamFindGameAsync(string users)
        {
            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Still in Development!",
                Description = $"Steam Link Integration is still being developed.\n\nHelp support the development: {Util.DonateUrl}",
                Color = Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}" }
            };
            await Context.Channel.SendMessageAsync(null, false, embed.Build());
        }

    }

    public class SteamApiHandler
    {
        private static SteamApiClient SteamClient;
        public static void SetupClient(string api_key)
        {
            SteamClient = new SteamApiClient(api_key);
        }

        public static ulong GetSteamId(string profileUrl) => SteamClient.GetUserProfile(profileUrl).Id;
        public static SteamUserProfile GetSteamProfile(ulong steamId) => SteamClient.GetUserProfile(steamId);
    }

    public class SteamFile
    {
        [JsonIgnore]
        static readonly string Dir = Path.Combine(AppContext.BaseDirectory, "config");

        [JsonIgnore]
        static readonly string Filename = "steam.json";

        public string API_KEY { get; set; }

        public List<LinkedUser> LinkedProfiles { get; set; }

        public SteamFile()
        {
            API_KEY = "";
            LinkedProfiles = new List<LinkedUser>();
        }

        public static bool Exists()
        {
            if (!Directory.Exists(Dir)) Directory.CreateDirectory(Dir);
            if (!File.Exists(Path.Combine(Dir, Filename)))
            {
                SteamFile file = new SteamFile();
                file.Save();
            }

            return File.Exists(Path.Combine(Dir, Filename));
        }

        public void Save() => File.WriteAllText(Path.Combine(Dir, Filename), ToJson());

        public static SteamFile Load()
        {
            return JsonConvert.DeserializeObject<SteamFile>(File.ReadAllText(Path.Combine(Dir, Filename)));
        }

        public string ToJson() => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class LinkedUser
    {
        public ulong DiscordId { get; set; }
        public ulong SteamId { get; set; }
    }

    
}
