using System;
using Budget.Users.Api.Entities;
using Microsoft.Extensions.Configuration;

namespace Budget.Users.Api.ServiceCollection.ReadModelPersistence
{
    public class ReadModelPersistenceSelector : CustomServiceCollectionSelector
    {
        public ReadModelPersistenceSelector(IConfiguration configuration, Providers providers) : base(configuration, providers)
        {
        }
        public override ICustomServiceCollection GetServiceCollection()
        {
            ICustomServiceCollection serviceCollection = null;

            switch (Providers.ReadModelPersistence)
            {
                case "InMemory":
                    serviceCollection = new InMemoryReadModelPersistenceServices();
                    break;

                case "Postgres":
                    serviceCollection = new PostgresReadModelPersistenceServices(Configuration);
                    break;

                default:
                    throw new ArgumentException($"Value {Providers.ReadModelPersistence} is not a supported read model persistence provider.");
            }

            return serviceCollection;
        }
    }
}