using Npgsql;
using Budget.Users.PostgresAdapters.Entities;

namespace Budget.Users.PostgresAdapters.Factories
{
    public class PostgresConnectionFactory : IPostgresConnectionFactory
    {
        public PostgresConnectionFactory()
        {
            
        }

        public IPostgresConnection Create(PostgresConfiguration configuration)
        {
            return new PostgresConnection(
                new NpgsqlConnection(configuration.GetConnectionString())
            );
        }
    }
}