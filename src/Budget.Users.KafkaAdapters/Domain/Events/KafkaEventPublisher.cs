using System.Collections.Generic;
using System.Threading.Tasks;
using Budget.EventSourcing.Events;
using Budget.EventSourcing.Services.Serialization;
using Budget.Users.Domain.Events;
using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Domain.Events
{
    public class KafkaEventPublisher : EventPublisherBase
    {
        internal const string eventSourcingTopic = "User.EventSourcing";
        internal const string publicEventsTopic = "User.PublicEvents";

        public KafkaEventPublisher(
            IKafkaProducerFactory producerFactory,
            IEventSerializer serializaer
        )
        {
            Producer = producerFactory.Create();
            Serializer = serializaer;
        }

        private IProducer<string, string> Producer { get; }

        private IEventSerializer Serializer { get; }

        public override async Task Publish(Event @event)
        {
            

            string serializedEvent = Serializer.Serialize(@event);
            
            await SendMessage(eventSourcingTopic, @event.AggregateId.ToString(), serializedEvent);

            if (@event is UserSubscribed)
                await SendMessage(publicEventsTopic, @event.AggregateId.ToString(), serializedEvent);
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