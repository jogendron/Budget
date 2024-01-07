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
}