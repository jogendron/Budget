using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.EventPublisher
{
    public class KafkaEventServices : IEventServices
    {
        public KafkaEventServices(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; set;}

        public void Configure(IServiceCollection services)
        {
            var kafkaConfiguration = new Budget.Users.KafkaAdapters.Entities.KafkaConfiguration();
            Configuration.GetSection("Kafka").Bind(kafkaConfiguration);
            services.AddSingleton(kafkaConfiguration);

            services.AddTransient(
                typeof(Budget.Users.KafkaAdapters.Factories.IKafkaGatewayFactory),
                typeof(Budget.Users.KafkaAdapters.Factories.FromConfigKafkaGatewayFactory)
            );

            services.AddTransient(
                typeof(Budget.EventSourcing.Services.Serialization.IEventSerializer),
                typeof(Budget.EventSourcing.Services.Serialization.Json.JsonEventSerializer)
            );

            services.AddTransient(
                typeof(Budget.EventSourcing.Events.IEventPublisher),
                typeof(Budget.Users.KafkaAdapters.Domain.Events.KafkaEventPublisher)
            );

            services.AddHostedService<Budget.Users.KafkaAdapters.HostedServices.KafkaEventConsumerService>();
        }
    }
}