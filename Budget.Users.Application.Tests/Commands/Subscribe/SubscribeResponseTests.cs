using System;
using Xunit;
using FluentAssertions;

using Budget.Users.Application.Commands.Subscribe;

namespace Budget.Users.Application.Tests
{
    public class SubscribeResponseTests
    {
        [Fact]
        public void Constructor_AssignsProperties()
        {
            //Arrange
            Guid id = Guid.NewGuid();

            //Act
            SubscribeResponse response = new SubscribeResponse(id);

            //Assert
            response.Id.Should().Be(id);
        }
    }
}