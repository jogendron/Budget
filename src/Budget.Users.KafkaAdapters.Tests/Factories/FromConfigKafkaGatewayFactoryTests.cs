using Xunit;
using AutoFixture;
using FluentAssertions;
using Budget.Users.KafkaAdapters.Entities;
using Budget.Users.KafkaAdapters.Factories;
using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Tests.Factories
{

    public class FromConfigKafkaGatewayFactoryTests
    {
        private Fixture fixture;
        private KafkaConfiguration configuration;

        public FromConfigKafkaGatewayFactoryTests()
        {
            fixture = new Fixture();
            configuration = fixture.Create<KafkaConfiguration>();
        }

        [Fact]
        public void CreateProducer_InitializesProducer()
        {
            //Arrange
            FromConfigKafkaGatewayFactory factory = new FromConfigKafkaGatewayFactory(configuration);

            //Act
            IProducer<string, string> producer = factory.CreateProducer();

            //Assert
            producer.Should().NotBeNull();
        }

        [Fact]
        public void CreateConsumer_InitializesConsumer()
        {
            //Arrange
            FromConfigKafkaGatewayFactory factory = new FromConfigKafkaGatewayFactory(configuration);

            //Act
            IConsumer<Ignore, string> consumer = factory.CreateConsumer();

            //Assert
            consumer.Should().NotBeNull();
        }

    }

}