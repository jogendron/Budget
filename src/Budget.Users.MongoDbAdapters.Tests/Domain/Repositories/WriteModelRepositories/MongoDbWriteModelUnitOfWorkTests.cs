using System;
using Xunit;
using NSubstitute;
using FluentAssertions;
using AutoFixture;

using MongoDB.Driver;
using Budget.Users.MongoDbAdapters.Factories;
using Budget.Users.MongoDbAdapters.Entities;
using Budget.Users.MongoDbAdapters.Domain.Repositories.WriteModelRepositories;

using Budget.Users.Domain.Services;
using Budget.Users.Domain.Factories.WriteModelFactories;

namespace Budget.Users.MongoDbAdapters.Tests.Domain.Repositories.WriteModelRepositories
{
    public class MongoDbWriteModelUnitOfWorkTests
    {
        private Fixture fixture;
        private MongoDbConfiguration configuration;
        private IMongoClient client;
        private IClientSessionHandle sessionHandle;
        private IMongoDatabase database;
        private IMongoClientFactory clientFactory;
        private ICryptService cryptService;
        private WriteModelUserFactory userFactory;
        private MongoDbWriteModelUnitOfWork unitOfWork;

        public MongoDbWriteModelUnitOfWorkTests()
        {
            fixture = new Fixture();

            //Setup mongo configuration
            configuration = fixture.Create<MongoDbConfiguration>();

            //Setup client factory
            client = Substitute.For<IMongoClient>();
            sessionHandle = Substitute.For<IClientSessionHandle>();
            database = Substitute.For<IMongoDatabase>();
            clientFactory = Substitute.For<IMongoClientFactory>();

            client.StartSession().Returns(sessionHandle);
            client.GetDatabase(Arg.Any<string>()).Returns(database);
            clientFactory.CreateClient(Arg.Any<MongoDbConfiguration>()).Returns(client);
            
            //Setup user factory
            cryptService = Substitute.For<ICryptService>();
            cryptService.Crypt(Arg.Any<string>()).Returns(args => args[0]);
            userFactory = new WriteModelUserFactory(cryptService);

            //Create unit of work
            unitOfWork = new MongoDbWriteModelUnitOfWork(configuration, clientFactory, userFactory);
        }

        [Fact]
        public void Constructor_GetsDatabase_And_StartsSession()
        {
            //Arrange
            
            //Act

            //Assert
            clientFactory.Received(1).CreateClient(Arg.Is(configuration));
            client.Received(1).GetDatabase(Arg.Is(MongoDbWriteModelUnitOfWork.databaseName));
            client.Received(1).StartSession();
        }

        [Fact]
        public void BeginTransaction_DoesNothings_IfTransactionsAreDisabled()
        {
            //Arrange
            configuration.EnableTransactions = false;

            //Act
            unitOfWork.BeginTransaction();

            //Assert
            sessionHandle.Received(0).StartTransaction();
        }

        [Fact]
        public void BeginTransaction_StartsTransaction_IfTransactionsAreEnabled()
        {
            //Arrange
            configuration.EnableTransactions = true;

            //Act
            unitOfWork.BeginTransaction();

            //Assert
            sessionHandle.Received(1).StartTransaction();
        }

        [Fact]
        public void BeginTransaction_ThrowsInvalidOperationException_WhenTransactionIsAlreadyStarted()
        {
            //Arrange
            configuration.EnableTransactions = true;
            sessionHandle.IsInTransaction.Returns(true);

            //Act
            Action action = (() => unitOfWork.BeginTransaction());

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void Commit_DoesNothing_IfTransactionsAreDisabled()
        {
            //Arrange
            configuration.EnableTransactions = false;

            //Act
            unitOfWork.Commit();

            //Assert
            sessionHandle.Received(0).CommitTransaction();
        }

        [Fact]
        public void Commit_ThrowsInvalidOperationException_IfTransactionEnabledAndNotStarted()
        {
            //Arrange
            configuration.EnableTransactions = true;
            sessionHandle.IsInTransaction.Returns(false);

            //Act
            Action action = (() => unitOfWork.Commit());

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void Commit_CommitsTransaction_IfTransactionEnabledAndStarted()
        {
            //Arrange
            configuration.EnableTransactions = true;
            sessionHandle.IsInTransaction.Returns(true);

            //Act
            unitOfWork.Commit();

            //Assert
            sessionHandle.Received(1).CommitTransaction();
        }

        [Fact]
        public void Rollback_DoesNothing_IfTransactionsAreDisabled()
        {
            //Arrange
            configuration.EnableTransactions = false;

            //Act
            unitOfWork.Rollback();

            //Assert
            sessionHandle.Received(0).AbortTransaction();
        }

        [Fact]
        public void Rollback_ThrowsInvalidOperationException_IfTransactionEnabledAndNotStarted()
        {
            //Arrange
            configuration.EnableTransactions = true;
            sessionHandle.IsInTransaction.Returns(false);

            //Act
            Action action = (() => unitOfWork.Rollback());

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void Rollback_CommitsTransaction_IfTransactionEnabledAndStarted()
        {
            //Arrange
            configuration.EnableTransactions = true;
            sessionHandle.IsInTransaction.Returns(true);

            //Act
            unitOfWork.Rollback();

            //Assert
            sessionHandle.Received(1).AbortTransaction();
        }       
    }
}