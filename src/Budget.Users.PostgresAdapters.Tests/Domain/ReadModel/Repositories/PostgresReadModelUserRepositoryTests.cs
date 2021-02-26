using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

using Xunit;
using NSubstitute;
using AutoFixture;
using FluentAssertions;

using Budget.Users.PostgresAdapters.Domain.ReadModel.Repositories;
using Budget.Users.PostgresAdapters.Entities;
using Budget.Users.Domain.ReadModel;

namespace Budget.Users.PostgresAdapters.Tests.Domain.ReadModel.Repositories
{

    public class PostgresReadModelUserRepositoryTests
    {
        private Fixture fixture;
        private IPostgresConnection connection;
        private IPostgresTransaction transaction;
        private IPostgresCommand command;
        private IPostgresParameterCollection parameterCollection;
        private PostgresReadModelUserRepository repository;


        public PostgresReadModelUserRepositoryTests()
        {
            fixture = new Fixture();

            parameterCollection = Substitute.For<IPostgresParameterCollection>();
            command = Substitute.For<IPostgresCommand>();
            transaction = Substitute.For<IPostgresTransaction>();
            connection = Substitute.For<IPostgresConnection>();

            command.CreateParameter().Returns(args => Substitute.For<IPostgresParameter>());
            command.Parameters.Returns(parameterCollection);
            connection.CreateCommand().Returns(command);
            connection.BeginTransaction().Returns(transaction);

            repository = new PostgresReadModelUserRepository(connection);
        }

        [Fact]
        public void Exists_SendsValidCommand_AndReadsResultsCorrectly()
        {
            //Arrange
            string username = fixture.Create<string>();
            IDataReader reader = Substitute.For<IDataReader>();
            reader.Read().Returns(true, false);
            reader.GetOrdinal(Arg.Is("count")).Returns(0);
            reader.GetInt32(Arg.Is(0)).Returns(1);

            command.ExecuteReaderAsync().Returns(Task.FromResult(reader));

            repository.SetTransaction(transaction);

            //Act
            var existenceCheck =  repository.Exists(username);
            existenceCheck.Wait();
            var exists = existenceCheck.Result;

            //Assert
            exists.Should().BeTrue();

            command.CommandText.Should().Be("SELECT count(0) AS count from users.users WHERE username = :p_username");

            parameterCollection.Received(1).Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_username"
                        && p.Value != null
                        && ((string)p.Value) == username
                )
            );

            command.Transaction.Should().Be(transaction);

            command.Received(1).PrepareAsync();

