using System;

using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Tests.Events
{
    public class FakeEvent : Event
    {
        //Constructor for serialization
        public FakeEvent() : base()
        {
            Message = string.Empty;
        }

        public FakeEvent(Guid aggregateId, string message) : this(aggregateId, Guid.NewGuid(), DateTime.Now, message)
        {
        }

        public FakeEvent(Guid aggregateId, Guid eventId, DateTime date, string message) : base(eventId, aggregateId, date)
        {
            Message = message;
        }

        public string Message { get; set; } //Public setter because serialization requires it
    }
}