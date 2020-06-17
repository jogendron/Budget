using Xunit;
using AutoFixture;
using FluentAssertions;

using Budget.Users.Application.Queries.GetUser; 

namespace Budget.Users.Application.Tests.Queries.GetUser
{
    public class GetUserRequestTests
    {
        Fixture fixture;

        public GetUserRequestTests()
        {
            fixture = new Fixture();
        }

        [Fact]
        public void Constructor_AssignsProperties()
        {
            //Arrange
            string userName = fixture.Create<string>();

            //Act
            GetUserRequest request = new GetUserRequest(userName);

            //Assert
            request.UserName.Should().Be(userName);
        }
    }
}