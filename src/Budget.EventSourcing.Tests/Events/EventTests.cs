using System;
using Xunit;
using FluentAssertions;

namespace Budget.EventSourcing.Tests.Events
{
    public class EventTests
    {
        [Fact]
        public void Constructor_Assigns_Properties()
        {
            //Arrange
            Guid aggregateId = Guid.NewGuid();

            //Act
            var @event = new FakeEvent(aggregateId, "Dummy");

            //Assert
            @event.AggregateId.Should().Be(aggregateId);
            @event.Id.Should().NotBe(Guid.Empty);
            @event.Date.Should().BeAfter(DateTime.MinValue);
        }
    }
}
