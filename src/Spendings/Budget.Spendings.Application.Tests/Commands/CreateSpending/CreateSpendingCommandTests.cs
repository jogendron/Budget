using Budget.Spendings.Application.Commands.CreateSpending;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpending;

public class CreateSpendingCommandTests
{
    private readonly Fixture _fixture;

    public CreateSpendingCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var categoryId = _fixture.Create<Guid>();
        var userId = _fixture.Create<string>();
        var date = _fixture.Create<DateTime>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var command = new CreateSpendingCommand(
            categoryId,
            userId,
            date,
            amount,
            description
        );

        //Assert
        command.CategoryId.Should().Be(categoryId);
        command.UserId.Should().Be(userId);
        command.Date.Should().Be(date);
        command.Amount.Should().Be(amount);
        command.Description.Should().Be(description);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenCategoryIdIsEmpty()
    {
        //Arrange
        var categoryId = Guid.Empty;
        var userId = _fixture.Create<string>();
        var date = _fixture.Create<DateTime>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var command = (() => new CreateSpendingCommand(
            categoryId,
            userId,
            date,
            amount,
            description
        ));

        //Assert
        command.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var categoryId = _fixture.Create<Guid>();
        var date = _fixture.Create<DateTime>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var commandNull = (() => new CreateSpendingCommand(
            categoryId,
            null!,
            date,
            amount,
            description
        ));

        var commandEmpty = (() => new CreateSpendingCommand(
            categoryId,
            null!,
            date,
            amount,
            description
        ));

        //Assert
        commandNull.Should().Throw<ArgumentException>();
        commandEmpty.Should().Throw<ArgumentException>();
    }
}