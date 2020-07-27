using Discord;
using Discord.Commands;
using Discord.Net.Bot;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace LorisAngelBot.Modules
{
    public class BlackjackModule : ModuleBase
    {
        [Command("blackjack")]
        private async Task BlackjackAsync()
        {
            await Context.Message.DeleteAsync();

            if (BJGames.GetGame(Context.Guild.Id) == null)
            {
                BJTable table = BJGames.StartGame(Context.Channel as ITextChannel);
                table.Players.Add(new BJPlayer(Context.User as IUser));
                await table.OpenTable();
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Discord.Color.DarkPurple,
                    Title = "Blackjack",
                    Description = "There is already a table in this guild!",
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("bj")]
        private async Task BlackjackActionAsync(string action = "")
        {
            await Context.Message.DeleteAsync();

            BJTable table = BJGames.GetGame(Context.Guild.Id);
            if (table != null)
            {
                switch (action.ToLower())
                {
                    case "hit":
                        await table.Act(Context.User.Id, BJState.Hit);
                        break;

                    case "double":
                        await table.Act(Context.User.Id, BJState.Double);
                        break;

                    case "hold":
                        await table.Act(Context.User.Id, BJState.Hold);
                        break;

                    default:
                        break;
                }
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Discord.Color.DarkPurple,
                    Title = "Blackjack",
                    Description = "There is not an active table in this guild!",
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("blackjack start")]
        private async Task BlackjackStartAsync()
        {
            await Context.Message.DeleteAsync();

            BJTable table = BJGames.GetGame(Context.Guild.Id);
            if (table != null)
            {
                await table.Start();
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Discord.Color.DarkPurple,
                    Title = "Blackjack",
                    Description = "There is not an active table in this guild!",
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("blackjack join")]
        private async Task BlackjackJoinAsync()
        {
            await Context.Message.DeleteAsync();

            BJTable table = BJGames.GetGame(Context.Guild.Id);
            if (table != null)
            {
                bool joined = await table.JoinTable(Context.User as IUser);
                if (joined)
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Discord.Color.DarkPurple,
                        Title = "Blackjack",
                        Description = "You joined the blackjack table.",
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());
                }
                else
                {
                    EmbedBuilder embed = new EmbedBuilder()
                    {
                        Color = Discord.Color.DarkPurple,
                        Title = "Blackjack",
                        Description = "There is already 4 players at this table.",
                        Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                    };
                    await Context.Channel.SendMessageAsync(null, false, embed.Build());
                }
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Discord.Color.DarkPurple,
                    Title = "Blackjack",
                    Description = "There is not an active table in this guild!",
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }

        [Command("blackjack leave")]
        private async Task BlackjackLeaveAsync()
        {
            await Context.Message.DeleteAsync();

            BJTable table = BJGames.GetGame(Context.Guild.Id);
            if (table != null)
            {
                table.LeaveTable(Context.User as IUser);

                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Discord.Color.DarkPurple,
                    Title = "Blackjack",
                    Description = "You left the blackjack table.",
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
            else
            {
                EmbedBuilder embed = new EmbedBuilder()
                {
                    Color = Discord.Color.DarkPurple,
                    Title = "Blackjack",
                    Description = "There is not an active table in this guild!",
                    Footer = new EmbedFooterBuilder() { Text = $"{Util.GetRandomEmoji()}  Requested by {Context.User.Username}#{Context.User.Discriminator}" },
                };
                await Context.Channel.SendMessageAsync(null, false, embed.Build());
            }
        }
    }

    public class BJGames
    {
        private static List<BJTable> ActiveTables = new List<BJTable>();

        public static BJTable StartGame(ITextChannel channel)
        {
            BJTable table = new BJTable(channel);
            ActiveTables.Add(table);

            return table;
        }

        public static BJTable GetGame(ulong id)
        {
            foreach (BJTable table in ActiveTables) if (table.TableId == id) return table;
            
            return null;
        }

        public static void EndGame(ulong id)
        {
            foreach (BJTable table in ActiveTables)
            {
                if (table.TableId == id)
                {
                    ActiveTables.Remove(table);
                    return;
                }
            }
        }
    }

    public class BJTable
    {
        public ulong TableId { get; set; }
        public ITextChannel TableChannel { get; set; }
        public IMessage TableMessage { get; set; }
        public List<BJPlayer> Players { get; set; }
        public List<BJPlayer> PlayersWaiting { get; set; }
        public Deck DealersDeck { get; set; }
        public Deck DealersHand { get; set; }
        public int DealersValue { get; set; }


        private bool TableStarted = false;

        public BJTable(ITextChannel channel)
        {
            TableId = channel.GuildId;
            TableChannel = channel;
            TableMessage = null;
            Players = new List<BJPlayer>();
            PlayersWaiting = new List<BJPlayer>();
            DealersDeck = new Deck();
            DealersHand = new Deck();
            DealersValue = 0;
            DealersDeck.ResetDeck();
            TableStarted = false;
        }

        private async Task SendMessageUpdate(bool updateImage = true)
        {
            bool ready = true;
            string prefix = CommandHandler.GetPrefix(TableId);

            foreach (BJPlayer player in Players) if (player.State == BJState.Waiting) ready = false;

            string text = "**Blackjack**\n";
            if (!TableStarted)
            {
                if (!ready) text = $"{text}Waiting for players to join...\n`{prefix}blackjack join`\n`{prefix}blackjack start`\n";
            }
            else
            {
                if (!ready) text = $"{text}Waiting for player actions...\n`{prefix}bj hit|hold`\n";
                else text = $"{text}Dealing...";
            }

            foreach (BJPlayer player in Players) text = $"{text}\n**{player.Name}:** {player.State.ToString()}";
            foreach (BJPlayer player in PlayersWaiting) text = $"{text}\n**{player.Name}:** {player.State.ToString()}";

            if (updateImage)
            {
                string tableImage = Draw();
                var msg = await TableChannel.SendFileAsync(tableImage, text);
                File.Delete(tableImage);

                if (TableMessage != null) await TableMessage.DeleteAsync();
                TableMessage = msg;
            }
            else
            {
                await (TableMessage as IUserMessage).ModifyAsync(x => x.Content = text);
            }
        }

        public async Task OpenTable()
        {
            await SendMessageUpdate();
        }

        public async Task Start()
        {
            if (Players.Count > 0 || PlayersWaiting.Count > 0)
            {

                foreach (BJPlayer player in PlayersWaiting)
                {
                    Players.Add(player);
                    PlayersWaiting.Remove(player);
                }

                foreach (BJPlayer player in Players)
                {
                    player.State = BJState.Waiting;
                    player.PlayersDeck.Cards.Clear();
                    player.Value = 0;
                }
                DealersDeck = new Deck();
                DealersHand = new Deck();
                DealersValue = 0;
                DealersDeck.ResetDeck();

                DealersDeck.ShuffleDeck();
                DealCards();
                DealCards();
                DealDealersHand();
                DealDealersHand();
                DealDealersHand(false);
                DealDealersHand(false);
                await SendMessageUpdate();
                TableStarted = true;
            }
            else TableStarted = false;
        }

        public async Task Act(ulong id, BJState newState)
        {
            if (!TableStarted) return;

            bool ready = true;
            foreach (BJPlayer player in Players)
            {
                if (player.User.Id == id)
                    if (player.State == BJState.Waiting)
                    {
                        player.State = newState;
                    }

                if (player.State == BJState.Waiting) ready = false;
            }

            await SendMessageUpdate(false);

            if (ready) await Turn();
        }

        private async Task Turn()
        {
            bool allHolding = true;
            foreach (BJPlayer player in Players)
            {
                if (player.State == BJState.Hit)
                {
                    DealCard(true, player);
                    if (player.Value > 21) player.State = BJState.SittingOut;
                    else player.State = BJState.Waiting;
                    allHolding = false;
                }
                else if (player.State == BJState.Double)
                {
                    DealCard(true, player);
                    if (player.Value > 21) player.State = BJState.SittingOut;
                    else player.State = BJState.Hold;
                    allHolding = false;
                }
            }

            foreach (Card card in DealersHand.Cards)
            {
                if (card.State == CardState.Unflipped)
                {
                    card.State = CardState.Flipped;
                    CalculateDealerValue();
                    await SendMessageUpdate();
                    if (allHolding) await End();
                    return;
                }
            }

            await End();
        }

        private async Task End()
        {
            for (int i = 0; i < 3; i++)
            {
                bool hasAWinner = false;
                foreach (BJPlayer player in Players)
                {
                    if (player.State != BJState.SittingOut)
                    {
                        if (player.Value <= 21 && player.Value > DealersValue) hasAWinner = true;
                    }
                }

                if (hasAWinner)
                {
                    DealersHand.Cards[i].State = CardState.Flipped;
                    CalculateDealerValue();
                }
            }

            foreach (BJPlayer player in Players)
            {
                if (player.State != BJState.SittingOut)
                {
                    if (player.Value <= 21 && player.Value > DealersValue || player.Value <= 21 && DealersValue > 21) player.State = BJState.Winner;
                    else if (player.Value == DealersValue) player.State = BJState.Draw;
                    else player.State = BJState.Loser;
                }
            }
            await SendMessageUpdate();

            await Task.Delay(10000);
            await Start();
        }

        private void DealCards(bool flip = true)
        {
            foreach (BJPlayer player in Players)
            {
                DealCard(flip, player);
            }
        }

        private void DealCard(bool flip, BJPlayer player)
        {
            Card card = DealersDeck.Cards[DealersDeck.Cards.Count - 1];

            if (flip) card.State = CardState.Flipped;
            else card.State = CardState.Unflipped;

            player.PlayersDeck.Cards.Add(card);
            player.Value += card.Value;
            DealersDeck.Cards.RemoveAt(DealersDeck.Cards.Count - 1);
        }

        private void DealDealersHand(bool flip = true)
        {
            Card card = DealersDeck.Cards[DealersDeck.Cards.Count - 1];

            if (flip) card.State = CardState.Flipped;
            else card.State = CardState.Unflipped;

            DealersHand.Cards.Add(card);
            DealersDeck.Cards.RemoveAt(DealersDeck.Cards.Count - 1);
            CalculateDealerValue();
        }

        private void CalculateDealerValue()
        {
            int value = 0;
            foreach (Card card in DealersHand.Cards)
            {
                if (card.State == CardState.Flipped) value += card.Value;
            }

            DealersValue = value;
        }

        public async Task<bool> JoinTable(IUser user)
        {
            if ((Players.Count + PlayersWaiting.Count) >= 4) return false;

            BJPlayer player = new BJPlayer(user);
            if (Players.Contains(player)) return false;
            if (PlayersWaiting.Contains(player)) return false;

            if (TableStarted)
            {
                player.State = BJState.SittingOut;
                PlayersWaiting.Add(player);
            }
            else Players.Add(player);

            await SendMessageUpdate(false);
            return true;
        }

        public void LeaveTable(IUser user)
        {
            if (Players.Count == 0 || PlayersWaiting.Count == 0) return;

            BJPlayer player = new BJPlayer(user);
            if (Players.Contains(player)) Players.Remove(player);
            if (PlayersWaiting.Contains(player)) PlayersWaiting.Remove(player);
        }

        private string Draw()
        {
            int width = 1280;
            int height = 720;
            int cardW = 691 / 5;
            int cardH = 1056 / 5;

            Bitmap editedBitmap = new Bitmap(width, height);
            Graphics graphicImage = Graphics.FromImage(editedBitmap);
            graphicImage.SmoothingMode = SmoothingMode.AntiAlias;
            graphicImage.FillRectangle(Brushes.DarkGreen, new Rectangle(0, 0, width, height));

            Bitmap backTexture = new Bitmap(Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/background1.png"));
            graphicImage.DrawImage(backTexture, 0, 0, width, height);

            // Render Deck
            int cardCount = 0;
            foreach (Card card in DealersDeck.Cards)
            {
                cardCount++;
                string cardTexturePath;
                if (card.State == CardState.Flipped) cardTexturePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/{card.Id.ToUpper()}.png");
                else cardTexturePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/back.png");

                Bitmap cardTexture = new Bitmap(cardTexturePath);
                graphicImage.DrawImage(cardTexture, 20 + (cardCount * 3), 30, cardW, cardH);
                cardTexture.Dispose();
            }

            cardCount = 0;
            // Render Dealers Hand
            foreach (Card card in DealersHand.Cards)
            {
                cardCount++;
                string cardTexturePath;
                if (card.State == CardState.Flipped) cardTexturePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/{card.Id.ToUpper()}.png");
                else cardTexturePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/back.png");

                Bitmap cardTexture = new Bitmap(cardTexturePath);
                graphicImage.DrawImage(cardTexture, (640 - cardW) + (cardCount * (cardW + 5)), 30, cardW, cardH);
                cardTexture.Dispose();
            }

            cardCount = 0;
            int playerCount = 0;
            // Render Players Hands
            foreach (BJPlayer player in Players)
            {
                foreach (Card card in player.PlayersDeck.Cards)
                {
                    cardCount++;
                    string cardTexturePath;
                    if (card.State == CardState.Flipped) cardTexturePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/{card.Id.ToUpper()}.png");
                    else cardTexturePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/back.png");

                    Bitmap cardTexture = new Bitmap(cardTexturePath);
                    graphicImage.DrawImage(cardTexture, (20 + (cardCount * (cardW / 4))) + (playerCount * 225), 475, cardW, cardH);
                    cardTexture.Dispose();
                }

                string download = player.User.GetAvatarUrl();
                string imagePath = Path.Combine(AppContext.BaseDirectory, $"blackjack/{player.Name}");
                using (WebClient client = new WebClient()) client.DownloadFile(new Uri(download), imagePath);
                Bitmap loadedTexture = new Bitmap(imagePath);
                graphicImage.DrawImage(loadedTexture, 20 + (playerCount * 225), 360, 75, 75);
                loadedTexture.Dispose();
                File.Delete(imagePath);

                playerCount++;
            }

            string path = Path.Combine(AppContext.BaseDirectory, $"blackjack/textures/{TableId}_board.jpg");
            editedBitmap.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);

            backTexture.Dispose();
            graphicImage.Dispose();
            editedBitmap.Dispose();

            // Return path to board
            return path;
        }
    }

    public class BJPlayer
    {
        public string Name { get; set; }
        public IUser User { get; set; }
        public Deck PlayersDeck { get; set; }
        public BJState State { get; set; }
        public int Value { get; set; }

        public BJPlayer(IUser user)
        {
            Name = Util.ToUppercaseFirst(user.Username);
            User = user;
            PlayersDeck = new Deck();
            State = BJState.Waiting;
            Value = 0;
        }


    }

    public enum BJState
    {
        Waiting, Hit, Hold, Double, SittingOut, Winner, Loser, Draw
    }

    public class Deck
    {
        public List<Card> Cards { get; set; }

        public Deck()
        {
            Cards = new List<Card>();
        }

        public void ResetDeck()
        {
            Cards.Clear();
            Cards.Add(new Card("Ace of Spades", "AS", 'a', 1));
            Cards.Add(new Card("Ace of Clubs", "AC", 'a', 1));
            Cards.Add(new Card("Ace of Diamonds", "AD", 'a', 1));
            Cards.Add(new Card("Ace of Hearts", "AH", 'a', 1));
            Cards.Add(new Card("Two of Spades", "2S", '2', 2));
            Cards.Add(new Card("Two of Clubs", "2C", '2', 2));
            Cards.Add(new Card("Two of Diamonds", "2D", '2', 2));
            Cards.Add(new Card("Two of Hearts", "2H", '2', 2));
            Cards.Add(new Card("Three of Spades", "3S", '3', 3));
            Cards.Add(new Card("Three of Clubs", "3C", '3', 3));
            Cards.Add(new Card("Three of Diamonds", "3D", '3', 3));
            Cards.Add(new Card("Three of Hearts", "3H", '3', 3));
            Cards.Add(new Card("Four of Spades", "4S", '4', 4));
            Cards.Add(new Card("Four of Clubs", "4C", '4', 4));
            Cards.Add(new Card("Four of Diamonds", "4D", '4', 4));
            Cards.Add(new Card("Four of Hearts", "4H", '4', 4));
            Cards.Add(new Card("Five of Spades", "5S", '5', 5));
            Cards.Add(new Card("Five of Clubs", "5C", '5', 5));
            Cards.Add(new Card("Five of Diamonds", "5D", '5', 5));
            Cards.Add(new Card("Five of Hearts", "5H", '5', 5));
            Cards.Add(new Card("Six of Spades", "6S", '6', 6));
            Cards.Add(new Card("Six of Clubs", "6C", '6', 6));
            Cards.Add(new Card("Six of Diamonds", "6D", '6', 6));
            Cards.Add(new Card("Six of Hearts", "6H", '6', 6));
            Cards.Add(new Card("Seven of Spades", "7S", '7', 7));
            Cards.Add(new Card("Seven of Clubs", "7C", '7', 7));
            Cards.Add(new Card("Seven of Diamonds", "7D", '7', 7));
            Cards.Add(new Card("Seven of Hearts", "7H", '7', 7));
            Cards.Add(new Card("Eight of Spades", "8S", '8', 8));
            Cards.Add(new Card("Eight of Clubs", "8C", '8', 8));
            Cards.Add(new Card("Eight of Diamonds", "8D", '8', 8));
            Cards.Add(new Card("Eight of Hearts", "8H", '8', 8));
            Cards.Add(new Card("Nine of Spades", "9S", '9', 9));
            Cards.Add(new Card("Nine of Clubs", "9C", '9', 9));
            Cards.Add(new Card("Nine of Diamonds", "9D", '9', 9));
            Cards.Add(new Card("Nine of Hearts", "9H", '9', 9));
            Cards.Add(new Card("Ten of Spades", "10S", '0', 10));
            Cards.Add(new Card("Ten of Clubs", "10C", '0', 10));
            Cards.Add(new Card("Ten of Diamonds", "10D", '0', 10));
            Cards.Add(new Card("Ten of Hearts", "10H", '0', 10));
            Cards.Add(new Card("Jack of Spades", "JS", 'j', 10));
            Cards.Add(new Card("Jack of Clubs", "JC", 'j', 10));
            Cards.Add(new Card("Jack of Diamonds", "JD", 'j', 10));
            Cards.Add(new Card("Jack of Hearts", "JH", 'j', 10));
            Cards.Add(new Card("Queen of Spades", "QS", 'q', 10));
            Cards.Add(new Card("Queen of Clubs", "QC", 'q', 10));
            Cards.Add(new Card("Queen of Diamonds", "QD", 'q', 10));
            Cards.Add(new Card("Queen of Hearts", "QH", 'q', 10));
            Cards.Add(new Card("King of Spades", "KS", 'k', 10));
            Cards.Add(new Card("King of Clubs", "KC", 'k', 10));
            Cards.Add(new Card("King of Diamonds", "KD", 'k', 10));
            Cards.Add(new Card("King of Hearts", "KH", 'k', 10));
        }

        public void ShuffleDeck()
        {
            Random rnd = new Random();

            int n = Cards.Count;
            while (n > 0)
            {
                n--;
                int i = rnd.Next(n + 1);
                Card card = Cards[i];
                Cards[i] = Cards[n];
                Cards[n] = card;
            }
        }

        private void AddCard(Card card)
        {
            Cards.Add(card);
        }

        private void RemoveCard(string id)
        {
            foreach (Card card in Cards)
            {
                if (card.Id == id)
                {
                    Cards.Remove(card);
                }
            }
        }
    }

    public class Card
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public char Icon { get; set; }
        public int Value { get; set; }
        public CardState State { get; set; }
        
        public Card(string name, string id, char icon, int value)
        {
            Name = name;
            Id = id;
            Icon = icon;
            Value = value;
            State = CardState.Unflipped;
        }
    }

    public enum CardState
    {
        Flipped, Unflipped
    }
}
