using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.EventPublisher
{
    public class InMemoryEventServices : IEventServices
    {
        public void Configure(IServiceCollection services)
        {
            services.AddTransient(
                typeof(Budget.EventSourcing.Events.IEventPublisher),
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Events.InMemoryEventPublisher)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Events.InMemoryEventStream),
                typeof(Budget.Users.InMemoryAdapters.Domain.WriteModel.Events.InMemoryEventStream)
            );            

            services.AddHostedService<Budget.Users.InMemoryAdapters.HostedServices.InMemoryEventConsumerService>();
        }
    }
}