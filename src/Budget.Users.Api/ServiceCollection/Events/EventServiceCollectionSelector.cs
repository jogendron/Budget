using System;
using Microsoft.Extensions.Configuration;
using Budget.Users.Api.Entities;

namespace Budget.Users.Api.ServiceCollection.Events
{
    public class EventServiceCollectionSelector : CustomServiceCollectionSelector
    {
        private const string InMemory = "InMemory";
        private const string Kafka = "Kafka";

        public EventServiceCollectionSelector(IConfiguration configuration, Providers providers) : base(configuration, providers)
        {
        }

        public override ICustomServiceCollection GetServiceCollection()
        {
            ICustomServiceCollection serviceCollection = null;

            switch (Providers.Events)
            {
                case "InMemory":
                    serviceCollection = new InMemoryEventServiceCollection();
                    break;

                case "Kafka":
                    serviceCollection = new KafkaEventServiceCollection(Configuration);
                    break;

                default:
                    throw new ArgumentException($"Value {Providers.Events} is not supported for events provider.");
            }

            return serviceCollection;
        }
    }

}