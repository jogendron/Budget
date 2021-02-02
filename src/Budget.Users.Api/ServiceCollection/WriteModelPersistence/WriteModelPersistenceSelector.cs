using System;
using Budget.Users.Api.Entities;
using Microsoft.Extensions.Configuration;

namespace Budget.Users.Api.ServiceCollection.WriteModelPersistence
{
    public class WriteModelPersistenceSelector : CustomServiceCollectionSelector
    {
        private const string InMemory = "InMemory";
        private const string MongoDb = "MongoDb";

        public WriteModelPersistenceSelector(IConfiguration configuration, Providers providers) : base(configuration, providers)
        {
        }

        public override ICustomServiceCollection GetServiceCollection()
        {
            ICustomServiceCollection serviceCollection = null;

            switch (Providers.WriteModelPersistence)
            {
                case InMemory:
                    serviceCollection = new InMemoryWriteModelPersistenceServices();
                    break;

                case MongoDb:
                    serviceCollection = new MongoDbWriteModelPersistenceServices(Configuration);
                    break;

                default:
                    throw new ArgumentException($"Value {Providers.WriteModelPersistence} is not a supported write model persistence provider.");
            }

            return serviceCollection;
        }
    }
}