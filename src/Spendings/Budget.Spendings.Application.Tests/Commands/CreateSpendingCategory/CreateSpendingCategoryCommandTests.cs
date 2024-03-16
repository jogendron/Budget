using Budget.Spendings.Application.Commands.CreateSpendingCategory;
using Budget.Spendings.Domain.Entities;

using FluentAssertions;
using AutoFixture;
using NSubstitute.Core;

namespace Budget.Spendings.Application.Tests.Commands.CreateSpendingCategory;

public class CreateSpendingCategoryCommandTests
{
    private Fixture _fixture;

    public CreateSpendingCategoryCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_Assigns_Properties()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();
        var frequency = _fixture.Create<Frequency>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();
        
        //Act
        var command = new CreateSpendingCategoryCommand(
            userId,
            name,
            frequency,
            amount,
            description
        );

        //Assert
        command.UserId.Should().Be(userId);
        command.Name.Should().Be(name);
        command.Frequency.Should().Be(frequency);
        command.Amount.Should().Be(amount);
        command.Description.Should().Be(description);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var name = _fixture.Create<string>();
        var frequency = _fixture.Create<Frequency>();
        var amount = _fixture.Create<double>();
        var description = _fixture.Create<string>();
        
        //Act
        var commandNull = (() => new CreateSpendingCategoryCommand(
            null!,
            name,
            frequency,
            amount,
            description
        ));

        var commandEmpty = (() => new CreateSpendingCategoryCommand(
            null!,
            name,
            frequency,
            amount,
            description
        ));

        //Assert
        commandNull.Should().Throw<ArgumentException>();
        commandEmpty.Should().Throw<ArgumentException>();
    }

}