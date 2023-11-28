using Budget.Spendings.Application.Queries.GetSpending;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpending;

public class GetSpendingsByCategoryCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingsByCategoryCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var categoryId = _fixture.Create<Guid>();
        var beginDate = _fixture.Create<DateTime>();
        var endDate = _fixture.Create<DateTime>();

        //Act
        var command = new GetSpendingsByCategoryCommand(
            userId,
            categoryId,
            beginDate,
            endDate
        );

        //Assert
        command.UserId.Should().Be(userId);
        command.CategoryId.Should().Be(categoryId);
        command.BeginDate.Should().Be(beginDate);
        command.EndDate.Should().Be(endDate);
    }
}