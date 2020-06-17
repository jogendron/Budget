using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;

using Budget.EventSourcing.Events;
using Budget.EventSourcing.Exceptions;

using Budget.EventSourcing.Tests.Events;

namespace Budget.EventSourcing.Tests.Entities
{
    public class AggregateTests
    {
        public AggregateTests()
        {
        }

        [Fact]
        public void AddChange_AddsChangeToHistory_AndAppliesChange()
        {
            //Arrange
            const string message = "Hello !";
            var aggregate = new FakeAggregate();

            //Act
            aggregate.TriggerFakeEvent(message);

            //Assert
            aggregate.Changes.Should().NotBeNullOrEmpty();
            var changes = aggregate.Changes.ToList();
            changes.Should().Contain(c => ((FakeEvent)c).Message == message);

            aggregate.MessageLog.Should().Contain(message);
        }

        [Fact]
        public void AddChange_ThrowsEventNotHandledException_WhenEventIsNotHandled()
        {
            //Arrange
            var aggregate = new FakeAggregate();

            //Act
            Action action = (() => aggregate.TriggerUnhandledFakeEvent());

            //Assert
            Assert.Throws<EventNotHandledException>(action);
        }

        [Fact]
        public void AddChange_ThrowsArgumentNullException_WhenEventIsNull()
        {
            //Arrange
            var aggregate = new FakeAggregate();

            //Act
            Action action = (() => aggregate.TriggerNullEvent());

            //Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public void ApplyChangeHistory_Applies_Changes()
        {
            //Arrange
            var event1 = new FakeEvent("First");
            var event2 = new FakeEvent("Second");
            var events = new List<Event>() {event1, event2};
            var aggregate = new FakeAggregate(Guid.NewGuid(), events);

            //Act
            aggregate.ApplyChangeHistory();

            //Assert
            aggregate.Changes.Should().NotBeNullOrEmpty();
            var changes = aggregate.Changes.ToList();
            changes.Should().Contain(event1);
            changes.Should().Contain(event2);

            aggregate.MessageLog.Should().Contain(event1.Message);
            aggregate.MessageLog.Should().Contain(event2.Message);
        }

        [Fact]
        public void ApplyChangeHistory_ThrowsEventNotHandledException_WhenOneEventIsNotHandled()
        {
            //Arrange
            var event1 = new UnhandledFakeEvent();
            var events = new List<Event>() { event1 };
            var aggregate = new FakeAggregate(Guid.NewGuid(), events);

            //Act
            Action action = (() => aggregate.ApplyChangeHistory());

            //Assert
            Assert.Throws<EventNotHandledException>(action);
        }

        [Fact]
        public void ApplyChangeHistory_AppliesEventBeforeUpToParameter_IfSupplied()
        {
            //Arrange
            var event1 = new FakeEvent(Guid.NewGuid(), DateTime.MinValue, "first");
            var event2 = new FakeEvent(Guid.NewGuid(), DateTime.Now.AddDays(-10), "second");
            var event3 = new FakeEvent(Guid.NewGuid(), DateTime.Now.AddDays(-5), "third");
            var event4 = new FakeEvent(Guid.NewGuid(), DateTime.MaxValue, "fourth");
            var events = new List<Event>() {event1, event2, event3, event4};
            var aggregate = new FakeAggregate(Guid.NewGuid(), events);

            //Act
            aggregate.ApplyChangeHistory(DateTime.Now);

            //Assert
            aggregate.MessageLog.Should().NotBeNullOrEmpty();
            aggregate.MessageLog.Should().Contain(event1.Message);
            aggregate.MessageLog.Should().Contain(event2.Message);
            aggregate.MessageLog.Should().Contain(event3.Message);
            aggregate.MessageLog.Should().NotContain(event4.Message);
        }
    }
}