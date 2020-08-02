using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace LorisAngelBot.Modules
{
    public class ImageModule : ModuleBase
    {
        [Command("quote")]
        private async Task QuoteAsync(IGuildUser user = null, [Remainder] string quote = "")
        {
            await Context.Message.DeleteAsync();

            if (user == null)
            {
                user = Context.User as IGuildUser;
                quote = "I should probably try using '-help quote'.";
            }

            string username = user.Username;
            if (user.Nickname != null) username = user.Nickname;

            IRole role = null;
            foreach (ulong roleid in user.RoleIds)
            {
                var r = Context.Guild.GetRole(roleid);
                if ((role == null || r.Position > role.Position) && !r.Color.ToString().Equals("#000000")) role = r;
            }

            Color roleColor = role != null ? Color.FromArgb(role.Color.R, role.Color.G, role.Color.B) : Color.White;
            string image = Images.DrawQuote(username, user.GetAvatarUrl(ImageFormat.Png, 128), DateTime.UtcNow, quote, roleColor, user.IsBot);
            await Context.Channel.SendFileAsync(image);
            File.Delete(image);
        }

        [Command("habbo")]
        private async Task HabboQuoteAsync(string username, [Remainder] string quote)
        {
            await Context.Message.DeleteAsync();
            string path = Images.DrawHabboQuote(username, quote);
            await Context.Channel.SendFileAsync(path);
            File.Delete(path);
        }
    }

    public class Images
    {
        public static string DrawQuote(string username, string image, DateTime time, string quote, Color nameColour, bool isBot = false)
        {
            Random rnd = new Random();

            // Get width
            int width = 0;
            int height = 0;
            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                width = (int)graphics.MeasureString(quote, new Font(new FontFamily("Catamaran"), 30, FontStyle.Bold)).Width + 20;
                height = (int)(graphics.MeasureString(quote, new Font(new FontFamily("Catamaran"), 30, FontStyle.Bold)).Height) + 120;
            }

            // Setup
            string path = Path.Combine(AppContext.BaseDirectory, "images", "quotes", $"{rnd.Next(10000, 99999)}_{time.Hour}{time.Minute}.jpg");
            Color backgroundColour = Color.FromArgb(53, 57, 63);
            Color timeColour = Color.FromArgb(113, 118, 124);
            Color textColour = Color.FromArgb(220, 221, 222);
            if (width < 1000) width = 1000;
            if (height < 200) height = 200;

            // Create image
            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
            graphicImage.FillRectangle(new SolidBrush(backgroundColour), new Rectangle(0, 0, width, height));

            // Draw profile pic
            string imagePath = Path.Combine(AppContext.BaseDirectory, $"images/{rnd.Next(10000, 99999)}.png");
            using (WebClient client = new WebClient()) client.DownloadFile(image, imagePath);
            Bitmap profilePic = new Bitmap(imagePath);
            graphicImage.DrawImage(profilePic, 10, 50, 100, 100);
            profilePic.Dispose();
            File.Delete(imagePath);

            Bitmap profilePicOverlay = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"images/quotes/overlay_profile.png"));
            graphicImage.DrawImage(profilePicOverlay, 10, 50, 100, 100);
            profilePicOverlay.Dispose();

            // Text
            graphicImage.DrawString(username, new Font(new FontFamily("Catamaran"), 30, FontStyle.Bold), new SolidBrush(nameColour), 150, 40);
            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                float dateX = graphics.MeasureString(username, new Font(new FontFamily("Catamaran"), 30, FontStyle.Bold)).Width + 170;

                if (isBot)
                {
                    Bitmap botBadge = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"images/quotes/badge_bot.png"));
                    graphicImage.DrawImage(botBadge, dateX - 6, 54, 54, 30);
                    botBadge.Dispose();
                    dateX += 60;
                }

                string hour = time.Hour.ToString();
                if (time.Hour < 10) hour = $"0{hour}";
                string min = time.Minute.ToString();
                if (time.Minute < 10) min = $"0{min}";

                graphicImage.DrawString($"Today at {hour}:{min}", new Font(new FontFamily("Catamaran Thin"), 22, FontStyle.Regular), new SolidBrush(timeColour), dateX, 50);
            }

            graphicImage.DrawString(quote, new Font(new FontFamily("Catamaran Thin"), 28, FontStyle.Bold), new SolidBrush(textColour), 150, 100);

            editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            graphicImage.Dispose();
            editedBitmap.Dispose();
            return path;
        }

        public static string DrawHabboQuote(string username, string quote)
        {

            Random rnd = new Random();
            string path = Path.Combine(AppContext.BaseDirectory, "images", "hquotes", $"{rnd.Next(10000, 99999)}.jpg");

            int width = 656;
            int height = 323;
            int x = rnd.Next(50, width - 350);
            int y = rnd.Next(50, height - 100);

            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;

            Bitmap backgroundBitmap = new Bitmap(Path.Combine(AppContext.BaseDirectory, "images", "hquotes", $"background.jpg"));
            graphicImage.DrawImage(backgroundBitmap, 0, 0, width, height);
            backgroundBitmap.Dispose();

            Bitmap startBitmap = new Bitmap(Path.Combine(AppContext.BaseDirectory, "images", "hquotes", $"box_start_0.png"));
            graphicImage.DrawImage(startBitmap, x, y, 31, 24);
            startBitmap.Dispose();

            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                int length1 = (int)graphics.MeasureString(quote, new Font(new FontFamily("Volter (Goldfish)"), 7, FontStyle.Regular)).Width;
                int length2 = (int)graphics.MeasureString(username + ": ", new Font(new FontFamily("Volter (Goldfish)"), 7, FontStyle.Bold)).Width + 4;
                int length = length1 + length2;

                Bitmap middleBitmap = new Bitmap(Path.Combine(AppContext.BaseDirectory, "images", "hquotes", $"box_middle_0.png"));
                for (int i = 0; i < length; i++)
                {
                    graphicImage.DrawImage(middleBitmap, 31 + i + x, y, 1, 24);
                }
                middleBitmap.Dispose();

                Bitmap endBitmap = new Bitmap(Path.Combine(AppContext.BaseDirectory, "images", "hquotes", $"box_end_0.png"));
                graphicImage.DrawImage(endBitmap, 31 + length + x, y, 9, 24);
                endBitmap.Dispose();

                Bitmap pointBitmap = new Bitmap(Path.Combine(AppContext.BaseDirectory, "images", "hquotes", $"box_point_0.png"));
                int pointX = rnd.Next(20, length - 20);
                graphicImage.DrawImage(pointBitmap, pointX + x, y + 23, 7, 5);
                pointBitmap.Dispose();

                graphicImage.DrawString(username + ": ", new Font(new FontFamily("Volter (Goldfish)"), 7, FontStyle.Bold), Brushes.Black, x + 31, y + 8);
                graphicImage.DrawString(quote, new Font(new FontFamily("Volter (Goldfish)"), 7, FontStyle.Regular), Brushes.Black, x + 31 + length2, y + 8);
            }


            editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            graphicImage.Dispose();
            editedBitmap.Dispose();
            return path;
        }
    }
}
