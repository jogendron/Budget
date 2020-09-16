using System;
using Budget.EventSourcing.Events;

namespace Budget.EventSourcing.Tests.Events
{
    public class FakeEvent : Event
    {
        //Constructor for serialization
        public FakeEvent() : base()
        {
        }

        public FakeEvent(string message) : this(Guid.NewGuid(), DateTime.Now, message)
        {
        }

        public FakeEvent(Guid id, DateTime date, string message) : base(id, date)
        {
            Message = message;
        }

        public string Message { get; set; } //Public setter because serialization requires it
    }
}