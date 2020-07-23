using Discord.Commands;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System;
using System.Net;
using System.Collections.Generic;
using Discord;
using Discord.Net.Bot;

namespace LorisAngelBot.Modules
{

    /// TODO:
    ///     Fix overlapping snakes and ladders


    public class SnakesModule : ModuleBase
    {
        [Command("snake")]
        private async Task StartSnakeAsync(IUser P2 = null, IUser P3 = null, IUser P4 = null)
        {
            await Context.Message.DeleteAsync();
            // Lock command for donators only...
            // Until further testing completed

            int width = 10;
            int height = 8;

            /**
            if (width < 5) width = 5;
            else if (width > 20) width = 20;

            if (height < 5) height = 5;
            else if (height > 20) height = 20;
            **/

            Player[] players = new Player[4];
            players[0] = new Player(Context.User.Username, Context.User.Id, Context.User.GetAvatarUrl(), 0, 0);
            if (P2 != null && !P2.IsBot)
            {
                players[1] = new Player(P2.Username, P2.Id, P2.GetAvatarUrl(), 0, 0);
                if (P3 != null && !P3.IsBot) players[2] = new Player(P3.Username, P3.Id, P3.GetAvatarUrl(), 0, 0);
                {
                    if (P4 != null && !P4.IsBot) players[3] = new Player(P4.Username, P4.Id, P4.GetAvatarUrl(), 0, 0);
                }
            }

            IUserMessage message = await Context.Channel.SendMessageAsync("Starting new game...");
            Board board = new Board(players, width, height);
            bool canStart = SnakeGames.StartNewGame(board, Context.Guild.Id, message);

            if (canStart)
            {
                string path = Path.Combine(AppContext.BaseDirectory, $"snakeladders/textures/{Context.Guild.Id}_board.jpg");
                await board.DrawBlankBoardAsync(Context.Guild.Id);
                await message.DeleteAsync();
                IUserMessage msg = await Context.Channel.SendFileAsync(path, $"**Snakes and Ladders**\nNext Up: **{board.Players[board.NextPlayer].Name}**\n`{CommandHandler.GetPrefix(Context.Guild.Id)}snake r` to roll\n`{CommandHandler.GetPrefix(Context.Guild.Id)}snake e` to end the game\n\n(Please note that Snakes and Ladders is still beta and you may experience issues.)");
                SnakeGames.UpdateGame(board, Context.Guild.Id, msg);
            }
            else
            {
                await message.ModifyAsync(x => x.Content = "Unable to start a new game. One is already running in this server.");
            }
        }

