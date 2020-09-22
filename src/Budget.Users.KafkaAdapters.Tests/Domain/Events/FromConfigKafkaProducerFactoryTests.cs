using Xunit;
using AutoFixture;
using FluentAssertions;

using Budget.Users.KafkaAdapters.Domain.Events;
using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Tests.Domain.Events
{

    public class FromConfigKafkaProducerFactoryTests
    {
        Fixture fixture;
        private KafkaConfiguration configuration;


        public FromConfigKafkaProducerFactoryTests()
        {
            fixture = new Fixture();
            configuration = fixture.Create<KafkaConfiguration>();
        }

        [Fact]
        public void Create_InitializesProducer()
        {
            //Arrange
            FromConfigKafkaProducerFactory factory = new FromConfigKafkaProducerFactory(configuration);

            //Act
            IProducer<string, string> producer = factory.Create();

            //Assert
            producer.Should().NotBeNull();
        }

    }

}