using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Domain.Events
{
    public interface IKafkaProducerFactory
    {
        IProducer<string, string> Create();
    }
}