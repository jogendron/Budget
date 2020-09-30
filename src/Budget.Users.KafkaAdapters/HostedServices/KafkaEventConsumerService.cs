using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Budget.EventSourcing.Services.Serialization;
using Budget.Users.KafkaAdapters.Factories;

using Confluent.Kafka;
using MediatR;

namespace Budget.Users.KafkaAdapters.HostedServices
{
    public class KafkaEventConsumerService : BackgroundService
    {

        public KafkaEventConsumerService(
            IServiceScopeFactory serviceScopeFactory
        )
        {
            ServiceScopeFactory = serviceScopeFactory;
        }

        private IServiceScopeFactory ServiceScopeFactory { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = ServiceScopeFactory.CreateScope())
            {
                IServiceProvider serviceProvider = scope.ServiceProvider;
                
                IKafkaGatewayFactory kafkaGatewayFactory = serviceProvider.GetRequiredService<IKafkaGatewayFactory>();
                IConsumer<Ignore, string> consumer = kafkaGatewayFactory.CreateConsumer();
                IEventSerializer eventSerializer = serviceProvider.GetRequiredService<IEventSerializer>();
                IMediator mediator = serviceProvider.GetRequiredService<IMediator>();

                EventProcessingLoop loop = new EventProcessingLoop(
                    stoppingToken,
                    consumer,
                    eventSerializer,
                    mediator
                );

                await loop.Run();
                consumer.Dispose();
            }
        }
    }
}