using System.Collections.Generic;
using System.Threading.Tasks;
using Budget.EventSourcing.Events;
using Budget.EventSourcing.Services.Serialization;
using Budget.Users.Domain.WriteModel.Events;
using Budget.Users.KafkaAdapters.Entities;
using Budget.Users.KafkaAdapters.Factories;
using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Domain.WriteModel.Events
{
    public class KafkaEventPublisher : EventPublisherBase
    {
        public KafkaEventPublisher(
            IKafkaGatewayFactory kafkaGatewayFactory,
            IEventSerializer serializaer
        )
        {
            Producer = kafkaGatewayFactory.CreateProducer();
            Serializer = serializaer;
        }

        private IProducer<string, string> Producer { get; }

        private IEventSerializer Serializer { get; }

        public override async Task Publish(Event @event)
        {
            string serializedEvent = Serializer.Serialize(@event);
            
            await SendMessage(KafkaTopics.UserEventSourcing, @event.AggregateId.ToString(), serializedEvent);

            if (@event is UserSubscribed)
                await SendMessage(KafkaTopics.UserPublicEvents, @event.AggregateId.ToString(), serializedEvent);
        }

        public override async Task Publish(IEnumerable<Event> events)
        {
            foreach (var @event in events)
                await Publish(@event);
        }

        private async Task SendMessage(string topicName, string key, string value)
        {
            await Producer.ProduceAsync(
                    topicName, 
                    new Message<string, string>() {
                        Key = key,
                        Value = value
                    }
                );
        }
    }
}