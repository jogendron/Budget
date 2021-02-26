using Npgsql;

namespace Budget.Users.PostgresAdapters.Entities
{
    public class PostgresConnection : IPostgresConnection
    {
        public PostgresConnection(NpgsqlConnection connection)
        {
            Connection = connection;
        }

        private NpgsqlConnection Connection { get; }

        public IPostgresTransaction BeginTransaction()
        {
            return new PostgresTransaction(Connection.BeginTransaction());
        }

        public IPostgresCommand CreateCommand()
        {
            return new PostgresCommand(Connection.CreateCommand());
        }

        public void Open()
        {
            Connection.Open();
        }

        public void Close()
        {
            Connection.Close();
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}