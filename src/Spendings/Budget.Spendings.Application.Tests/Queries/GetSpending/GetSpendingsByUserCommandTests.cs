using AutoFixture;
using Budget.Spendings.Application.Queries.GetSpending;

using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpending;

public class GetSpendingsByUserCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingsByUserCommandTests()
    {
        _fixture = new Fixture();    
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        //Act
        var command = new GetSpendingsByUserCommand(userId, beginDate, endDate);

        //Assert
        command.UserId.Should().Be(userId);
        command.BeginDate.Should().Be(beginDate);
        command.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsEmpty()
    {
        //Arrange
        var userId = string.Empty;
        var beginDate = _fixture.Create<DateTime>();
        var endDate = _fixture.Create<DateTime>();

        //Act
        var action = (() => new GetSpendingsByUserCommand(userId, beginDate, endDate));
         
        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenBeginDateIsAfterEndDate()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var beginDate = DateTime.MaxValue;
        var endDate = DateTime.MinValue;

        //Act
        var action = (() => new GetSpendingsByUserCommand(userId, beginDate, endDate));
         
        //Assert
        action.Should().Throw<ArgumentException>();
    }
}