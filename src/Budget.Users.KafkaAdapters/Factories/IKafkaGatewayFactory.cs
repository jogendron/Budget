using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Factories
{
    public interface IKafkaGatewayFactory
    {
        IProducer<string, string> CreateProducer();

        IConsumer<Ignore, string> CreateConsumer();
    }
}