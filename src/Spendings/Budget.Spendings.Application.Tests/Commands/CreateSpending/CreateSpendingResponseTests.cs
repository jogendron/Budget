using Budget.Spendings.Application.Commands.CreateSpending;

using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpending;

public class CreateSpendingResponseTests
{
    public CreateSpendingResponseTests()
    {   
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var response = new CreateSpendingResponse(id);

        //Assert
        response.Id.Should().Be(id);
    }
}