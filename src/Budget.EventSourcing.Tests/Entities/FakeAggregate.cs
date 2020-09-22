using System;
using System.Collections.Generic;
using Budget.EventSourcing.Entities;
using Budget.EventSourcing.Events;

using Budget.EventSourcing.Tests.Events;

namespace Budget.EventSourcing.Tests.Entities
{
    public class FakeAggregate : Aggregate, IEventHandler<FakeEvent>
    {
        public FakeAggregate() : base()
        {
            MessageLog = new List<string>();
        }

        public FakeAggregate(Guid id, List<Event> changes) : base(id, changes)
        {
            MessageLog = new List<string>();
        }

        public List<string> MessageLog { get; }

        public new IEnumerable<Event> Changes 
        {
            get { return base.Changes; }
        }

        public void TriggerFakeEvent(string message)
        {
            AddChange(new FakeEvent(Id, message));
        }

        public void TriggerUnhandledFakeEvent()
        {
            AddChange(new UnhandledFakeEvent(Id));
        }

        public void TriggerNullEvent()
        {
            AddChange(null);
        }
        
        public void Handle(FakeEvent @event)
        {
            MessageLog.Add(@event.Message);
        }

        
    }
}