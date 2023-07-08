using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Budget.EventSourcing.Entities;
using Budget.EventSourcing.Events;
using Budget.EventSourcing.Tests.Events;

namespace Budget.EventSourcing.Tests.Entities
{
    public class FakeAggregate : Aggregate, IEventHandler<FakeEvent>
    {

        public FakeAggregate() : base()
        {
            InitializeMembers();
        }

        public FakeAggregate(Guid id, IEnumerable<Event> changes) : base(id, changes)
        {
            InitializeMembers();
        }

        public List<string> MessageLog { get; private set;}

        [MemberNotNull(nameof(MessageLog))]
        protected override void InitializeMembers()
        {
            MessageLog = new List<string>();
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