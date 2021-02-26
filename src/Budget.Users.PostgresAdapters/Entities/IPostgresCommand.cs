using System;
using System.Data;
using System.Threading.Tasks;

namespace Budget.Users.PostgresAdapters.Entities
{
    public interface IPostgresCommand : IDisposable
    {
        string CommandText { get; set; }

        IPostgresTransaction Transaction { get; set; }
        
        IPostgresParameterCollection Parameters { get; }

        IPostgresParameter CreateParameter();

        void Prepare();

        Task PrepareAsync();

        IDataReader ExecuteReader();

        Task<IDataReader> ExecuteReaderAsync();

        int ExecuteNonQuery();

        Task<int> ExecuteNonQueryAsync();
    }
}