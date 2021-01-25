using Discord.Net.Bot.Database.Configs;
using MySql.Data.MySqlClient;
using System;

namespace Discord.Net.Bot.Database.Sql
{
    public class DBConnection
    {
        private DBConnection()
        {

        }

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        private MySqlConnection connection = null;
        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                if (String.IsNullOrEmpty(databaseName)) return false;

                string connstring = $"Server=127.0.0.1; database={databaseName}; UID=root; password={BotConfig.Load().DatabasePassword}";
                connection = new MySqlConnection(connstring);
                connection.Open();
            }

            return true;
        }

        public void Close()
        {
            connection.Close();
        }
    }
}
