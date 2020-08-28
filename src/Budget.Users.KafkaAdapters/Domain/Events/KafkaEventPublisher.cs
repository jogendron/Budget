using System.Collections.Generic;
using System.Threading.Tasks;
using Budget.EventSourcing.Events;
using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Domain.Events
{
    public class KafkaEventPublisher : EventPublisherBase
    {
        public KafkaEventPublisher(IKafkaProducerFactory producerFactory)
        {
            Producer = producerFactory.Create();
        }

        private IProducer<string, string> Producer { get; }

        public override async Task Publish(Event @event)
        {
            await Task.CompletedTask;
            throw new System.NotImplementedException();
        }

        public override async Task Publish(IEnumerable<Event> events)
        {
            foreach (var @event in events)
            {
                await Publish(@event);
            }
        }
    }
}