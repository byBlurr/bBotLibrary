using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;
using Discord.Net.Bot.Database.Configs;
using System.Collections.Generic;
using System.Diagnostics;

namespace Discord.Net.Bot
{
    public abstract class CommandHandler
    {
        private static ConfigType configType;

        private CommandService commands;
        protected static DiscordSocketClient bot;
        private IServiceProvider map;
        public int RestartEveryMs = 60000 * 6;

        public void SetUp(IServiceProvider provider, ConfigType ctype)
        {
            configType = ctype;

            map = provider;
            bot = map.GetService<DiscordSocketClient>();
            commands = map.GetService<CommandService>();

            bot.Ready += CheckConfigsAsync;
            bot.JoinedGuild += JoinGuildAsync;
            bot.MessageReceived += HandleCommandAsync;
            bot.MessageUpdated += HandleEditCommandAsync;
            bot.MessagesBulkDeleted += BulkDeleteAsync;
            SetupHandlers(bot);

            BotConfig conf = BotConfig.Load();
            RegisterCommands(conf.Commands);
            conf.Save();
        }

        private async Task BulkDeleteAsync(IReadOnlyCollection<Cacheable<IMessage, ulong>> messages, ISocketMessageChannel channel)
        {
            // Trying to get rid of the bulk delete warning
            await Task.CompletedTask;
        }

        private async Task JoinGuildAsync(SocketGuild guild)
        {
            if (configType == ConfigType.Individual)
            {
                BotConfig conf = BotConfig.Load();

                IndividualConfig gconf = conf.GetConfig(guild.Id);
                if (gconf == null)
                {
                    gconf = conf.FreshConfig(guild.Id);
                    conf.Configs.Add(gconf);
                    conf.Save();
                }
            }
        }

        private async Task CheckConfigsAsync()
        {
            BotConfig conf = BotConfig.Load();
            if (configType == ConfigType.Individual)
            {
                foreach (var guild in GetBot().Guilds)
                {
                    IndividualConfig gconf = conf.GetConfig(guild.Id);
                    if (gconf == null)
                    {
                        gconf = conf.FreshConfig(guild.Id);
                        conf.Configs.Add(gconf);
                    }
                }
            }

            await Util.Logger(new LogMessage(LogSeverity.Info, "Gateway", $"Successfully connected to {bot.Guilds.Count} guilds"));
            conf.LastStartup = DateTime.UtcNow;
            conf.Save();

            var restart = Task.Run(async () =>
            {
                await Task.Delay(RestartEveryMs);

                // Code to restart bot
                Process.Start("D:\\Coding and Development\\bProjects\\bBotLibrary\\LorisAngelBot\\Current\\LorisAngelBot.exe");
                // Close this instance
                Environment.Exit(0);
            });
        }

        public static string GetPrefix(ulong id = 0L)
        {
            BotConfig conf = BotConfig.Load();
            if (configType == ConfigType.Individual)
                return conf.GetConfig(id).Prefix;
            else
                return conf.SoloConfig.Prefix;
        }

        public virtual void SetupHandlers(DiscordSocketClient bot) { }
        public virtual void RegisterCommands(List<BotCommand> commands) { }

        private async Task HandleEditCommandAsync(Cacheable<IMessage, ulong> msg, SocketMessage pMsg, ISocketMessageChannel channel)
        {
            if (pMsg.Author.IsBot) return;
            var cmd = Task.Run(async () =>
            {
                SocketUserMessage message = pMsg as SocketUserMessage;
                if (message == null) return;
                var context = new SocketCommandContext(bot, message);

                string prefix = "";
                BotConfig config = BotConfig.Load();
                if (configType == ConfigType.Solo)
                    prefix = config.SoloConfig.Prefix;
                else
                    prefix = config.GetConfig((message.Channel as IGuildChannel).GuildId).Prefix;

                if (message.Content.StartsWith(bot.CurrentUser.Mention) && message.Content.Length == bot.CurrentUser.Mention.Length)
                {
                    await context.Channel.SendMessageAsync($"Try `{prefix}help`");
                    return;
                }

                int argPos = 0;
                if (message.HasStringPrefix(prefix, ref argPos) || message.HasMentionPrefix(bot.CurrentUser, ref argPos))
                {
                    var result = await commands.ExecuteAsync(context, argPos, map);
                    if (!result.IsSuccess && result.ErrorReason != "Unknown command.")
                    {
                        if (result.ErrorReason.Contains("Collection was modified")) return;

                        await Util.Logger(new LogMessage(LogSeverity.Warning, "Commands", result.ErrorReason, null));

                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Title = "Command Error",
                            Description = result.ErrorReason,
                            Color = Color.DarkRed
                        };

                        await context.Channel.SendMessageAsync(null, false, embed.Build());
                    }
                }
            });
        }

        private async Task HandleCommandAsync(SocketMessage pMsg)
        {
            if (pMsg.Author.IsBot) return;
            var cmd = Task.Run(async () =>
            {
                SocketUserMessage message = pMsg as SocketUserMessage;
                if (message == null) return;
                var context = new SocketCommandContext(bot, message);

                string prefix = "";
                BotConfig config = BotConfig.Load();
                if (configType == ConfigType.Solo)
                    prefix = config.SoloConfig.Prefix;
                else
                    prefix = config.GetConfig((message.Channel as IGuildChannel).GuildId).Prefix;

                if (message.Content.StartsWith(bot.CurrentUser.Mention) && message.Content.Length == bot.CurrentUser.Mention.Length)
                {
                    await context.Channel.SendMessageAsync($"Try `{prefix}help`");
                    return;
                }

                int argPos = 0;
                if (message.HasStringPrefix(prefix, ref argPos) || message.HasMentionPrefix(bot.CurrentUser, ref argPos))
                {
                    var result = await commands.ExecuteAsync(context, argPos, map);
                    if (!result.IsSuccess && result.ErrorReason != "Unknown command.")
                    {
                        if (result.ErrorReason.Contains("Collection was modified")) return;

                        await Util.Logger(new LogMessage(LogSeverity.Warning, "Commands", result.ErrorReason, null));

                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Title = "Command Error",
                            Description = result.ErrorReason,
                            Color = Color.DarkRed
                        };

                        await context.Channel.SendMessageAsync(null, false, embed.Build());
                    }
                }

            });
        }

        public async Task ConfigureAsync() { await commands.AddModulesAsync(Assembly.GetEntryAssembly(), map); }

        public static DiscordSocketClient GetBot() => bot;
    }
}
