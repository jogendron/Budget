using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using NSubstitute;
using FluentAssertions;
using AutoFixture;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.Events;
using Budget.Users.Domain.Services;
using Budget.Users.Domain.Factories.WriteModelFactories;
using Budget.Users.MongoDbAdapters.Domain.Repositories.WriteModelRepositories;
using Budget.Users.MongoDbAdapters.Entities;

using MongoDB.Driver;

namespace Budget.Users.MongoDbAdapters.Tests.Domain.Repositories.WriteModelRepositories
{
    public class MongoDbWriteModelUserRepositoryTests
    {
        private Fixture fixture;
        private IMongoDatabase database;
        private IMongoCollection<User> userCollection;
        private ICryptService cryptService;
        private WriteModelUserFactory userFactory;
        private MongoDbWriteModelUserRepository repository;

        public MongoDbWriteModelUserRepositoryTests()
        {
            fixture = new Fixture();
            fixture.Customize<UserSubscribed>(composer => composer.With(u => u.Email, "john@doe.com"));

            database = Substitute.For<IMongoDatabase>();
            userCollection = Substitute.For<IMongoCollection<User>>();
            database.GetCollection<User>(Arg.Is(MongoDbWriteModelUserRepository.collectionName)).Returns(userCollection);

            cryptService = Substitute.For<ICryptService>();
            cryptService.Crypt(Arg.Any<string>()).Returns(args => args[0]);
            userFactory = new WriteModelUserFactory(cryptService);

            repository = new MongoDbWriteModelUserRepository(database, userFactory);
        }

        [Fact]
        public void Constructor_GetsUserCollection()
        {
            //Arrange

            //Act

            //Assert
            database.Received(1).GetCollection<User>(Arg.Is(MongoDbWriteModelUserRepository.collectionName));
        }

        [Fact]
        public void Get_ReturnsNull_WhenUserDoesNotExist()
        {
            //Arrange
            IAsyncCursor<User> cursor = new FakeAsyncCursor<User>();

            var findTask = Task.FromResult(cursor);
            userCollection.FindAsync<User>(Arg.Any<FilterDefinition<User>>()).Returns(findTask);

            //Act
            var getUserTask = repository.Get(Guid.NewGuid());
            getUserTask.Wait();
            var user = getUserTask.Result;

            //Assert
            user.Should().BeNull();
        }

        [Fact]
        public void Get_LoadsAndReturnsUser_WhenFound()
        {
            //Arrange
            User user = new User();
            user.Id = Guid.NewGuid();
            user.Changes = new List<Event>() {
                fixture.Create<UserSubscribed>(),
                fixture.Create<PasswordChanged>()
            };

            List<User> users = new List<User>() { user };
            IAsyncCursor<User> cursor = new FakeAsyncCursor<User>(users);

            var findTask = Task.FromResult(cursor);
            userCollection.FindAsync<User>(Arg.Any<FilterDefinition<User>>()).Returns(findTask);

            //Act
            var getUserTask = repository.Get(Guid.NewGuid());
            getUserTask.Wait();
            var domainUser = getUserTask.Result;

            //Assert
            domainUser.Should().NotBeNull();
            domainUser.Changes.Count().Should().Be(user.Changes.Count());
        }

        [Fact]
        public void Save_InsertsUser_WhenIsNewUser()
        {
            //Arrange
            var domainUser = userFactory.Create(
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                "john@doe.com",
                fixture.Create<string>()
            );
  
            //Act
            var saveTask = repository.Save(domainUser);
            saveTask.Wait();

            //Assert
            userCollection.Received(1).InsertOneAsync(Arg.Is<User>(
                u => u.Changes.Count() == domainUser.Changes.Count()
            ));
        }

        [Fact]
        public void Save_ReplacesUser_WhenIsNotNewUser()
        {
            //Arrange
            var domainUser = userFactory.Load(Guid.NewGuid(), new List<Event>() {
                fixture.Create<UserSubscribed>(),
                fixture.Create<PasswordChanged>()
            });

            //Act
            var saveTask = repository.Save(domainUser);
            saveTask.Wait();

            //Assert
            userCollection.Received(1).FindOneAndReplaceAsync<User>(Arg.Any<FilterDefinition<User>>(), Arg.Any<User>());
        }

    }
}