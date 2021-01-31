using MongoDB.Driver;

using Budget.Users.MongoDbAdapters.Entities;

namespace Budget.Users.MongoDbAdapters.Factories
{
    public interface IMongoClientFactory
    {
        IMongoClient CreateClient(MongoDbConfiguration configuration);
    }
}