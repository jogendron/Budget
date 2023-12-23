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
            spendingId,
            userId,
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
}