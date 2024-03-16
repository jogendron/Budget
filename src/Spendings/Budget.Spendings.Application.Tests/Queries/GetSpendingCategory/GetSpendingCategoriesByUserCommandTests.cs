using Budget.Spendings.Application.Queries.GetSpendingCategory;

using AutoFixture;
using FluentAssertions;
using NSubstitute.Core;

namespace Budget.Spendings.Application.Tests.Queries.GetSpendingCategory;

public class GetSpendingCategoriesByUserCommandTests
{
    private readonly Fixture _fixture;

    public GetSpendingCategoriesByUserCommandTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        //Arrange
        string userId = _fixture.Create<string>();

        //Act
        var command = new GetSpendingCategoriesByUserCommand(userId);

        //Assert
        command.UserId.Should().Be(userId);
    }

    [Fact]
    public void Constructor_ThrowsArgumentException_WhenUserIdIsNullOrEmpty()
    {
        //Arrange

        //Act
        var actionNull = (() => new GetSpendingCategoriesByUserCommand(null!));
        var actionEmpty = (() => new GetSpendingCategoriesByUserCommand(string.Empty));

        //Assert
        actionNull.Should().Throw<ArgumentException>();
        actionEmpty.Should().Throw<ArgumentException>();
    }
}