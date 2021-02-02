using Xunit;
using FluentAssertions;
using NSubstitute;

using Budget.Users.Api.Entities;
using Budget.Users.Api.ServiceCollection.EventPublisher;
using Microsoft.Extensions.Configuration;

namespace Budget.Users.Api.Tests.ServiceCollection.Events
{
    public class EventServiceCollectionSelectorTests
    {
        private IConfiguration configuration;

        public EventServiceCollectionSelectorTests()
        {
            configuration = Substitute.For<IConfiguration>();
        }

        [Fact]
        public void GetServiceCollection_WhenCalledWithInMemory_ReturnsInMemoryServices()
        {
            //Arrange
            var providers = new Providers();
            providers.EventPublisher = "InMemory";

            var selector = new EventPublisherSelector(configuration, providers);

            //Act
            var serviceCollection = selector.GetServiceCollection();

            //Assert
            serviceCollection.Should().NotBeNull();
            serviceCollection.Should().BeOfType<InMemoryEventServices>();
        }

        [Fact]
        public void GetServiceCollection_WhenCalledWithKafka_ReturnsKafkaServices()
        {
            //Arrange
            var providers = new Providers();
            providers.EventPublisher = "Kafka";

            var selector = new EventPublisherSelector(configuration, providers);

            //Act
            var serviceCollection = selector.GetServiceCollection();

            //Assert
            serviceCollection.Should().NotBeNull();
            serviceCollection.Should().BeOfType<KafkaEventServices>();
        }

    }
}