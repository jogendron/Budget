using Budget.Spendings.Application.Queries.GetSpending;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpending;

public class GetSpendingCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingCommandTests()
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
        var command = new GetSpendingByIdCommand(id, userId);

        //Assert
        command.Id.Should().Be(id);
        command.UserId.Should().Be(userId);
    }
}