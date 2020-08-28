using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Domain.Events
{
    public class FromConfigKafkaProducerFactory : IKafkaProducerFactory
    {
        public FromConfigKafkaProducerFactory(KafkaConfiguration configuration)
        {
            Configuration = configuration;
        }

        private KafkaConfiguration Configuration { get; }

        public IProducer<string, string> Create()
        {
            throw new System.NotImplementedException();
        }
    }
}