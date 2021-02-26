using System;

using Xunit;
using NSubstitute;
using AutoFixture;
using FluentAssertions;

using Budget.Users.PostgresAdapters.Domain.ReadModel.Repositories;
using Budget.Users.PostgresAdapters.Entities;
using Budget.Users.PostgresAdapters.Factories;

namespace Budget.Users.PostgresAdapters.Tests.Domain.ReadModel.Repositories
{
    public class PostgresReadModelUnitOfWorkTests
    {
        private Fixture fixture;
        private IPostgresConnection connection;
        private IPostgresTransaction transaction;
        private IPostgresConnectionFactory connectionFactory;

        public PostgresReadModelUnitOfWorkTests()
        {
            fixture = new Fixture();

            transaction = Substitute.For<IPostgresTransaction>();
            connection = Substitute.For<IPostgresConnection>();

            connection.BeginTransaction().Returns(transaction);

            connectionFactory = Substitute.For<IPostgresConnectionFactory>();
            connectionFactory.Create(Arg.Any<PostgresConfiguration>()).Returns(connection);
        }

        [Fact]
        public void Constructor_OpensConnection_And_SetsUpRepository()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();

            //Act
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);
            
            //Assert
            unitOfWork.UserRepository.Should().NotBeNull();
            connection.Received(1).Open();
        }

        [Fact]
        public void BeginTransaction_CreatesATransaction()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            unitOfWork.BeginTransaction();
            
            //Assert
            connection.Received(1).BeginTransaction();
        }

        [Fact]
        public void BeginTransaction_ThrowsInvalidOperationException_WhenATransactionIsAlreadyOpened()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            unitOfWork.BeginTransaction();
            Action action = (() => unitOfWork.BeginTransaction());           

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void Commit_CommitsAndDisposes_Transaction()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            unitOfWork.BeginTransaction();
            unitOfWork.Commit();

            //Assert
            transaction.Received(1).Commit();
            transaction.Received(1).Dispose();
        }

        [Fact]
        public void Commit_ThrowsInvalidOperationException_WhenNoTransactionIsOpened()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            Action action = (() => unitOfWork.Commit());

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

         [Fact]
        public void Rollback_RollbacksAndDisposes_Transaction()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            unitOfWork.BeginTransaction();
            unitOfWork.Rollback();

            //Assert
            transaction.Received(1).Rollback();
            transaction.Received(1).Dispose();
        }

        [Fact]
        public void Rollback_ThrowsInvalidOperationException_WhenNoTransactionIsOpened()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            Action action = (() => unitOfWork.Rollback());

            //Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public void Dispose_DisposesConnectionAndTransaction()
        {
            //Arrange
            var configuration = fixture.Create<PostgresConfiguration>();
            var unitOfWork = new PostgresReadModelUnitOfWork(configuration, connectionFactory);

            //Act
            unitOfWork.BeginTransaction();
            unitOfWork.Dispose();

            //Assert
            transaction.Received(1).Dispose();
            connection.Received(1).Close();
            connection.Received(1).Dispose();
        }
    }
}