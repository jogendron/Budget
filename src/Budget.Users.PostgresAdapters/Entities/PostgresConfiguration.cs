using System;
using System.Text;

namespace Budget.Users.PostgresAdapters.Entities
{
    public class PostgresConfiguration
    {
        public PostgresConfiguration()
        {
            Host = string.Empty;
            Port = -1;
            Username = string.Empty;
            Password = string.Empty;
            Database = string.Empty;
        }

        public string Host { get; set;}

        public int Port { get; set;}

        public string Username { get; set;}

        public string Password { get; set; }

        public string Database { get; set; }

        public string GetConnectionString()
        {
            if (string.IsNullOrEmpty(Host))
                throw new ArgumentException("Host cannot be empty.");

            if (string.IsNullOrEmpty(Username))
                throw new ArgumentException("Username cannot be empty.");

            if (string.IsNullOrEmpty(Password))
                throw new ArgumentException("Password cannot be empty.");

            var builder = new StringBuilder();

            builder.Append($"Host={Host}");

            if (Port > 0)
                builder.Append($";Port={Port}");

            builder.Append($";Username={Username}");

            builder.Append($";Password={Password}");

            if (! string.IsNullOrEmpty(Database))
                builder.Append($";Database={Database}");

            return builder.ToString();
        }
    }
}