using System;
using System.Collections.Generic;

using Budget.EventSourcing.Events;

namespace Budget.Users.InMemoryAdapters.Domain.WriteModel.Events
{
    public class InMemoryEventStream : Queue<Event>
    {
    }
}