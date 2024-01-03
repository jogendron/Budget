using Budget.Spendings.Application.Commands.DeleteSpendings;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Commands.DeleteSpendings;

public class DeleteSpendingsCommandTests
{
    private readonly Fixture _fixture;

    public DeleteSpendingsCommandTests()
    {
        _fixture = new Fixture();        
    }

    [Fact]
    public void Constructor_AssignsProperties()
    {
        //Arrange
        var ids = _fixture.Create<List<Guid>>();
        var userId = _fixture.Create<string>();

        //Act
        var command = new DeleteSpendingsCommand(ids, userId);

        //Assert
        command.SpendingIds.Should().BeEquivalentTo(ids);
        command.UserId.Should().Be(userId);
    }
}