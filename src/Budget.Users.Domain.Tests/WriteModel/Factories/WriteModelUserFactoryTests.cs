using Xunit;
using AutoFixture;
using FluentAssertions;
using NSubstitute;

using System;
using System.Collections.Generic;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.WriteModel.Events;
using Budget.Users.Domain.WriteModel;
using Budget.Users.Domain.WriteModel.Factories;
using Budget.Users.Domain.WriteModel.Services;

namespace Budget.Users.Domain.Tests.WriteModel.Factories
{
    public class WriteModelUserFactoryTests
    {
        private Fixture fixture;

        private ICryptService cryptService;
        private string cryptServiceOutput;

        WriteModelUserFactory factory;

        public WriteModelUserFactoryTests()
        {
            fixture = new Fixture();

            cryptService = Substitute.For<ICryptService>();
            cryptServiceOutput = fixture.Create<string>();
            cryptService.Crypt(Arg.Any<string>()).Returns(cryptServiceOutput);

            factory = new WriteModelUserFactory(cryptService);
        }

        [Fact]
        public void Create_CryptsPassword_CreatesValidUser()
        {
            //Arrange
            string userName = fixture.Create<string>();
            string firstName = fixture.Create<string>();
            string lastName = fixture.Create<string>();
            string email = "abc@def.com";
            string password = fixture.Create<string>();

            //Act
            User user = factory.Create(userName, firstName, lastName, email, password);

            //Assert
            cryptService.Received(1).Crypt(Arg.Is(password));

            user.UserName.Should().Be(userName);
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().NotBeNull();
            user.Email.Address.Should().Be(email);
            user.EncryptedPassword.Should().Be(cryptServiceOutput);
        }

        [Fact]
        public void Load_AssignsId_AppliesAllChanges()
        {
            //Arrange          
            Guid id = Guid.NewGuid();

            UserSubscribed subscriptionEvent = new UserSubscribed(
                id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                "abc@def.com"
            );

            PasswordChanged passwordChangeEvent = new PasswordChanged(id, fixture.Create<string>());

            IEnumerable<Event> changes = new List<Event>() { subscriptionEvent, passwordChangeEvent };

            //Act
            User user = factory.Load(id, changes);

            //Assert
            user.Should().NotBeNull();
            user.Id.Should().Be(id);
            user.Changes.Should().BeEquivalentTo(changes);
            user.UserName.Should().Be(subscriptionEvent.UserName);
            user.FirstName.Should().Be(subscriptionEvent.FirstName);
            user.LastName.Should().Be(subscriptionEvent.LastName);
            user.Email.Address.Should().Be(subscriptionEvent.Email);
            user.EncryptedPassword.Should().Be(passwordChangeEvent.EncryptedPassword);
        }

    }
}
