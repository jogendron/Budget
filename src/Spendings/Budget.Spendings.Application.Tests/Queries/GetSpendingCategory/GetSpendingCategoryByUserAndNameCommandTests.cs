using Budget.Spendings.Application.Queries.GetSpendingCategory;

using AutoFixture;
using FluentAssertions;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoryByUserAndNameCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingCategoryByUserAndNameCommandTests()
    {
        _fixture = new Fixture();
    }

    public void Constructor_SetsPropperties()
    {
        //Arrange
        var userId = _fixture.Create<string>();
        var name = _fixture.Create<string>();

        //Act
        var command = new GetSpendingCategoryByUserAndNameCommand(userId, name);

        //Assert
        command.UserId.Should().Be(userId);
        command.Name.Should().Be(name);
    }
}