        [Command("snake roll")]
        [Alias("snake r")]
        private async Task SnakeRollAsync()
        {
            await Context.Message.DeleteAsync();
            SnakeGame game = SnakeGames.GetGame(Context.Guild.Id);

            if (game != null)
            {
                Random rnd = new Random();
                int dice = rnd.Next(1, 7);

                for (int p = 0; p < game.Board.Players.Length; p++)
                {
                    if (game.Board.Players[p] != null)
                    {
                        if (game.Board.Players[p].Id == Context.User.Id && p == game.Board.NextPlayer)
                        {
                            // Rewritten move code
                            if (game.Board.Players[p].Y % 2 != 0)
                                game.Board.Players[p].X -= dice;
                            else
                                game.Board.Players[p].X += dice;

                            if (game.Board.Players[p].X < 0)
                            {
                                game.Board.Players[p].X = (-game.Board.Players[p].X) - 1;

                                if (game.Board.Players[p].Y < game.Board.SnakesBoard.GetLength(1) - 1)
                                    game.Board.Players[p].Y++;
                                else
                                    game.Board.Players[p].X = 0;
                            }
                            else if(game.Board.Players[p].X >= game.Board.SnakesBoard.GetLength(0))
                            {
                                game.Board.Players[p].X = (game.Board.SnakesBoard.GetLength(0) - 1) - (game.Board.Players[p].X - game.Board.SnakesBoard.GetLength(0));

                                if (game.Board.Players[p].Y < game.Board.SnakesBoard.GetLength(1) - 1)
                                    game.Board.Players[p].Y++;
                                else
                                    game.Board.Players[p].X = game.Board.SnakesBoard.GetLength(0) - 1;
                            }


                            // Check where we landed
                            var tileState = game.Board.SnakesBoard[game.Board.Players[p].X, game.Board.Players[p].Y].State;
                            if (tileState == TileState.SNAKE_TOP)
                            {
                                while (game.Board.SnakesBoard[game.Board.Players[p].X, game.Board.Players[p].Y].State != TileState.SNAKE_BOTTOM)
                                {
                                    game.Board.Players[p].Y--;
                                }
                            }
                            else if (tileState == TileState.LADDER_BOTTOM)
                            {
                                while (game.Board.SnakesBoard[game.Board.Players[p].X, game.Board.Players[p].Y].State != TileState.LADDER_TOP)
                                {
                                    game.Board.Players[p].Y++;
                                }
                            }

                            await game.Message.DeleteAsync();

                            string path = Path.Combine(AppContext.BaseDirectory, $"snakeladders/textures/{Context.Guild.Id}_board.jpg");
                            await game.Board.DrawBlankBoardAsync(Context.Guild.Id);

                            int X = game.Board.Players[p].X;
                            int Y = game.Board.Players[p].Y;

                            if (dice != 6) game.Board.UpdateNextPlayer();

                            var user = await Context.Guild.GetUserAsync(game.Board.Players[game.Board.NextPlayer].Id);
                            var message = await Context.Channel.SendFileAsync(path, $"**Snakes and Ladders**\nRolled: **{dice}**\nNext Up: **{user.Mention}**\n`{CommandHandler.GetPrefix(Context.Guild.Id)}snake r` to roll\n`{CommandHandler.GetPrefix(Context.Guild.Id)}snake e` to end the game\n\n(Please note that Snakes and Ladders is still beta and you may experience issues.)");
                            SnakeGames.UpdateGame(game.Board, Context.Guild.Id, message);

                            if (game.Board.SnakesBoard[game.Board.Players[p].X, game.Board.Players[p].Y].State == TileState.END)
                            {
                                // We has a winner!
                                SnakeGames.FinishGame(Context.Guild.Id);

                                EmbedBuilder embed = new EmbedBuilder()
                                {
                                    Title = "Snakes and Ladders",
                                    Description = $"**{Context.User.Username} has WON!**",
                                    Color = Discord.Color.DarkPurple,
                                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  I hope you enjoyed!" }
                                };

                                await Context.Channel.SendMessageAsync(null, false, embed.Build());
                            }
                        }
                    }
                }
            }
        }

        [Command("snake end")]
        [Alias("snake e")]
        private async Task SnakeEndAsync()
        {
            await Context.Message.DeleteAsync();
            SnakeGame game = SnakeGames.GetGame(Context.Guild.Id);
            string description = "It appears you aren't part of the game!";

            if (game != null)
            {
                for (int i = 0; i < game.Board.Players.Length; i++)
                {
                    if (game.Board.Players[i] != null)
                    {
                        if (game.Board.Players[i].Id == Context.User.Id)
                        {
                            SnakeGames.FinishGame(Context.Guild.Id);
                            description = "Successfully ended the game!";
                        }
                    }
                }
            }
            else
            {
                description = "It appears there isn't a game in this guild!";
            }

            EmbedBuilder embed = new EmbedBuilder()
            {
                Title = "Snakes and Ladders",
                Description = description,
                Color = Discord.Color.DarkPurple,
                Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
            };

            await Context.Channel.SendMessageAsync(null, false, embed.Build());

        }

    }

    public class SnakeGames
    {
        public static List<SnakeGame> Games = new List<SnakeGame>();

        public static bool StartNewGame(Board board, ulong guild, IUserMessage message)
        {
            SnakeGame game = new SnakeGame(board, guild, message);
            if (Games.Contains(game)) return false;

            Games.Add(game);
            return true;
        }

        public static SnakeGame GetGame(ulong guild)
        {
            foreach (SnakeGame game in Games)
            {
                if (game.Guild == guild)
                    return game;
            }
            return null;
        }

