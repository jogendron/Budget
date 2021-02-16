using System;
using System.Collections.Generic;
using System.Reflection;
using Xunit;

using AutoFixture;
using FluentAssertions;

using Budget.EventSourcing.Events;
using Budget.Users.Domain.WriteModel;

namespace Budget.Users.Domain.Tests.WriteModel
{
    public class UserTests
    {
        private Fixture fixture;

        private string userName;
        private string firstName;
        private string lastName;
        private string email;
        private string encryptedPassword;

        public UserTests()
        {
            fixture = new Fixture();

            userName = fixture.Create<string>();
            firstName = fixture.Create<string>();
            lastName = fixture.Create<string>();
            email = "abc@def.com";
            encryptedPassword = fixture.Create<string>();
        }

        [Fact]
        public void NewUserConstructor_AssignsProperties()
        {
            //Arrange

            //Act
            User user = new User(userName, firstName, lastName, email, encryptedPassword);

            //Assert
            user.Id.Should().NotBeEmpty();
            user.UserName.Should().Be(userName);
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Address.Should().Be(email);
            user.EncryptedPassword.Should().Be(encryptedPassword);
        }

        [Fact]
        public void ExistingUserConstructor_AssignsProperties()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            List<Event> changes = new List<Event>();

            //Act
            User user = new User(id, changes);

            //Assert
            user.Id.Should().Be(id);
            user.Changes.Should().BeEquivalentTo(changes);
        }

        [Fact]
        public void UserName_ThrowsArgumentException_WhenAssignedToNullOrEmpty()
        {
            //Arrange

            //Act
            Action nullAssignment = (() => new User(null, firstName, lastName, email, encryptedPassword));
            Action emptyAssignment = (() => new User(string.Empty, firstName, lastName, email, encryptedPassword));

            //Assert
            Assert.Throws<ArgumentException>(nullAssignment);

            Assert.Throws<ArgumentException>(emptyAssignment);
        }

        [Fact]
        public void FirstName_ThrowsArgumentException_WhenAssignedToNullOrEmpty()
        {
            //Arrange

            //Act
            Action nullAssignment = (() => new User(userName, null, lastName, email, encryptedPassword));
            Action emptyAssignment = (() => new User(userName, string.Empty, lastName, email, encryptedPassword));

            //Assert
            Assert.Throws<ArgumentException>(nullAssignment);
            Assert.Throws<ArgumentException>(emptyAssignment);
        }

        [Fact]
        public void LastName_ThrowsArgumentException_WhenAssignedToNullOrEmpty()
        {
            //Arrange

            //Act
            Action nullAssignment = (() => new User(userName, firstName, null, email, encryptedPassword));
            Action emptyAssignment = (() => new User(userName, firstName, string.Empty, email, encryptedPassword));

            //Assert
            Assert.Throws<ArgumentException>(nullAssignment);

            Assert.Throws<ArgumentException>(emptyAssignment);
        }

        [Fact]
        public void Email_ThrowsArgumentNullException_WhenAssignedToNullOrEmpty()
        {
            //Arrange

            //Act
            Action nullAssignment = (() => new User(userName, firstName, lastName, null, encryptedPassword));
            
            //Assert
            Assert.Throws<ArgumentNullException>(nullAssignment);
        }

        [Fact]
        public void Email_ThrowsArgumentException_WhenAssignedToEmptyOrInvalidEmail()
        {
            //Arrange

            //Act
            Action emptyAssignment = (() => new User(userName, firstName, lastName, string.Empty, encryptedPassword));
            
            //Assert
            Assert.Throws<ArgumentException>(emptyAssignment);
        }

        [Fact]
        public void Email_ThrowsFormatException_WhenAssignedToInvalidEmail()
        {
            //Arrange

            //Act
            Action invalidAssignment = (() => new User(userName, firstName, lastName, "toto", encryptedPassword));

            //Assert
            Assert.Throws<FormatException>(invalidAssignment);
        }

        [Fact]
        public void EncryptedPassword_ThrowsArgumentException_WhenAssignedToNullOrEmpty()
        {
            //Arrange

            //Act
            Action nullAssignment = (() => new User(userName, firstName, lastName, email, null));
            Action empyAssignment = (() => new User(userName, firstName, lastName, email, string.Empty));

            //Assert
            Assert.Throws<ArgumentException>(nullAssignment);

            Assert.Throws<ArgumentException>(empyAssignment);
        }
        
    }
}
