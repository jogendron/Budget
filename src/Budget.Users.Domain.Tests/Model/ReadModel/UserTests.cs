using Xunit;
using AutoFixture;
using FluentAssertions;

using System;

using Budget.Users.Domain.Model.ReadModel;

namespace Budget.Users.Domain.Tests.Model.ReadModel
{
    public class ReadModelUserTests
    {
        Fixture fixture;

        public ReadModelUserTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Constructor_AssignsProperties()
        {
            //Arrange
            Guid id = Guid.NewGuid();
            string userName = fixture.Create<string>();
            string firstName = fixture.Create<string>();
            string lastName = fixture.Create<string>();
            string email = fixture.Create<string>();

            //Act
            User user = new User(id, userName, firstName, lastName, email);

            //Assert
            user.Id.Should().Be(id);
            user.UserName.Should().Be(userName);
            user.FirstName.Should().Be(firstName);
            user.LastName.Should().Be(lastName);
            user.Email.Should().Be(email);
        }
    }
}