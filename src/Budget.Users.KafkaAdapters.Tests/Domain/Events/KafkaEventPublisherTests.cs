using Xunit;
using NSubstitute;
using AutoFixture;

using Budget.EventSourcing.Events;
using Budget.EventSourcing.Services.Serialization;
using Budget.Users.Domain.Events;
using Budget.Users.KafkaAdapters.Domain.Events;
using Budget.Users.KafkaAdapters.Entities;
using Budget.Users.KafkaAdapters.Factories;
using Confluent.Kafka;

namespace Budget.Users.KafkaAdapters.Tests.Domain.Events
{
    public class KafkaEventPublisherTests
    {
        private Fixture fixture;

        private IKafkaGatewayFactory kafkaGatewayFactory;

        private IProducer<string, string> producer;

        private IEventSerializer eventSerializer;

        KafkaEventPublisher eventPublisher;

        public KafkaEventPublisherTests()
        {
            fixture = new Fixture();

            producer = Substitute.For<IProducer<string, string>>();
            
            kafkaGatewayFactory = Substitute.For<IKafkaGatewayFactory>();
            kafkaGatewayFactory.CreateProducer().Returns(producer);

            eventSerializer = Substitute.For<IEventSerializer>();
            eventSerializer.Serialize<Event>(Arg.Any<Event>()).Returns("We are the knight who say Ni");

            eventPublisher = new KafkaEventPublisher(kafkaGatewayFactory, eventSerializer);
        }

        [Fact]
        public void Publish_SendsUserSubscribedEvent_ToEventSourcingTopic_AndPublicTopic()
        {
            //Arrange
            Event subscription = fixture.Create<UserSubscribed>();
            Event passwordChange = fixture.Create<PasswordChanged>();

            //Act
            eventPublisher.Publish(new[] { subscription, passwordChange }).Wait();

            //Assert
            eventSerializer.Received(1).Serialize(Arg.Any<UserSubscribed>());

            producer.Received(1).ProduceAsync(
                Arg.Is(KafkaTopics.UserEventSourcing),
                Arg.Is<Message<string, string>>(
                    m => m.Key == subscription.AggregateId.ToString()
                        && ! string.IsNullOrEmpty(m.Value)
                )
            );

            producer.Received(1).ProduceAsync(
                Arg.Is(KafkaTopics.UserPublicEvents),
                Arg.Is<Message<string, string>>(
                    m => m.Key == subscription.AggregateId.ToString()
                        && ! string.IsNullOrEmpty(m.Value)
                )
            );

            producer.Received(1).ProduceAsync(
                Arg.Is(KafkaTopics.UserEventSourcing),
                Arg.Is<Message<string, string>>(
                    m => m.Key == passwordChange.AggregateId.ToString()
                        && ! string.IsNullOrEmpty(m.Value)
                )
            );
        }

        [Fact]
        public void Publish_SendEventToEventSourcingTopicOnly_WhenEventIsNotSubcription()
        {
            Event @event = fixture.Create<PasswordChanged>();

            //Act
            eventPublisher.Publish(new[] { @event }).Wait();

            //Assert
            eventSerializer.Received(1).Serialize(Arg.Any<PasswordChanged>());

            producer.Received(1).ProduceAsync(
                Arg.Is(KafkaTopics.UserEventSourcing),
                Arg.Is<Message<string, string>>(
                    m => m.Key == @event.AggregateId.ToString()
                        && ! string.IsNullOrEmpty(m.Value)
                )
            );

            producer.Received(0).ProduceAsync(
                Arg.Is(KafkaTopics.UserPublicEvents),
                Arg.Any<Message<string, string>>()
            );
        }

    }
}