using System;
using Xunit;
using AutoFixture;
using NSubstitute;
using FluentAssertions;

using System.Threading.Tasks;

using Budget.Users.Domain.WriteModel;
using Budget.Users.Domain.WriteModel.Services;
using Budget.Users.Domain.WriteModel.Factories;
using Budget.Users.InMemoryAdapters.Domain.WriteModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Tests.Domain.WriteModel.Repositories
{
    public class InMemoryUserRepositoryTests
    {
        private Fixture fixture;

        private ICryptService cryptService;

        private WriteModelUserFactory userFactory;

        private InMemoryUserWriteData data;

        private InMemoryWriteModelUserRepository repository;

        public InMemoryUserRepositoryTests()
        {
            fixture = new Fixture();

            cryptService = Substitute.For<ICryptService>();
            cryptService.Crypt(Arg.Any<string>()).Returns(args => args[0]);

            userFactory = new WriteModelUserFactory(cryptService);

            data = new InMemoryUserWriteData();

            repository = new InMemoryWriteModelUserRepository(userFactory, data);
        }

        [Fact]
        public void Get_ReturnsNull_WhenUserNotFound()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            //Act
            Task<User> task = repository.Get(id);
            task.Wait();

            User user = task.Result;

            //Assert
            user.Should().BeNull();
        }

        [Fact]
        public void Save_AddsUserToMemory()
        {
            //Arrange
            User user = userFactory.Create(
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                "abc@def.com",
                fixture.Create<string>()
            );

            //Act
            repository.Save(user).Wait();

            Task<User> saveTask = repository.Get(user.Id);
            saveTask.Wait();

            Task<User> getTask = repository.Get(user.Id);
            getTask.Wait();

            User userFromRepository = getTask.Result;

            //Assert
            userFromRepository.UserName.Should().Be(user.UserName);
            userFromRepository.FirstName.Should().Be(user.FirstName);
            userFromRepository.LastName.Should().Be(user.LastName);
            userFromRepository.Email.Should().Be(user.Email);
            userFromRepository.EncryptedPassword.Should().Be(user.EncryptedPassword);
        }
    }
}
