using Budget.Users.MongoDbAdapters.Entities;
using MongoDB.Driver;

namespace Budget.Users.MongoDbAdapters.Factories
{

    public class FromConfigMongoClientFactory : IMongoClientFactory
    {
        public FromConfigMongoClientFactory()
        {
        }

        public IMongoClient CreateClient(MongoDbConfiguration configuration)
        {
            return new MongoClient(configuration.GetConnectionString());
        }
    }

}