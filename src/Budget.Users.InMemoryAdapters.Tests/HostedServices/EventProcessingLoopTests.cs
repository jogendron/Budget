using Xunit;
using AutoFixture;
using FluentAssertions;
using NSubstitute;

using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Budget.Users.InMemoryAdapters.Domain.WriteModel.Events;
using Budget.Users.InMemoryAdapters.HostedServices;
using Budget.Users.Domain.WriteModel.Events;
using Budget.Cqrs.Commands.EventProcessors;

namespace Budget.Users.InMemoryAdapters.Tests.HostedServices
{
    public class EventProcessingLoopTests
    {
        private Fixture fixture;
        private IMediator mediator;
        private InMemoryEventStream eventStream;        
        private CancellationTokenSource tokenSource;
        private EventProcessingLoop loop;


        public EventProcessingLoopTests()
        {
            fixture = new Fixture();
            mediator = Substitute.For<IMediator>();
            eventStream = new InMemoryEventStream();
            tokenSource = new CancellationTokenSource();

            loop = new EventProcessingLoop(tokenSource.Token, mediator, eventStream);
        }

        [Fact]
        public void ExecuteAsync_PullsEvents_AndSendThemToMediator()
        {
            //Arrange
            eventStream.Enqueue(fixture.Create<UserSubscribed>());
            eventStream.Enqueue(fixture.Create<PasswordChanged>());
            
            CancellationToken token = tokenSource.Token;
            
            //Act
            Task eventProcessing = loop.Run();
            for (int i = 0; i < 5 && eventStream.Count > 0; i++)
                Task.Delay(100).Wait();
            tokenSource.Cancel();
            eventProcessing.Wait();

            //Assert
            eventStream.Count.Should().Be(0);
            mediator.Received(1).Send(Arg.Is<object>(o => o is ProcessEventCommand<UserSubscribed>));
            mediator.Received(1).Send(Arg.Is<object>(o => o is ProcessEventCommand<PasswordChanged>));
        }

    }
}