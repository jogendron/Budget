using System;
using System.Linq;
using System.Threading.Tasks;

using Budget.Users.Domain.Events;
using Budget.Users.Domain.Repositories.WriteModelRepositories;
using Budget.Users.MongoDbAdapters.Entities;

using MongoDB.Driver;

namespace Budget.Users.MongoDbAdapters.Domain.Repositories.WriteModelRepositories
{
    public class MongoDbWriteModelUserRepository : IWriteModelUserRepository
    {
        internal const string collectionName = "Users";

        internal MongoDbWriteModelUserRepository(
            IMongoDatabase database,
            Budget.Users.Domain.Factories.WriteModelFactories.WriteModelUserFactory userFactory
        )
        {
            Users = database.GetCollection<User>(collectionName);
            UserFactory = userFactory;
        }

        private IMongoCollection<User> Users { get; }

        private Budget.Users.Domain.Factories.WriteModelFactories.WriteModelUserFactory UserFactory { get; }

        public async Task<Budget.Users.Domain.Model.WriteModel.User> Get(Guid id)
        {
            Budget.Users.Domain.Model.WriteModel.User user = null;
            var search = await Users.FindAsync<User>(u => u.Id == id);

            if (search.Any())
            {
                var dbUser = search.First();
                user = UserFactory.Load(id, dbUser.Changes);
            }

            return user;
        }

        public async Task Save(Budget.Users.Domain.Model.WriteModel.User user)
        {
            var dbUser = new User(user);

            if (IsNewUser(user))
                await Users.InsertOneAsync(dbUser);
            else
                await Users.FindOneAndReplaceAsync<User>(u => u.Id == dbUser.Id, dbUser);
        }

        private bool IsNewUser(Budget.Users.Domain.Model.WriteModel.User user)
        {
            var newSubscriptions = user.NewChanges.Where(
                @event => @event.GetType() == typeof(UserSubscribed)
            );
            
            return newSubscriptions.Any();
        }

    }
}
