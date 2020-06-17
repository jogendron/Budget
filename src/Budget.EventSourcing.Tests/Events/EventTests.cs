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

            //Act
            var @event = new FakeEvent("Dummy");

            //Assert
            @event.Id.Should().NotBe(Guid.Empty);
            @event.Date.Should().BeAfter(DateTime.MinValue);
        }
    }
}
