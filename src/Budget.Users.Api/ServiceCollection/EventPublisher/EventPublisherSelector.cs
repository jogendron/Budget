using System;
using Microsoft.Extensions.Configuration;
using Budget.Users.Api.Entities;

namespace Budget.Users.Api.ServiceCollection.EventPublisher
{
    public class EventPublisherSelector : CustomServiceCollectionSelector
    {
        private const string InMemory = "InMemory";
        private const string Kafka = "Kafka";

        public EventPublisherSelector(IConfiguration configuration, Providers providers) : base(configuration, providers)
        {
        }

        public override ICustomServiceCollection GetServiceCollection()
        {
            ICustomServiceCollection serviceCollection = null;

            switch (Providers.EventPublisher)
            {
                case "InMemory":
                    serviceCollection = new InMemoryEventServices();
                    break;

                case "Kafka":
                    serviceCollection = new KafkaEventServices(Configuration);
                    break;

                default:
                    throw new ArgumentException($"Value {Providers.EventPublisher} is not a supported event publisher.");
            }

            return serviceCollection;
        }
    }

}