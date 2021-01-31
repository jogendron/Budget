using System;
using Budget.Users.Api.Entities;
using Microsoft.Extensions.Configuration;

namespace Budget.Users.Api.ServiceCollection.WriteModelPersistence
{
    public class WriteModelServiceCollectionSelector : CustomServiceCollectionSelector
    {
        private const string InMemory = "InMemory";
        private const string MongoDb = "MongoDb";

        public WriteModelServiceCollectionSelector(IConfiguration configuration, Providers providers) : base(configuration, providers)
        {
        }

        public override ICustomServiceCollection GetServiceCollection()
        {
            ICustomServiceCollection serviceCollection = null;

            switch (Providers.WriteModelPersistence)
            {
                case InMemory:
                    serviceCollection = new InMemoryWriteModelPersistenceServiceCollection();
                    break;

                case MongoDb:
                    serviceCollection = new MongoDbWriteModelPersistenceServiceCollection(Configuration);
                    break;

                default:
                    throw new ArgumentException($"Value {Providers.WriteModelPersistence} is not supported for write model persistence.");
            }

            return serviceCollection;
        }
    }
}