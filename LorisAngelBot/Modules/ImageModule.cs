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

            string image = Images.DrawQuoteD(username, user.GetAvatarUrl(ImageFormat.Png, 128), DateTime.UtcNow, quote, Color.White, user.IsBot);
            await Context.Channel.SendFileAsync(image);
            File.Delete(image);
        }
    }

    public class Images
    {
        public static string DrawQuote(string username, string image, DateTime time, string quote, Color nameColour, bool isBot = false)
        {
            Random rnd = new Random();

            // Setup
            string path = Path.Combine(AppContext.BaseDirectory, "images", "quotes", $"{rnd.Next(10000, 99999)}_{time.Hour}{time.Minute}.jpg");
            Color backgroundColour = Color.FromArgb(53, 57, 63);
            Color timeColour = Color.FromArgb(113,118,124);
            Color textColour = Color.FromArgb(220, 221, 222);
            int width = 500;
            int height = 100;

            // Create image
            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
            graphicImage.FillRectangle(new SolidBrush(backgroundColour), new Rectangle(0, 0, width, height));

            // Draw profile pic
            string imagePath = Path.Combine(AppContext.BaseDirectory, $"images/{rnd.Next(10000, 99999)}.png");
            using (WebClient client = new WebClient()) client.DownloadFile(image, imagePath);
            Bitmap profilePic = new Bitmap(imagePath);
            graphicImage.DrawImage(profilePic, 5, 25, 50, 50);
            profilePic.Dispose();
            File.Delete(imagePath);

            Bitmap profilePicOverlay = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"images/quotes/overlay_profile.png"));
            graphicImage.DrawImage(profilePicOverlay, 5, 25, 50, 50);
            profilePicOverlay.Dispose();

            // Text
            graphicImage.DrawString(username, new Font(new FontFamily("Catamaran"), 15, FontStyle.Bold), new SolidBrush(nameColour), 75, 20);
            using (Graphics graphics = Graphics.FromImage(new Bitmap(1, 1)))
            {
                float dateX = graphics.MeasureString(username, new Font(new FontFamily("Catamaran"), 15, FontStyle.Bold)).Width + 85;

                if (isBot)
                {
                    Bitmap botBadge = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"images/quotes/badge_bot.png"));
                    graphicImage.DrawImage(botBadge, dateX-3, 27, 27, 15);
                    botBadge.Dispose();
                    dateX += 30;
                }

                string hour = time.Hour.ToString();
                if (time.Hour < 10) hour = $"0{hour}";
                string min = time.Minute.ToString();
                if (time.Minute < 10) min = $"0{min}";

                graphicImage.DrawString($"Today at {hour}:{min}", new Font(new FontFamily("Catamaran Thin"), 11, FontStyle.Regular), new SolidBrush(timeColour), dateX, 25);
            }

            graphicImage.DrawString(quote, new Font(new FontFamily("Catamaran Thin"), 14, FontStyle.Bold), new SolidBrush(textColour), 75, 50);

            editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            graphicImage.Dispose();
            editedBitmap.Dispose();
            return path;
        }

        public static string DrawQuoteD(string username, string image, DateTime time, string quote, Color nameColour, bool isBot = false)
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
    }
}
