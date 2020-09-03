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
            ProducerConfig config = new ProducerConfig() 
            {
                BootstrapServers = Configuration.GetBootstrapServerString()
            };

            ProducerBuilder<string, string> builder = new ProducerBuilder<string, string>(config);
            
            return builder.Build();
        }
    }
}