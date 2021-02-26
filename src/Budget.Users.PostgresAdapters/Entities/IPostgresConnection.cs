using System;

namespace Budget.Users.PostgresAdapters.Entities
{
    public interface IPostgresConnection : IDisposable
    {
        void Open();

        void Close();

        IPostgresTransaction BeginTransaction();

        IPostgresCommand CreateCommand();
    }
}