using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Xunit;
using NSubstitute;
using AutoFixture;
using FluentAssertions;

using Budget.Cqrs.Commands.EventProcessors;
using Budget.EventSourcing.Services.Serialization;
using Budget.EventSourcing.Services.Serialization.Json;
using Budget.Users.KafkaAdapters.HostedServices;
using Budget.Users.KafkaAdapters.Entities;
using Budget.Users.Domain.WriteModel.Events;

using Confluent.Kafka;
using MediatR;


namespace Budget.Users.KafkaAdapters.Tests.HostedServices
{
    public class EventProcessingLoopTests
    {
        private Fixture fixture;
        private CancellationTokenSource tokenSource;
        private IConsumer<Ignore, string> consumer;
        private IEventSerializer eventSerializer;
        private IMediator mediator;
        private EventProcessingLoop loop;

        public EventProcessingLoopTests()
        {
            fixture = new Fixture();

            tokenSource = new CancellationTokenSource();
            consumer = Substitute.For<IConsumer<Ignore, string>>();
            eventSerializer = new JsonEventSerializer();
            mediator = Substitute.For<IMediator>();

            loop = new EventProcessingLoop(tokenSource.Token, consumer, eventSerializer, mediator);
        }

        [Fact]
        public void EventProcessingLoop_SubscribesTo_UserEventSourcing_Topic()
        {
            //Arrange
            tokenSource.Cancel();

            //Act
            loop.Run().Wait();

            //Assert
            consumer.Received(1).Subscribe(
                Arg.Is<string>(KafkaTopics.UserEventSourcing)
            );
        }

        [Fact]
        public void EventProcessingLoop_PullsEventFromConsumer_And_SendsItToMediator()
        {
            //Arrange
            UserSubscribed subscription = fixture.Create<UserSubscribed>();
            string jsonSubscription = eventSerializer.Serialize(subscription);

            consumer.Consume(Arg.Any<CancellationToken>()).Returns(
                new ConsumeResult<Ignore, string>()  {
                    Message = new Message<Ignore, string>()  {
                        Value = jsonSubscription
                    }
            });

            consumer.When(c => c.Consume(Arg.Any<CancellationToken>())).Do(
                c => {
                    tokenSource.Cancel();
                }
            );

            //Act
            loop.Run().Wait();

            //Assert
            mediator.Received(1).Send(
                Arg.Is<object>(
                    o => o is ProcessEventCommand<UserSubscribed>
                )
            );
            
        }
    }
}