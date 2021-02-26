using System;
using Budget.Users.Domain.ReadModel.Repositories;
using Budget.Users.PostgresAdapters.Entities;
using Budget.Users.PostgresAdapters.Factories;
using Npgsql;

namespace Budget.Users.PostgresAdapters.Domain.ReadModel.Repositories
{
    public class PostgresReadModelUnitOfWork : IReadModelUnitOfWork, IDisposable
    {
        PostgresReadModelUserRepository userRepository;
        IPostgresTransaction transaction;

        public PostgresReadModelUnitOfWork(PostgresConfiguration configuration, IPostgresConnectionFactory connectionFactory)
        {
            Connection = connectionFactory.Create(configuration);
            Connection.Open();

            userRepository = new PostgresReadModelUserRepository(Connection);
            transaction = null;
        }

        private IPostgresConnection Connection { get; }

        private IPostgresTransaction Transaction 
        { 
            get { return transaction;}
            set {
                transaction = value;
                userRepository.SetTransaction(transaction);
            }
        }
        
        public IReadModelUserRepository UserRepository => userRepository;

        public void BeginTransaction()
        {
            if (Transaction != null)
                throw new InvalidOperationException("A transaction is already opened.");

            Transaction = Connection.BeginTransaction();
        }

        public void Commit()
        {
            if (Transaction == null)
                throw new InvalidOperationException("A transaction must be opened in order to call commit.");

            Transaction.Commit();
            Transaction.Dispose();
            Transaction = null;
        }

        public void Rollback()
        {
            if (Transaction == null)
                throw new InvalidOperationException("A transaction must be opened in ordre to call rollback.");

            Transaction.Rollback();
            Transaction.Dispose();
            Transaction = null;
        }

        public void Dispose()
        {
            Transaction?.Dispose();
            Transaction = null;

            Connection.Close();
            Connection.Dispose();
        }
    }
}