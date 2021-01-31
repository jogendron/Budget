using Microsoft.Extensions.DependencyInjection;

namespace Budget.Users.Api.ServiceCollection.Events
{
    public class InMemoryEventServiceCollection : IEventServiceCollection
    {
        public void Configure(IServiceCollection services)
        {
            services.AddTransient(
                typeof(Budget.EventSourcing.Events.IEventPublisher),
                typeof(Budget.Users.InMemoryAdapters.Domain.Events.InMemoryEventPublisher)
            );

            services.AddSingleton(
                typeof(Budget.Users.InMemoryAdapters.Domain.Events.InMemoryEventStream),
                typeof(Budget.Users.InMemoryAdapters.Domain.Events.InMemoryEventStream)
            );            

            services.AddHostedService<Budget.Users.InMemoryAdapters.HostedServices.InMemoryEventConsumerService>();
        }
    }
}