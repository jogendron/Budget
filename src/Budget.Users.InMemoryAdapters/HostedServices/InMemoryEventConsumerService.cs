using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Budget.Users.InMemoryAdapters.Domain.WriteModel.Events;

using MediatR;

namespace Budget.Users.InMemoryAdapters.HostedServices
{
    public class InMemoryEventConsumerService : BackgroundService
    {
        public InMemoryEventConsumerService(
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

                IMediator mediator = serviceProvider.GetRequiredService<IMediator>();
                InMemoryEventStream eventStream = serviceProvider.GetRequiredService<InMemoryEventStream>();

                EventProcessingLoop eventProcessingLoop = new EventProcessingLoop(
                    stoppingToken,
                    mediator,
                    eventStream
                );

                await eventProcessingLoop.Run();
            }
        }

    }
}