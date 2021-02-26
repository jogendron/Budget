using Npgsql;

namespace Budget.Users.PostgresAdapters.Entities
{
    public class PostgresTransaction : IPostgresTransaction
    {
        public PostgresTransaction(NpgsqlTransaction transaction)
        {
            Transaction = transaction;
        }

        public NpgsqlTransaction Transaction { get; }

        public void Commit()
        {
            Transaction.Commit();
        }

        public void Dispose()
        {
            Transaction.Dispose();
        }

        public void Rollback()
        {
            Transaction.Rollback();
        }
    }
}