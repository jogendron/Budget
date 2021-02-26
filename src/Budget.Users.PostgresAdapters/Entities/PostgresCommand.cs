using System.Data;
using System.Threading.Tasks;

using Npgsql;

namespace Budget.Users.PostgresAdapters.Entities
{
    public class PostgresCommand : IPostgresCommand
    {

        public PostgresCommand(NpgsqlCommand command)
        {
            Command = command;
        }

        private NpgsqlCommand Command { get; }
        
        public string CommandText 
        {
            get { return Command.CommandText; }
            set { Command.CommandText = value; }
        }

        public IPostgresTransaction Transaction 
        {
            get { return new PostgresTransaction(Command.Transaction); }
            set { Command.Transaction = ((PostgresTransaction)value).Transaction; }
        }

        public IPostgresParameterCollection Parameters
        {
            get { return new PostgresParameterCollection(Command.Parameters); }
        }

        public IPostgresParameter CreateParameter()
        {
            return new PostgresParameter(Command.CreateParameter());
        }

        public int ExecuteNonQuery()
        {
            return Command.ExecuteNonQuery();
        }

        public Task<int> ExecuteNonQueryAsync()
        {
            return Command.ExecuteNonQueryAsync();
        }

        public IDataReader ExecuteReader()
        {
            return Command.ExecuteReader();
        }

        public async Task<IDataReader> ExecuteReaderAsync()
        {
            var result = await Command.ExecuteReaderAsync();
            return result as IDataReader;
        }

        public void Prepare()
        {
            Command.Prepare();
        }

        public Task PrepareAsync()
        {
            return Command.PrepareAsync();
        }

        public void Dispose()
        {
            Command.Dispose();
        }
    }
}