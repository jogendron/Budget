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

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenIdIsEmpty()
    {
        //Arrange
        var userId = _fixture.Create<string>();

        //Act
        var action = (() => new DeleteSpendingCommand(Guid.Empty, userId));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var actionNull = (() => new DeleteSpendingCommand(id, null!));
        var actionEmpty = (() => new DeleteSpendingCommand(id, string.Empty));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }
}