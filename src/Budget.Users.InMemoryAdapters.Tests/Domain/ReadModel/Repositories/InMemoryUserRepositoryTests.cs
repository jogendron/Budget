using System;
using System.Threading.Tasks;

using Xunit;
using FluentAssertions;
using AutoFixture;

using Budget.Users.Domain.ReadModel;
using Budget.Users.InMemoryAdapters.Domain.ReadModel.Repositories;

namespace Budget.Users.InMemoryAdapters.Tests.Domain.ReadModel.Repositories
{
    public class InMemoryUserRepositoryTests
    {
        private Fixture fixture;
        InMemoryUserReadData data;
        private InMemoryReadModelUserRepository repository;

        public InMemoryUserRepositoryTests()
        {
            fixture = new Fixture();
            data = new InMemoryUserReadData();
            repository = new InMemoryReadModelUserRepository(data);
        }

        [Fact]
        public void Exists_ReturnsTrueWhenUserExists_FalseOtherwhise()
        {
            //Arrange
            User user = new User(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            string nonExistentUserName = fixture.Create<string>();

            //Act
            Task saveTask = repository.Save(user);
            saveTask.Wait();

            //Assert
            Task<bool> existenceCheck = repository.Exists(user.UserName);
            existenceCheck.Wait();
            existenceCheck.Result.Should().BeTrue();

            Task<bool> otherExistenceCheck = repository.Exists(nonExistentUserName);
            otherExistenceCheck.Wait();
            otherExistenceCheck.Result.Should().BeFalse();
        }

        [Fact]
        public void Save_AddsNewUser()
        {
            //Arrange
            User user = new User(
                Guid.NewGuid(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            //Act
            Task saveTask = repository.Save(user);
            saveTask.Wait();

            //Assert
            Task<User> readTask = repository.GetUser(user.UserName);
            readTask.Wait();

            User userFromRepository = readTask.Result;
            userFromRepository.Should().BeEquivalentTo(user);
        }

        [Fact]
        public void Save_OverwritesExistingUser()
        {
            //Arrange
            string username = fixture.Create<string>();

            User user1 = new User(
                Guid.NewGuid(),
                username,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            User user2 = new User(
                Guid.NewGuid(),
                username,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            //Act
            Task saveTask = repository.Save(user1);
            saveTask.Wait();

            saveTask = repository.Save(user2);
            saveTask.Wait();

            //Assert
            Task<User> readTask = repository.GetUser(username);
            readTask.Wait();

            User userFromRepository = readTask.Result;
            userFromRepository.Should().BeEquivalentTo(user2);
        }
    }
}