        public static void UpdateGame(Board board, ulong guild, IUserMessage message)
        {
            foreach (SnakeGame game in Games)
            {
                if (game.Guild == guild)
                {
                    Games.Remove(game);
                    Games.Add(new SnakeGame(board, guild, message));
                }
            }
        }

        public static void FinishGame(ulong guild)
        {
            foreach (SnakeGame game in Games)
            {
                if (game.Guild == guild)
                {
                    Games.Remove(game);
                    return;
                }
            }
        }

    }

    public class SnakeGame
    {
        public SnakeGame(Board board, ulong guild, IUserMessage message)
        {
            Board = board;
            Guild = guild;
            Message = message;
        }

        public Board Board { get; set; }

        public ulong Guild { get; set; }

        public IUserMessage Message { get; set; }


        public override bool Equals(object obj)
        {
            return obj is SnakeGame game &&
                   Guild == game.Guild;
        }
    }

    public class Board
    {
        public Tile[,] SnakesBoard;

        public Player[] Players;
        public int NextPlayer = 0;

        public Board(Player[] players, int width = 10, int height = 10, int ladders = 6, int snakes = 6)
        {
            SnakesBoard = new Tile[width, height];
            Players = players;
            NextPlayer = 0;

            Random rnd = new Random();

            // Generate base board
            int id = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    TileState state = TileState.BLANK;
                    SnakesBoard[x, y] = new Tile(x, y, state, id);
                    id++;
                }
            }

            // Add start and end
            SnakesBoard[0, 0] = new Tile(0, 0, TileState.START, 0);
            if (height % 2 == 0)
                SnakesBoard[0, height - 1] = new Tile(0, height - 1, TileState.END, 0);
            else
                SnakesBoard[width - 1, height - 1] = new Tile(width - 1, height - 1, TileState.END, 0);

