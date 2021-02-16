using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Budget.EventSourcing.Events;

namespace Budget.Users.InMemoryAdapters.Domain.WriteModel.Events
{
    public class InMemoryEventPublisher : EventPublisherBase
    {
        public InMemoryEventPublisher(InMemoryEventStream eventStream)
        {
            EventStream = eventStream;
        }

        private InMemoryEventStream EventStream { get; }

        public override async Task Publish(Event @event)
        {
            await Task.CompletedTask;
            EventStream.Enqueue(@event);
        }

        public override async Task Publish(IEnumerable<Event> events)
        {
            foreach (Event @event in events)
                await Publish(@event);
        }
    }
}