            command.Received(1).ExecuteReaderAsync();
        }

        [Fact]
        public void GetUser_SendsValidCommand_AndReadsResultsCorrectly()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string username = fixture.Create<string>();
            string firstname = fixture.Create<string>();
            string lastname = fixture.Create<string>();
            string email = fixture.Create<string>();

            IDataReader reader = Substitute.For<IDataReader>();
            reader.Read().Returns(true, false);
            reader.GetOrdinal("id").Returns(0);
            reader.GetOrdinal("username").Returns(1);
            reader.GetOrdinal("firstname").Returns(2);
            reader.GetOrdinal("lastname").Returns(3);
            reader.GetOrdinal("email").Returns(4);

            reader.GetGuid(0).Returns(id);
            reader.GetString(1).Returns(username);
            reader.GetString(2).Returns(firstname);
            reader.GetString(3).Returns(lastname);
            reader.GetString(4).Returns(email);
            
            command.ExecuteReaderAsync().Returns(reader);

            repository.SetTransaction(transaction);

            //Act
            var getTask = repository.GetUser(username);
            getTask.Wait();
            var user = getTask.Result;

            //Assert
            command.CommandText.Should().Be("SELECT id, username, firstname, lastname, email FROM users.users WHERE username = :p_username");

            parameterCollection.Received(1).Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_username"
                        && p.Value != null
                        && ((string)p.Value) == username
                )
            );

            command.Transaction.Should().Be(transaction);

            command.Received(1).PrepareAsync();

            command.Received(1).ExecuteReaderAsync();

            user.Should().NotBeNull();
            user.Id.Should().Be(id);
            user.UserName.Should().Be(username);
            user.FirstName.Should().Be(firstname);
            user.LastName.Should().Be(lastname);
            user.Email.Should().Be(email);
        }

        [Fact]
        public void Save_InsertsUser_IfUserDoesNotExists()
        {
            //Arrange
            var user = fixture.Create<User>();

            repository.SetTransaction(transaction);

            //Act
            var saveTask = repository.Save(user);
            saveTask.Wait();

            //Assert
            command.CommandText.Should().Be(@"
                    INSERT INTO users.users (
                        id, 
                        username, 
                        firstname, 
                        lastname, 
                        email
                    ) values (
                        :p_id, 
                        :p_username, 
                        :p_firstname, 
                        :p_lastname, 
                        :p_email
                    )
                ");

            command.Transaction.Should().Be(transaction);

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.Guid
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_id"
                        && p.Value != null
                        && ((Guid)p.Value) == user.Id
                )
            );

            parameterCollection.Received().Add( 
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_username"
                        && p.Value != null
                        && ((string)p.Value) == user.UserName
                )
            );

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_firstname"
                        && p.Value != null
                        && ((string)p.Value) == user.FirstName
                )
            );

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_lastname"
                        && p.Value != null
                        && ((string)p.Value) == user.LastName
                )
            );

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_email"
                        && p.Value != null
                        && ((string)p.Value) == user.Email
                )
            );

            command.Received().PrepareAsync(); 

            command.Received().ExecuteNonQueryAsync();
        }

        [Fact]
        public void Save_UpdatesUser_IfUserExistsAndIsModified()
        {
            //Arrange
            var existingUser = fixture.Create<User>();
            var modifiedUser = new User(
                existingUser.Id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            );

            IDataReader reader = Substitute.For<IDataReader>();
            reader.Read().Returns(true, false);
            reader.GetOrdinal("id").Returns(0);
            reader.GetOrdinal("username").Returns(1);
            reader.GetOrdinal("firstname").Returns(2);
            reader.GetOrdinal("lastname").Returns(3);
            reader.GetOrdinal("email").Returns(4);

            reader.GetGuid(0).Returns(existingUser.Id);
            reader.GetString(1).Returns(existingUser.UserName);
            reader.GetString(2).Returns(existingUser.FirstName);
            reader.GetString(3).Returns(existingUser.LastName);
            reader.GetString(4).Returns(existingUser.Email);
            
            command.ExecuteReaderAsync().Returns(reader);

            repository.SetTransaction(transaction);

            var emptyParameterList = new List<IPostgresParameter>();
            var notEmptyParameterList = new List<IPostgresParameter>() { Substitute.For<IPostgresParameter>() };
            
            parameterCollection.GetEnumerator().Returns(
                emptyParameterList.GetEnumerator(), 
                notEmptyParameterList.GetEnumerator(),
                notEmptyParameterList.GetEnumerator(),
                notEmptyParameterList.GetEnumerator()
            );

            //Act
            var saveTask = repository.Save(modifiedUser);
            saveTask.Wait();

            //Assert
            command.CommandText.Should().Be("UPDATE users.users SET username = :p_username, firstname = :p_firstname, lastname = :p_lastname, email = :p_email WHERE id = :p_id");
        
            command.Transaction.Should().Be(transaction);

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.Guid
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_id"
                        && p.Value != null
                        && ((Guid)p.Value) == modifiedUser.Id
                )
            );

            parameterCollection.Received().Add( //2 calls because of the GetUser call
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_username"
                        && p.Value != null
                        && ((string)p.Value) == modifiedUser.UserName
                )
            );

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_firstname"
                        && p.Value != null
                        && ((string)p.Value) == modifiedUser.FirstName
                )
            );

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_lastname"
                        && p.Value != null
                        && ((string)p.Value) == modifiedUser.LastName
                )
            );

            parameterCollection.Received().Add(
                Arg.Is<IPostgresParameter>(
                    p => p.DbType == DbType.String
                        && p.Direction == ParameterDirection.Input
                        && p.ParameterName == "p_email"
                        && p.Value != null
                        && ((string)p.Value) == modifiedUser.Email
                )
            );
        }

    }

}