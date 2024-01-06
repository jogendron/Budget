using Budget.Spendings.Application.Commands.DeleteSpending;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.DeleteSpending;

public class DeleteSpendingsCommandTests
{
    private readonly Fixture _fixture;

    public DeleteSpendingsCommandTests()
    {
        _fixture = new Fixture();        
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var id = _fixture.Create<Guid>();
        var userId = _fixture.Create<string>();

        //Act
        var command = new DeleteSpendingCommand(id, userId);

        //Assert
        command.Id.Should().Be(id);
        command.UserId.Should().Be(userId);
    }
}