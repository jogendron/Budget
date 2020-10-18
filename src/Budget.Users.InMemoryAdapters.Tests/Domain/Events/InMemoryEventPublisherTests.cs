using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using AutoFixture;
using FluentAssertions;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.Events;
using Budget.Users.InMemoryAdapters.Domain.Events;

namespace Budget.Users.InMemoryAdapters.Tests.Domain.Events
{
    public class InMemoryEventPublisherTests
    {
        private Fixture fixture;

        private InMemoryEventStream eventStream;

        private InMemoryEventPublisher eventPublisher;

        public InMemoryEventPublisherTests()
        {
            fixture = new Fixture();
            eventStream = new InMemoryEventStream();
            eventPublisher = new InMemoryEventPublisher(eventStream);
        }

        [Fact]
        public void Publish_SingleEvent_AddsEventToStream()
        {
            //Arrange
            UserSubscribed subscription = new UserSubscribed(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            //Act
            Task publishTask = eventPublisher.Publish(subscription);
            publishTask.Wait();

            //Assert
            Event firstEvent = null;
            eventStream.TryPeek(out firstEvent).Should().BeTrue();
        }

        [Fact]
        public void Publish_EventCollection_AddsAllEventsToStream()
        {
            //Arrange
            UserSubscribed subscription1 = new UserSubscribed(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            UserSubscribed subscription2 = new UserSubscribed(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            List<Event> events = new List<Event>() { subscription1, subscription2 };

            //Act
            Task publishTask = eventPublisher.Publish(events);
            publishTask.Wait();

            //Assert
            Event @event = null;
            eventStream.TryPeek(out @event).Should().BeTrue();
            @event = eventStream.Dequeue();

            eventStream.TryPeek(out @event).Should().BeTrue();
            @event = eventStream.Dequeue();
        }
    }
}