using Xunit;
using AutoFixture;
using FluentAssertions;

using Budget.Users.Application.Commands.Subscribe;

namespace Budget.Users.Application.Tests.Commands.Subscribe
{
    public class UnitTest1
    {
        private Fixture fixture;

        public UnitTest1()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Constructor_AssignsProperties()
        {
            //Arrange
            string userName = fixture.Create<string>();
            string firstName = fixture.Create<string>();
            string lastName = fixture.Create<string>();
            string email = fixture.Create<string>();
            string password = fixture.Create<string>();

            //Act
            SubscribeCommand command = new SubscribeCommand(userName, firstName, lastName, email, password);

            //Assert
            command.UserName.Should().Be(userName);
            command.FirstName.Should().Be(firstName);
            command.LastName.Should().Be(lastName);
            command.Email.Should().Be(email);
            command.Password.Should().Be(password);
        }
    }
}
