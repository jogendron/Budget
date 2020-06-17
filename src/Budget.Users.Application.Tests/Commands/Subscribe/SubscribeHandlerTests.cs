using Xunit;
using AutoFixture;
using NSubstitute;

using System;
using System.Threading;
using System.Threading.Tasks;

using Budget.Users.Application.Commands.Subscribe;
using Budget.Users.Application.Exceptions;
using Budget.EventSourcing.Events;
using Budget.Users.Domain.Model.WriteModel;
using Budget.Users.Domain.Factories.WriteModelFactories;
using Budget.Users.Domain.Repositories.ReadModelRepositories;
using Budget.Users.Domain.Repositories.WriteModelRepositories;
using Budget.Users.Domain.Services;

using MediatR;

namespace Budget.Users.Application.Tests.Commands.Subscribe
{
    public class SubscribeHandlerTests
    {
        private Fixture fixture;
        private ICryptService cryptService;
        private string cryptServiceOutput;
        private WriteModelUserFactory userFactory;
        private IReadModelUserRepository userReadRepository;
        private IReadModelUnitOfWork readModelUnitOfWork;
        private IWriteModelUserRepository userWriteRepository;
        private IWriteModelUnitOfWork writeModelUnitOfWork;
        private IEventPublisher eventPublisher;
        private IRequestHandler<SubscribeCommand> handler;

        public SubscribeHandlerTests()
        {
            fixture = new Fixture();

            cryptService = Substitute.For<ICryptService>();
            cryptServiceOutput = fixture.Create<string>();
            cryptService.Crypt(Arg.Any<string>()).Returns(cryptServiceOutput);

            userFactory = new WriteModelUserFactory(cryptService);
            
            userReadRepository = Substitute.For<IReadModelUserRepository>();
            readModelUnitOfWork = Substitute.For<IReadModelUnitOfWork>();
            readModelUnitOfWork.UserRepository.Returns(userReadRepository);

            userWriteRepository = Substitute.For<IWriteModelUserRepository>();
            writeModelUnitOfWork = Substitute.For<IWriteModelUnitOfWork>();
            writeModelUnitOfWork.UserRepository.Returns(userWriteRepository);

            eventPublisher = Substitute.For<IEventPublisher>();

            handler = new SubscribeHandler(
                userFactory, 
                readModelUnitOfWork, 
                writeModelUnitOfWork, 
                eventPublisher
            );
        }

        [Fact]
        public void HandleSubscribe_CreatesUser_ThenSavesItAndPublishesEvents()
        {
            //Arrange
            SubscribeCommand command = new SubscribeCommand(
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                "abc@def.com",
                fixture.Create<string>()
            );

            CancellationToken token = new CancellationToken();

            //Act
            Task task = handler.Handle(command, token);
            task.Wait();

            //Assert
            cryptService.Received(1).Crypt(Arg.Is(command.Password)); //Called by the factory

            writeModelUnitOfWork.Received(1).BeginTransaction();
            writeModelUnitOfWork.Received(1).Commit();

            userWriteRepository.Received(1).Save(Arg.Is<User>(u => 
                u.UserName == command.UserName
                && u.FirstName == command.FirstName 
                && u.LastName == command.LastName
                && u.Email.Address == command.Email
            ));

            eventPublisher.Received(1).PublishNewEvents(Arg.Is<User>(u => u.UserName == command.UserName));
        }

        [Fact]
        public void HandleSubscribe_RollbacksTransaction_WhenExceptionOccur()
        {
            //Arrange
            SubscribeCommand command = new SubscribeCommand(
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                "wololoo",
                fixture.Create<string>()
            );

            CancellationToken token = new CancellationToken();

            //Act
            handler.Handle(command, token);

            //Assert
            writeModelUnitOfWork.Received(1).BeginTransaction();
            writeModelUnitOfWork.Received(1).Rollback();
        }

        [Fact]
        public void HandleSubscribe_ThrowsUserAlreadyExistsException_WhenUserExistsAlready()
        {
            //Arrange
            SubscribeCommand command = new SubscribeCommand(
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                "abc@def.com",
                fixture.Create<string>()
            );

            CancellationToken token = new CancellationToken();

            userReadRepository.Exists(Arg.Is(command.UserName)).Returns(true);

            //Act
            Assert.ThrowsAsync<UserAlreadyExistsException>(() => handler.Handle(command, token));

            //Assert
        }
    }
}