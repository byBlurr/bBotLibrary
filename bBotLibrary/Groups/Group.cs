using System;
using System.Collections.Generic;
using System.Text;

namespace Discord.Net.Bot.Groups
{
    public class Group
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public int Points { get; set; }
        public List<User> Users { get; set; }

        public Group()
        {
            Name = "";
            Id = 0;
            Points = 0;
            Users = new List<User>();
        }
    }
}
