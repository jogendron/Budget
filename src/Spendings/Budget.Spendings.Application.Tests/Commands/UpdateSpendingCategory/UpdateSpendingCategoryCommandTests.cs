using Budget.Spendings.Application.Commands.UpdateSpendingCategory;

using AutoFixture;
using FluentAssertions;
using Budget.Spendings.Domain.Entities;

namespace Budget.Spendings.Application.Tests.Commands.UpdateSpendingCategory;

public class UpdateSpendingCategoryCommandTests
{
    private Fixture _fixture;

    public UpdateSpendingCategoryCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var categoryId = _fixture.Create<Guid>();
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var frequency = _fixture.Create<Frequency>();
        var isPeriodOpened = _fixture.Create<bool>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var command = new UpdateSpendingCategoryCommand(
            userId,
            categoryId,
            name,
            frequency,
            isPeriodOpened,
            amount,
            description
        );

        //Assert
        command.SpendingCategoryId.Should().Be(categoryId);
        command.UserId.Should().Be(userId);
        command.Name.Should().Be(name);
        command.Frequency.Should().Be(frequency);
        command.IsPeriodOpened.Should().Be(isPeriodOpened);
        command.Amount.Should().Be(amount);
        command.Description.Should().Be(description);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenSpendingCategoryIdIsEmpty()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var frequency = _fixture.Create<Frequency>();
        var isPeriodOpened = _fixture.Create<bool>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var command = (() => new UpdateSpendingCategoryCommand(
            userId,
            Guid.Empty,
            name,
            frequency,
            isPeriodOpened,
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
        var name = _fixture.Create<string>();
        var frequency = _fixture.Create<Frequency>();
        var isPeriodOpened = _fixture.Create<bool>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();

        //Act
        var commandNull = (() => new UpdateSpendingCategoryCommand(
            null!,
            categoryId,
            name,
            frequency,
            isPeriodOpened,
            amount,
            description
        ));

        var commandEmpty = (() => new UpdateSpendingCategoryCommand(
            string.Empty,
            categoryId,
            name,
            frequency,
            isPeriodOpened,
            amount,
            description
        ));

        //Assert
        commandNull.Should().Throw<ArgumentException>();
        commandEmpty.Should().Throw<ArgumentException>();
    }
}