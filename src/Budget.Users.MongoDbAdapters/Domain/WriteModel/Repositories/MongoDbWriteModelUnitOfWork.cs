using System;

using MongoDB.Driver;

using Budget.Users.Domain.WriteModel.Repositories;
using Budget.Users.MongoDbAdapters.Factories;
using Budget.Users.MongoDbAdapters.Entities;

namespace Budget.Users.MongoDbAdapters.Domain.WriteModel.Repositories
{

    public class MongoDbWriteModelUnitOfWork : IWriteModelUnitOfWork
    {
        internal const string databaseName = "Budget_User";

        public MongoDbWriteModelUnitOfWork(
            MongoDbConfiguration configuration,
            IMongoClientFactory clientFactory, 
            Budget.Users.Domain.WriteModel.Factories.WriteModelUserFactory userFactory
        )
        {
            Configuration = configuration;
            Client = clientFactory.CreateClient(configuration);
            Session = Client.StartSession();
            Database = Client.GetDatabase(databaseName);
            UserRepository = new MongoDbWriteModelUserRepository(Database, userFactory);
        }

        private MongoDbConfiguration Configuration { get; }

        private IMongoClient Client { get; }

        private IClientSessionHandle Session { get; }

        private IMongoDatabase Database { get; }

        public IWriteModelUserRepository UserRepository { get; }

        public void BeginTransaction()
        {
            if (Configuration.EnableTransactions)
            {
                if (Session.IsInTransaction)
                    throw new InvalidOperationException("A transaction is already opened.");

                Session.StartTransaction();
            }
        }

        public void Commit()
        {
            if (Configuration.EnableTransactions)
            {
                if (! Session.IsInTransaction)
                    throw new InvalidOperationException("A transaction must be opened for commit to work.");

                Session.CommitTransaction();
            }
        }

        public void Rollback()
        {
            if (Configuration.EnableTransactions)
            {
                if (! Session.IsInTransaction)
                    throw new InvalidOperationException("A transaction must be opened for rollback to work.");

                Session.AbortTransaction();
            }
        }
    }

}