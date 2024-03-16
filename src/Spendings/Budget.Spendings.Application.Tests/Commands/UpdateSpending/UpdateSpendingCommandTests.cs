using Budget.Spendings.Application.Commands.UpdateSpending;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.UpdateSpending;

public class UpdateSpendingCommandTests
{
    private readonly Fixture _fixture;

    public UpdateSpendingCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var spendingId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = _fixture.Create<DateTime>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var command = new UpdateSpendingCommand(
            userId,
            spendingId,
            categoryId,
            date,
            amount,
            description
        );

        //Assert
        command.UserId.Should().Be(userId);
        command.SpendingId.Should().Be(spendingId);
        command.CategoryId.Should().Be(categoryId);
        command.Date.Should().Be(date);
        command.Amount.Should().Be(amount);
        command.Description.Should().Be(description);        
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenSpendingIdIsEmpty()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var spendingId = Guid.Empty;
        var categoryId = Guid.NewGuid();
        var date = _fixture.Create<DateTime>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var command = (() => new UpdateSpendingCommand(
            userId,
            spendingId,
            categoryId,
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
        var spendingId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var date = _fixture.Create<DateTime>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var commandNull = (() => new UpdateSpendingCommand(
            null!,
            spendingId,
            categoryId,
            date,
            amount,
            description
        ));

        var commandEmpty = (() => new UpdateSpendingCommand(
            string.Empty,
            spendingId,
            categoryId,
            date,
            amount,
            description
        ));

        //Assert
        commandNull.Should().Throw<ArgumentException>();
        commandEmpty.Should().Throw<ArgumentException>();
    }
}