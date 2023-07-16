using System.Collections.Generic;
using System.Threading.Tasks;

using Budget.EventSourcing.Entities;

namespace Budget.EventSourcing.Events;

public abstract class EventPublisherBase : IEventPublisher
{
    public abstract Task Publish(Event @event);

    public abstract Task Publish(IEnumerable<Event> events);

    public async Task PublishNewEvents(Aggregate aggregate)
    {
        await Publish(aggregate.NewChanges);
    }
}