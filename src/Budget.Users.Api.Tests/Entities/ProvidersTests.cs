using System;
using Xunit;
using FluentAssertions;

using Budget.Users.Api.Entities;

namespace Budget.Users.Api.Tests.Entities
{
    public class ProvidersTests
    {
        [Fact]
        public void Event_CanBeSet()
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
        public void WriteModelPersistence_CanBeSet()
        {
            //Arrange
            string inMemory = "InMemory";
            Providers providers = new Providers();

            //Act
            providers.WriteModelPersistence = inMemory;

            //Assert
            providers.WriteModelPersistence.Should().Be(inMemory);
        }
    }
}