            GenerateLadders(width, height, ladders, rnd);
            GenerateSnakes(width, height, snakes, rnd);
        }

        private void GenerateLadders(int width, int height, int ladders, Random rnd)
        {
            for (int l = 0; l < ladders; l++)
            {
                bool found = false;
                while (!found)
                {
                    int size = rnd.Next(2, 5);
                    int x = rnd.Next(0, width);
                    int y = rnd.Next(1, height);

                    if (y - size < 0) size = y;

                    found = true;
                    for (int ls = 0; ls < size; ls++)
                    {
                        if (SnakesBoard[x, y - ls].State != TileState.BLANK) found = false;
                    }

                    if (found == true)
                    {
                        for (int ls = 1; ls < size; ls++)
                        {
                            SnakesBoard[x, y - ls] = new Tile(x, y - ls, TileState.LADDER, 0);
                        }
                        SnakesBoard[x, y] = new Tile(x, y, TileState.LADDER_TOP, 0);
                        SnakesBoard[x, y - size] = new Tile(x, y - size, TileState.LADDER_BOTTOM, 0);
                    }
                }
            }
        }

        private void GenerateSnakes(int width, int height, int snakes, Random rnd)
        {
            for (int s = 0; s < snakes; s++)
            {
                bool found = false;
                while (!found)
                {
                    int size = rnd.Next(2, 5);
                    int x = rnd.Next(0, width);
                    int y = rnd.Next(1, height);

                    if (y - size < 0) size = y;
                    found = true;

                    for (int ss = 0; ss < size; ss++)
                    {
                        if (SnakesBoard[x, y - ss].State != TileState.BLANK) found = false;
                    }

                    if (found == true)
                    {
                        for (int ss = 1; ss < size; ss++)
                        {
                            SnakesBoard[x, y - ss] = new Tile(x, y - ss, TileState.SNAKE, 0);
                        }
                        SnakesBoard[x, y] = new Tile(x, y, TileState.SNAKE_TOP, 0);
                        SnakesBoard[x, y - size] = new Tile(x, y - size, TileState.SNAKE_BOTTOM, 0);
                    }
                }
            }
        }

        public void UpdateNextPlayer()
        {
            NextPlayer++;
            if (NextPlayer >= Players.Length || Players[NextPlayer] == null) NextPlayer = 0;
        }

        public string GetBoardAsString()
        {
            string board = "";

            for (int y = SnakesBoard.GetLength(1)-1; y >= 0 ; y--)
            {
                board = $"{board}| ";
                for (int x = 0; x < SnakesBoard.GetLength(0); x++)
                {
                    string tile = "--";
                    switch (SnakesBoard[x, y].State)
                    {
                        case TileState.BLANK:
                            tile = "BL";
                            break;
                        case TileState.START:
                            tile = "S>";
                            break;
                        case TileState.END:
                            tile = "E<";
                            break;
                        case TileState.LADDER:
                            tile = "LA";
                            break;
                        case TileState.LADDER_BOTTOM:
                            tile = "LB";
                            break;
                        case TileState.LADDER_TOP:
                            tile = "LT";
                            break;
                        case TileState.SNAKE:
                            tile = "SN";
                            break;
                        case TileState.SNAKE_BOTTOM:
                            tile = "SB";
                            break;
                        case TileState.SNAKE_TOP:
                            tile = "ST";
                            break;
                    }

                    board = $"{board}[{tile}] ";
                }
                board = $"{board}|\n";
            }

            return board;
        }

        public async Task DrawBlankBoardAsync(ulong id)
        {
            try
            {
                int tileSize = 32;
                int width = SnakesBoard.GetLength(0) * tileSize;
                int height = SnakesBoard.GetLength(1) * tileSize;

                Bitmap editedBitmap = new Bitmap(width, height);
                Graphics graphicImage = Graphics.FromImage(editedBitmap);

                graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
                graphicImage.FillRectangle(Brushes.Magenta, new Rectangle(0, 0, width, height));

                // Draw board
                for (int y = SnakesBoard.GetLength(1) - 1; y >= 0; y--)
                {
                    for (int x = 0; x < SnakesBoard.GetLength(0); x++)
                    {
                        string texture = Path.Combine(AppContext.BaseDirectory, $"snakeladders/textures/tile_{SnakesBoard[x, y].State.ToString().ToLower()}.png");
                        Bitmap loadedTexture = new Bitmap(texture);

                        int truey = (SnakesBoard.GetLength(1) - y) - 1;
                        graphicImage.DrawImage(loadedTexture, x * tileSize, truey * tileSize, tileSize, tileSize);
                        loadedTexture.Dispose();
                    }
                }

                // Draw players
                for (int i = 0; i < Players.Length; i++)
                {
                    if (Players[i] != null)
                    {
                        Player player = Players[i];

                        string download = player.Image;
                        string imagePath = Path.Combine(AppContext.BaseDirectory, $"snakeladders/{player.Name}");
                        using (WebClient client = new WebClient()) client.DownloadFile(new Uri(download), imagePath);
                        Bitmap loadedTexture = new Bitmap(imagePath);

                        int truey = (SnakesBoard.GetLength(1) - player.Y) - 1;
                        graphicImage.DrawImage(loadedTexture, (player.X * tileSize) + ((tileSize / 6) * i), (truey * tileSize) + ((tileSize / 6) * i), tileSize / 2, tileSize / 2);
                        loadedTexture.Dispose();
                        File.Delete(imagePath);
                    }
                }

                string path = Path.Combine(AppContext.BaseDirectory, $"snakeladders/textures/{id}_board.jpg");
                editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

                graphicImage.Dispose();
                editedBitmap.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class Player
    {
        public Player(string name, ulong id, string image, int posX, int posY)
        {
            Name = name;
            Id = id;
            Image = image;
            X = posX;
            Y = posY;
        }

        public string Name { get; set; }

        public string Image { get; set; }

        public int X { get; set; }

        public int Y { get; set; }

        public ulong Id { get; set; }

    }

    public class Tile
    {
        public Tile(int x, int y, TileState state, int id)
        {
            X = x;
            Y = y;
            State = state;
            Id = id;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public TileState State { get; set; }

        public int Id { get; set; }


    }

    public enum TileState
    {
        START, 
        END, 
        BLANK, 
        LADDER, 
        LADDER_BOTTOM, 
        LADDER_TOP, 
        SNAKE, 
        SNAKE_BOTTOM, 
        SNAKE_TOP
    }
}
