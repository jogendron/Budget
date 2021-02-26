using Npgsql;
using Budget.Users.PostgresAdapters.Entities;

namespace Budget.Users.PostgresAdapters.Factories
{
    public interface IPostgresConnectionFactory
    {
        IPostgresConnection Create(PostgresConfiguration configuration);
    }
}