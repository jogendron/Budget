using System;

using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Tests.Events
{
    public class UnhandledFakeEvent : Event
    {
        public UnhandledFakeEvent(Guid aggregateId) : base(aggregateId, Guid.NewGuid(), DateTime.Now)
        {
        }
    }
}