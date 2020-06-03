using System;
using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Tests.Events
{
    public class FakeEvent : Event
    {
        public FakeEvent(string message) : this(Guid.NewGuid(), DateTime.Now, message)
        {
        }

        public FakeEvent(Guid id, DateTime date, string message) : base(id, date)
        {
            Message = message;
        }

        public string Message { get; }
    }
}