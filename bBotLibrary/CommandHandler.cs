using System.Threading.Tasks;
using System.Reflection;
using Discord.Commands;
using Discord.WebSocket;
using System;
using Microsoft.Extensions.DependencyInjection;
using Discord.Net.Bot.Database.Configs;

namespace Discord.Net.Bot
{
    public abstract class CommandHandler
    {
        private ConfigType configType;

        private CommandService commands;
        protected static DiscordSocketClient bot;
        private IServiceProvider map;
        public void SetUp(IServiceProvider provider, ConfigType ctype)
        {
            configType = ctype;

            map = provider;
            bot = map.GetService<DiscordSocketClient>();
            commands = map.GetService<CommandService>();

            bot.Ready += CheckConfigsAsync;
            bot.JoinedGuild += JoinGuildAsync;
            bot.MessageReceived += HandleCommandAsync;
            SetupHandlers(bot);
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
            if (configType == ConfigType.Individual)
            {
                BotConfig conf = BotConfig.Load();
                bool save = false;
                foreach (var guild in GetBot().Guilds)
                {
                    IndividualConfig gconf = conf.GetConfig(guild.Id);
                    if (gconf == null)
                    {
                        gconf = conf.FreshConfig(guild.Id);
                        conf.Configs.Add(gconf);
                        save = true;
                    }
                }

                if (save) conf.Save();
            }
        }

        public virtual void SetupHandlers(DiscordSocketClient bot) { }
        private async Task HandleCommandAsync(SocketMessage pMsg)
        {
            SocketUserMessage message = pMsg as SocketUserMessage;
            if (message == null) return;
            var context = new SocketCommandContext(bot, message);
            if (message.Author.IsBot) return;

            string prefix = "";
            BotConfig config = BotConfig.Load();
            if (configType == ConfigType.Solo)
                prefix = config.SoloConfig.Prefix;
            else
                prefix = config.GetConfig((message.Channel as IGuildChannel).GuildId).Prefix;

            int argPos = 0;
            if (message.HasStringPrefix(prefix, ref argPos))
            {
                var result = await commands.ExecuteAsync(context, argPos, map);
                if (!result.IsSuccess && result.ErrorReason != "Unknown command.") Console.WriteLine(result.ErrorReason);
            }
        }

        public async Task ConfigureAsync() { await commands.AddModulesAsync(Assembly.GetEntryAssembly(), map); }

        public static DiscordSocketClient GetBot() => bot;
    }
}
