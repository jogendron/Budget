using System.Text;

namespace Budget.Users.MongoDbAdapters.Entities
{
    public class MongoDbConfiguration
    {
        public MongoDbConfiguration()
        {
            UserName = string.Empty;
            Password = string.Empty;
            Address = string.Empty;
            Port = -1;
            EnableTransactions = false;
        }
        
        public string UserName { get; set; } 
        
        public string Password { get; set; } 
        
        public string Address { get; set; } 

        public int Port { get; set; } 

        //Standalone server cannot support transactions. This feature toggle allows dev environment to work
        public bool EnableTransactions { get; set;} 

        public string GetConnectionString()
        {
            var builder = new StringBuilder();
            builder.Append("mongodb://");

            if (! string.IsNullOrEmpty(UserName) && ! string.IsNullOrEmpty(Password))
            {
                builder.Append($"{UserName}:{Password}@");
            }

            builder.Append(Address);

            if (Port > 0)
                builder.Append($":{Port}");

            return builder.ToString();
        }
    }
}