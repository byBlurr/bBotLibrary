namespace Discord.Net.Bot.Groups
{
    public class User
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Points { get; set; }
        public int Level { get; set; }

        public User()
        {
            Name = "";
            Id = 0;
            Points = 0;
            Level = 0;
        }
    }
}
