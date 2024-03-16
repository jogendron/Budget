using Budget.Spendings.Application.Queries.GetSpending;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpending;

public class GetSpendingByIdCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingByIdCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var id = _fixture.Create<Guid>();
        var userId = _fixture.Create<string>();

        //Act
        var command = new GetSpendingByIdCommand(userId, id);

        //Assert
        command.Id.Should().Be(id);
        command.UserId.Should().Be(userId);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenIdIsEmpty()
    {
        //Arrange
        var id = Guid.Empty;
        var userId = _fixture.Create<string>();

        //Act
        var action = (() => new GetSpendingByIdCommand(userId, id));

        //Assert
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsEmpty()
    {
        //Arrange
        var id = _fixture.Create<Guid>();;
        var userId = string.Empty;

        //Act
        var action = (() => new GetSpendingByIdCommand(userId, id));

        //Assert
        action.Should().Throw<ArgumentException>();
    }
}