using Xunit;
using AutoFixture;
using NSubstitute;

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;

using Budget.Cqrs.Commands.EventProcessors;
using Budget.Users.Application.EventProcessors;
using Budget.Users.Domain.Events;
using Budget.Users.Domain.Model.ReadModel;
using Budget.Users.Domain.Repositories.ReadModelRepositories;

namespace Budget.Users.Application.Tests.EventProcessors
{
    public class UserSubscribedProcessorTests
    {
        private Fixture fixture;
        private CancellationTokenSource tokenSource;
        private IReadModelUserRepository repository;
        private IReadModelUnitOfWork unitOfWork;
        private IRequestHandler<ProcessEventCommand<UserSubscribed>> eventProcessor;

        public UserSubscribedProcessorTests()
        {
            fixture = new Fixture();
            tokenSource = new CancellationTokenSource();
            repository = Substitute.For<IReadModelUserRepository>();
            unitOfWork = Substitute.For<IReadModelUnitOfWork>();
            unitOfWork.UserRepository.Returns(repository);

            eventProcessor = new UserSubscribedProcessor(unitOfWork);
        }

        [Fact]
        public void Handle_SendsUserToReadModelRepository()
        {
            //Arrange
            UserSubscribed @event = new UserSubscribed(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            ProcessEventCommand<UserSubscribed> command = new ProcessEventCommand<UserSubscribed>(
                @event
            );
            
            CancellationToken token = tokenSource.Token;

            //Act
            Task handling = eventProcessor.Handle(command, token);
            handling.Wait();

            //Assert
            unitOfWork.Received(1).BeginTransaction();
            unitOfWork.Received(1).Commit();

            repository.Received(1).Save(Arg.Is<User>(
                u => u.UserName == @event.UserName
                && u.FirstName == @event.FirstName
                && u.LastName == @event.LastName
                && u.Email == @event.Email
            ));
        }

        [Fact]
        public void Handle_RollbacksTransaction_WhenErrorOccur()
        {
            //Arrange
            UserSubscribed @event = new UserSubscribed(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            ProcessEventCommand<UserSubscribed> command = new ProcessEventCommand<UserSubscribed>(
                @event
            );
            
            CancellationToken token = tokenSource.Token;

            repository.Save(Arg.Any<User>()).Returns(u => Task.Run(() => throw new Exception()));

            //Act
            Task handling = eventProcessor.Handle(command, token);
            handling.Wait();

            //Assert
            unitOfWork.Received(1).BeginTransaction();
            unitOfWork.Received(1).Rollback();
        }
    }
}