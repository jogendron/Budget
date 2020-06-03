using System;

using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Tests.Events
{
    public class UnhandledFakeEvent : Event
    {
        public UnhandledFakeEvent() : base(Guid.NewGuid(), DateTime.Now)
        {
        }
    }
}