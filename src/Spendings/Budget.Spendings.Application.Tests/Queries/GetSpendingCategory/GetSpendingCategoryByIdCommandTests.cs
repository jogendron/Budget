using Budget.Spendings.Application.Queries.GetSpendingCategory;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoryByIdCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingCategoryByIdCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        //Arrange
        var id = Guid.NewGuid();
        var userId = _fixture.Create<string>();

        //Act
        var command = new GetSpendingCategoryByIdCommand(id, userId);

        //Assert
        command.Id.Should().Be(id);
        command.UserId.Should().Be(userId);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange
        var id = Guid.NewGuid();

        //Act
        var actionNull = (() =>new GetSpendingCategoryByIdCommand(id, null!));
        var actionEmpty = (() =>new GetSpendingCategoryByIdCommand(id, string.Empty));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenIdIsEmpty()
    {
        //Arrange
        var id = Guid.Empty;
        var userId = _fixture.Create<string>();

        //Act
        var action = (() =>new GetSpendingCategoryByIdCommand(id, userId));

        //Assert
        action.Should().Throw<ArgumentException>();
    }
}