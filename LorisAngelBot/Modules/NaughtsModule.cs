using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class NaughtsModule : ModuleBase
    {
        [Command("naughts")]
        [Alias("ttt", "tictactoe", "tic tac toe")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task NaughtsAsync(IUser playerTwo)
        {
            await Context.Message.DeleteAsync();
            IUser playerOne = Context.User as IUser;

            if (playerTwo.IsBot)
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "TicTacToe | Naughts & Crosses",
                    Description = "You can not play against a bot!",
                    Color = Discord.Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {playerOne.Username}#{playerOne.Discriminator}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
                return;
            }

            if (NaughtsGames.GetBoard(Context.Guild.Id) == null)
            {
                var board = NaughtsGames.CreateNewGame(Context.Guild.Id, playerOne, playerTwo);
                if (board != null)
                {
                    var msg = await Context.Channel.SendFileAsync(board.DrawBoard(), $"**TicTacToe | Naughts & Crosses**\n" +
                        $"Next Up: **{board.Players[0].SelectedState.ToString().ToUpper()} ({board.Players[0].User.Mention})**\n" +
                        $"`{CommandHandler.GetPrefix(Context.Guild.Id)}t <x> <y>` to take your turn\n`{CommandHandler.GetPrefix(Context.Guild.Id)}t end` to end the game\n\n" +
                        $"(Please note that Naughts and Crosses is still beta and you may experience issues.)");

                    board.MessageId = msg.Id;
                    NaughtsGames.UpdateGame(Context.Guild.Id, board);
                }
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "TicTacToe | Naughts & Crosses",
                    Description = "There is already a game in this server!",
                    Color = Discord.Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {playerOne.Username}#{playerOne.Discriminator}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("t")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task NaughtsTurnAsync(int x = -1, int y = -1)
        {
            await Context.Message.DeleteAsync();

            NaughtsBoard board = NaughtsGames.GetBoard(Context.Guild.Id);
            if (board != null)
            {
                if (board.Players[board.CurrentPlayer].User.Id == Context.User.Id)
                {
                    if (x < 1 || x > board.Board.GetLength(0) || y < 1 || y > board.Board.GetLength(1))
                    {
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Title = "TicTacToe | Naughts & Crosses",
                            Description = $"Appears you tried to place your {board.Players[board.CurrentPlayer].SelectedState} outside the board.",
                            Color = Discord.Color.DarkPurple,
                            Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                        };
                        await Context.Channel.SendMessageAsync(null, false, embed.Build());
                    }
                    else if (board.Board[x-1, y-1].State != NaughtsTileState.Free)
                    {
                        EmbedBuilder embed = new EmbedBuilder()
                        {
                            Title = "TicTacToe | Naughts & Crosses",
                            Description = $"That tile is already taken!",
                            Color = Discord.Color.DarkPurple,
                            Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                        };
                        await Context.Channel.SendMessageAsync(null, false, embed.Build());
                    }
                    else
                    {
                        x = x - 1;
                        y = y - 1;

                        board.Board[x, y].State = board.Players[board.CurrentPlayer].SelectedState;
                        
                        NaughtsTileState winnerState = board.CheckForWinner();

                        if (winnerState == NaughtsTileState.Free)
                        {
                            // No winner
                            if (board.CurrentPlayer == 0) board.CurrentPlayer = 1;
                            else if (board.CurrentPlayer == 1) board.CurrentPlayer = 0;

                            var msg = await Context.Channel.SendFileAsync(board.DrawBoard(), $"**TicTacToe | Naughts & Crosses**\n" +
                                $"Next Up: **{board.Players[board.CurrentPlayer].SelectedState.ToString().ToUpper()} ({board.Players[board.CurrentPlayer].User.Mention})**\n" +
                                $"`{CommandHandler.GetPrefix(Context.Guild.Id)}t <x> <y>` to take your turn\n`{CommandHandler.GetPrefix(Context.Guild.Id)}t end` to end the game\n\n" +
                                $"(Please note that Naughts and Crosses is still beta and you may experience issues.)");

                            var oldMsg = await Context.Channel.GetMessageAsync(board.MessageId);
                            if (oldMsg != null) await oldMsg.DeleteAsync();
                            board.MessageId = msg.Id;
                            NaughtsGames.UpdateGame(Context.Guild.Id, board);
                        }
                        else
                        {
                            // We have a winner
                            IUser winner;
                            if (board.Players[0].SelectedState == winnerState) winner = board.Players[0].User;
                            else winner = board.Players[1].User;

                            var oldMsg = await Context.Channel.GetMessageAsync(board.MessageId);
                            if (oldMsg != null) await oldMsg.DeleteAsync();

                            var msg = await Context.Channel.SendFileAsync(board.DrawBoard(), $"**TicTacToe | Naughts & Crosses**\n" +
                                $"Well Done, {winner.Username} has won the game!");
                            NaughtsGames.EndGame(Context.Guild.Id);
                        }
                    }
                }
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Title = "TicTacToe | Naughts & Crosses",
                    Description = "There is no game in this server?!?",
                    Color = Discord.Color.DarkPurple,
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("t end")]
        [Alias("t e")]
        [RequireBotPermission(ChannelPermission.ManageMessages)]
        [RequireBotPermission(ChannelPermission.SendMessages)]
        [RequireBotPermission(ChannelPermission.AttachFiles)]
        private async Task NaughtsEndAsync()
        {
            NaughtsBoard board = NaughtsGames.GetBoard(Context.Guild.Id);
            if (board != null)
            {
                if (board.Players[0].User.Id == Context.User.Id || board.Players[1].User.Id == Context.User.Id || (Context.User as IGuildUser).GetPermissions(Context.Channel as IGuildChannel).ManageMessages)
                {
                    await Context.Message.DeleteAsync();

                    NaughtsGames.EndGame(Context.Guild.Id);
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Title = "TicTacToe | Naughts & Crosses",
                        Description = $"The game has ended.",
                        Color = Discord.Color.DarkPurple,
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" }
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());
                }
            }
        }
    }

    static class NaughtsGames
    {
        private static List<NaughtsBoard> Games = new List<NaughtsBoard>();

        public static NaughtsBoard CreateNewGame(ulong guild, IUser playerOne, IUser playerTwo)
        {
            NaughtsBoard board = new NaughtsBoard(guild, playerOne, playerTwo);
            Games.Add(board);
            return board;
        }

        public static void EndGame(ulong guild)
        {
            foreach (NaughtsBoard board in Games)
            {
                if (board.Guild == guild)
                {
                    Games.Remove(board);
                    return;
                }
            }
        }

        public static void UpdateGame(ulong guild, NaughtsBoard updatedBoard)
        {
            foreach (NaughtsBoard board in Games)
            {
                if (board.Guild == guild)
                {
                    Games.Remove(board);
                    Games.Add(updatedBoard);
                    return;
                }
            }
        }

        public static NaughtsBoard GetBoard(ulong guild)
        {
            foreach (NaughtsBoard board in Games)
            {
                if (board.Guild == guild) return board;
            }
            return null;
        }
    }

    class NaughtsBoard
    {
        public ulong Guild;
        public ulong MessageId { get; set; }

        public NaughtsTile[,] Board;
        public NaughtsPlayer[] Players;
        public int CurrentPlayer = 0;
        

        public NaughtsBoard(ulong guild, IUser playerOne, IUser playerTwo)
        {
            Guild = guild;

            Board = new NaughtsTile[3, 3];
            for (int x = 0; x < Board.GetLength(0); x++)
                for (int y = 0; y < Board.GetLength(1); y++)
                    Board[x, y] = new NaughtsTile(NaughtsTileState.Free);

            Players = new NaughtsPlayer[] {
                new NaughtsPlayer(playerOne, NaughtsTileState.Naught),
                new NaughtsPlayer(playerTwo, NaughtsTileState.Cross),
            };

            Console.WriteLine("t");
        }

        public NaughtsTileState CheckForWinner()
        {
            // Todo: Make this compatible for bigger boards

            if (Board[0, 0].State == Board[1, 1].State && Board[0, 0].State == Board[2, 2].State) return Board[1, 1].State;
            if (Board[0, 2].State == Board[1, 1].State && Board[0, 2].State == Board[2, 0].State) return Board[1, 1].State;

            for (int x = 0; x < 3; x++)
                if (Board[x, 0].State == Board[x, 1].State && Board[x, 0].State == Board[x, 2].State) return Board[x, 0].State;

            for (int y = 0; y < 3; y++)
                if (Board[0,y].State == Board[1,y].State && Board[0,y].State == Board[2,y].State) return Board[0,y].State;

            return NaughtsTileState.Free;
        }

        public string DrawBoard()
        {
            // Draw board
            int tileSize = 128;
            int width = (Board.GetLength(0) * tileSize) + tileSize;
            int height = (Board.GetLength(1) * tileSize) + tileSize;

            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
            graphicImage.FillRectangle(Brushes.Magenta, new Rectangle(0, 0, width, height));

            Bitmap backTexture = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"naughts/textures/background.png"));
            graphicImage.DrawImage(backTexture, 0, 0, width, height);

            for (int x = 0; x < Board.GetLength(0); x++)
            {
                for (int y = 0; y < Board.GetLength(1); y++)
                {
                    string tileTexturePath = Path.Combine(AppContext.BaseDirectory, $"naughts/textures/tile_{Board[x,y].State.ToString().ToLower()}.png");
                    Bitmap tileTexture = new Bitmap(tileTexturePath);

                    graphicImage.DrawImage(tileTexture, (x*tileSize)+(tileSize/2), (y*tileSize)+(tileSize/2), tileSize, tileSize);
                    tileTexture.Dispose();
                }
            }

            // Save board
            string path = Path.Combine(AppContext.BaseDirectory, $"naughts/textures/{Guild}_board.jpg");
            editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

            graphicImage.Dispose();
            editedBitmap.Dispose();

            // Return path to board
            return path;
        }
    }

    class NaughtsPlayer
    {
        public IUser User;
        public NaughtsTileState SelectedState;

        public NaughtsPlayer(IUser user, NaughtsTileState selectedState)
        {
            User = user;
            SelectedState = selectedState;
        }
    }

    class NaughtsTile
    {
        public NaughtsTileState State { get; set; }

        public NaughtsTile(NaughtsTileState state)
        {
            State = state;
        }
    }

    enum NaughtsTileState
    {
        Free, Naught, Cross
    }
}
