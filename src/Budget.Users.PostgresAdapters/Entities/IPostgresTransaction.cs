using System;

namespace Budget.Users.PostgresAdapters.Entities
{
    public interface IPostgresTransaction : IDisposable
    {
        void Commit();

        void Rollback();
    }
}