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
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

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

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsEmpty()
    {
        //Arrange
        var categoryId = _fixture.Create<Guid>();
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        //Act
        var actionNull = (() => new GetSpendingsByCategoryCommand(null!, categoryId, beginDate, endDate));
        var actionEmpty = (() => new GetSpendingsByCategoryCommand(string.Empty, categoryId, beginDate, endDate));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenCategoryIdIsEmpty()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var beginDate = DateTime.MinValue;
        var endDate = DateTime.MaxValue;

        //Act
        var action = (() => new GetSpendingsByCategoryCommand(userId, Guid.Empty, beginDate, endDate));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenBeginDateIsAfterEndDate()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var categoryId = _fixture.Create<Guid>();
        var beginDate = DateTime.MaxValue;
        var endDate = DateTime.MinValue;

        //Act
        var action = (() => new GetSpendingsByCategoryCommand(userId, categoryId, beginDate, endDate));

        //Assert
        action.Should().Throw<ArgumentException>();
    }
}