using Xunit;
using AutoFixture;
using FluentAssertions;
using NSubstitute;

using System;
using System.Threading;
using System.Threading.Tasks;

using Budget.Users.Application.Queries.GetUser;
using Budget.Users.Domain.ReadModel;
using Budget.Users.Domain.ReadModel.Repositories;

namespace Budget.Users.Application.Tests.Queries.GetUser
{
    public class GetUserHandlerTests
    {
        Fixture fixture;

        private IReadModelUnitOfWork unitOfWork;

        private IReadModelUserRepository userRepository;

        private GetUserHandler handler;

        public GetUserHandlerTests()
        {
            fixture = new Fixture();

            unitOfWork = Substitute.For<IReadModelUnitOfWork>();
            userRepository = Substitute.For<IReadModelUserRepository>();

            unitOfWork.UserRepository.Returns(userRepository);

            handler = new GetUserHandler(unitOfWork);
        }

        [Fact]
        public void GetUser_CallsRepository_ReturnsUser()
        {
            //Arrange
            string userName = fixture.Create<string>();

            User user = new User(
                Guid.NewGuid(),
                userName,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            userRepository.GetUser(Arg.Is(userName)).Returns(user);

            GetUserRequest request = new GetUserRequest(userName);

            CancellationToken token = new CancellationToken();

            //Act
            Task<User> task = handler.Handle(request, token);
            task.Wait();

            User result = task.Result;

            //Assert
            userRepository.Received(1).GetUser(Arg.Is(userName));

            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(user);
        }
    }
}