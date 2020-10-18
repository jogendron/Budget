using Confluent.Kafka;
using Budget.Users.KafkaAdapters.Entities;

namespace Budget.Users.KafkaAdapters.Factories
{
    public class FromConfigKafkaGatewayFactory : IKafkaGatewayFactory
    {
        public FromConfigKafkaGatewayFactory(KafkaConfiguration configuration)
        {
            Configuration = configuration;
        }

        private KafkaConfiguration Configuration { get; }

        public IConsumer<Ignore, string> CreateConsumer()
        {
            const string groupId = "Budget.Users";

            ConsumerConfig config = new ConsumerConfig()
            {
                BootstrapServers = Configuration.GetBootstrapServerString(),
                GroupId = groupId,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };

            ConsumerBuilder<Ignore, string> builder = new ConsumerBuilder<Ignore, string>(config);

            return builder.Build(); 
        }

        public IProducer<string, string> CreateProducer()
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