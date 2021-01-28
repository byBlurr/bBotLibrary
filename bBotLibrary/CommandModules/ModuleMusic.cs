using Discord.Audio;
using Discord.Commands;
using System.Threading.Tasks;

namespace Discord.Net.Bot.CommandModules
{
    public class ModuleMusic
    {
        public static async Task JoinChannelAsync(ICommandContext Context, IVoiceChannel channel = null)
        {
            channel = channel ?? (Context.User as IGuildUser).VoiceChannel;

            if (channel != null)
            {
                IAudioClient audioClient = await channel.ConnectAsync();
            }
            else await MessageUtil.SendErrorAsync((Context.Channel as ITextChannel), "Music Error", "Unable to find the channel to join?");
        }

        /**
         * Untested and Unused at the moment, will implement soon
         * 
        public static async Task PlayAudioAsync(IAudioClient client, string path)
        {
            // Create FFmpeg using the previous example
            using (var ffmpeg = CreateStream(path))
            using (var output = ffmpeg.StandardOutput.BaseStream)
            using (var discord = client.CreatePCMStream(AudioApplication.Mixed))
            {
                try { await output.CopyToAsync(discord); }
                finally { await discord.FlushAsync(); }
            }
        }

        private static Process CreateStream(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
                UseShellExecute = false,
                RedirectStandardOutput = true,
            });
        }
        **/
    }
}
