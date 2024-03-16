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
        var command = new DeleteSpendingCommand(userId, id);

        //Assert
        command.Id.Should().Be(id);
        command.UserId.Should().Be(userId);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenIdIsEmpty()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        //Act
        var action = (() => new DeleteSpendingCommand(userId, Guid.Empty));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var actionNull = (() => new DeleteSpendingCommand(null!, id));
        var actionEmpty = (() => new DeleteSpendingCommand(string.Empty, id));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }
}