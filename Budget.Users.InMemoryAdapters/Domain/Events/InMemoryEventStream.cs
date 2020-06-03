using System;
using System.Collections.Generic;

using Budget.EventSourcing.Events;

namespace Budget.Users.InMemoryAdapters.Domain.Events
{
    public class InMemoryEventStream : Queue<Event>
    {
    }
}