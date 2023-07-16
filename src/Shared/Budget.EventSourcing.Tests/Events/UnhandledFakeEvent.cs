using System;

using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Tests.Events
{
    public class UnhandledFakeEvent : Event
    {
        public UnhandledFakeEvent(Guid aggregateId) : base(Guid.NewGuid(), aggregateId, DateTime.Now)
        {
        }
    }
}