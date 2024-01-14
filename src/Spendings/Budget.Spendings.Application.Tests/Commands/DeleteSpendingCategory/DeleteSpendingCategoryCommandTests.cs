using Budget.Spendings.Application.Commands.DeleteSpendingCategory;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.DeleteSpendingCategory;

public class DeleteSpendingCategoryCommandTests
{
    private readonly Fixture _fixture;

    public DeleteSpendingCategoryCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        //Act
        var command = new DeleteSpendingCategoryCommand(id, userId);

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
        var action = (() => new DeleteSpendingCategoryCommand(Guid.Empty, userId));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var actionNull = (() => new DeleteSpendingCategoryCommand(id, null!));
        var actionEmpty = (() => new DeleteSpendingCategoryCommand(id, string.Empty));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }
}