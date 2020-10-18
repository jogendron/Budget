using System;
using Xunit;
using FluentAssertions;

using Budget.Users.Api.Entities;

namespace Budget.Users.Api.Tests.Entities
{
    public class ProvidersTests
    {
        [Fact]
        public void Event_CanBeSet_ToExpectedInMemory()
        {
            //Arrange
            string inMemory = "InMemory";
            Providers providers = new Providers();

            //Act
            providers.Events = inMemory;

            //Assert
            providers.Events.Should().Be(inMemory);
        }

        [Fact]
        public void Event_CanBeSet_ToKafka()
        {
            //Arrange
            string kafka = "Kafka";
            Providers providers = new Providers();

            //Act
            providers.Events = kafka;

            //Assert
            providers.Events.Should().Be(kafka);
        }

        [Fact]
        public void Event_ThrowsArgumentException_WhenSetToUnexpectedString()
        {
            //Arrange
            string unexpected = "Unexpected";
            Providers providers = new Providers();

            //Act
            Assert.Throws<ArgumentException>(() => providers.Events = unexpected);

            //Assert
        }
    }
}