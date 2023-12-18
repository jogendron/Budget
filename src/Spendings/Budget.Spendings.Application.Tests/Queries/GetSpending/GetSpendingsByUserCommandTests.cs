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
        var beginDate = _fixture.Create<DateTime>();
        var endDate = _fixture.Create<DateTime>();

        //Act
        var command = new GetSpendingsByUserCommand(userId, beginDate, endDate);

        //Assert
        command.UserId.Should().Be(userId);
        command.BeginDate.Should().Be(beginDate);
        command.EndDate.Should().Be(endDate);
    }
}