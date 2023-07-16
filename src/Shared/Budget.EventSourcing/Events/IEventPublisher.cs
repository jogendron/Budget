using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Budget.EventSourcing.Entities;

namespace Budget.EventSourcing.Events;

public interface IEventPublisher
{
    Task Publish(Event @event);

    Task Publish(IEnumerable<Event> events);

    Task PublishNewEvents(Aggregate aggregate);
}