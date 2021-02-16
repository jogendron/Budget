using System;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using NSubstitute;
using AutoFixture;

using Budget.Users.MongoDbAdapters.Entities;
using Budget.Users.Domain.WriteModel.Services;
using Budget.Users.Domain.WriteModel.Factories;
using Budget.EventSourcing.Events;
using Budget.Users.Domain.WriteModel.Events;

namespace Budget.Users.MongoDbAdapters.Tests.Entities
{
    public class UserTests
    {
        Fixture fixture;

        public UserTests()
        {
            fixture = new Fixture();
        }


        [Fact]
        public void DefaultConstructor_Sets_EmptyValue()
        {
            //Arrange
            var user = new User();

            //Act

            //Assert
            user.Id.Should().Be(Guid.Empty);
            user.Changes.Should().BeNull();
        }

        [Fact]
        public void ParameteredConstructor_sets_Values()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            
            var cryptService = Substitute.For<ICryptService>();
            cryptService.Crypt(Arg.Any<string>()).Returns(args => args[0]);
            var userFactory = new WriteModelUserFactory(cryptService);
            var domainUser = userFactory.Create("Kakarot", "Son", "Goku", "songoku@dbz.com", "chichi123");
            
            //Act
            var user = new User(domainUser);

            //Assert
            user.Id.Should().Be(domainUser.Id);
            user.Changes.Should().BeEquivalentTo(domainUser.Changes);
        }

        [Fact]
        public void Id_CanBeSet()
        {
            //Arrange
            var user = new User();

            //Act
            user.Id = Guid.NewGuid();

            //Assert
            user.Id.Should().NotBeEmpty();
        }

        [Fact]
        public void Changes_CanBeSet()
        {
            //Arrange
            var user = new User();
            
            var changes = new List<Event>();
            changes.Add(fixture.Create<UserSubscribed>());
            changes.Add(fixture.Create<PasswordChanged>());

            //Act
            user.Changes = changes;

            //Assert
            user.Changes.Should().BeEquivalentTo(changes);
        }

    }
}